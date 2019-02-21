namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using Blueve.ObjectRedisMapping.Proxy;

    /// <summary>
    /// The proxy type builder.
    /// </summary>
    internal class ProxyTypeBuilder
    {
        private const string GetMethodPrefix = "get_";
        private const string SetMethodPrefix = "set_";
        private const string ProxyModuleName = "Proxy";
        private const string StubFieldName = "_stub";

        private readonly Type originType;
        private readonly AppDomain doamin;
        private readonly AssemblyName assembly;
        private readonly AssemblyBuilder assemblyBuilder;
        private readonly ModuleBuilder moduleBuilder;
        private readonly TypeBuilder typeBuilder;

        private FieldInfo stubFieldInfo;
        private readonly ILGenerator ctorBuilder;

        /// <summary>
        /// Initialize an instance of <see cref="ProxyTypeBuilder"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        public ProxyTypeBuilder(Type type)
        {
            this.originType = type;
            this.doamin = AppDomain.CurrentDomain;
            this.assembly = new AssemblyName(Guid.NewGuid().ToString());
            this.assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(this.assembly, AssemblyBuilderAccess.Run);
            this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(ProxyModuleName);
            this.typeBuilder = this.moduleBuilder.DefineType(
                type.FullName + ProxyModuleName, TypeAttributes.Public | TypeAttributes.Class, type);

            // Implement IProxy for all proxy type.
            this.typeBuilder.AddInterfaceImplementation(typeof(IProxy));
        }

        /// <summary>
        /// Inject a proxy stub.
        /// </summary>
        /// <returns>The proxy type builder.</returns>
        public ProxyTypeBuilder InjectStub()
        {
            this.stubFieldInfo = this.typeBuilder.DefineField(
                StubFieldName, typeof(DynamicProxyStub), FieldAttributes.Private | FieldAttributes.InitOnly);

            return this;
        }

        /// <summary>
        /// Inject an existing proxy stub.
        /// </summary>
        /// <param name="stubField">The existing stub field.</param>
        /// <returns>The proxy type builder.</returns>
        public ProxyTypeBuilder InjectStub(FieldInfo stubField)
        {
            this.stubFieldInfo = stubField;

            return this;
        }

        /// <summary>
        /// Create a constructor for current type.
        /// The constructor include one parameter 'stub' and will assign it to this._stub in it's implementation.
        /// </summary>
        /// <returns>The proxy type builder.</returns>
        public ProxyTypeBuilder CreateCtor()
        {
            // Build a constructor and inject IDatabaseClient.
            var constructorBuilder = this.typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(DynamicProxyStub) });
            var constructorILGenerator = constructorBuilder.GetILGenerator();
            GenerateConstructorIL(this.stubFieldInfo, constructorILGenerator);

            return this;
        }

        /// <summary>
        /// Create a constructor for container type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>The proxy type builder.</returns>
        public ProxyTypeBuilder CreateCtor<T>()
        {
            // Build a constructor and inject IDatabaseClient.
            var constructorBuilder = this.typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(string), typeof(DynamicProxyStub) });
            var constructorILGenerator = constructorBuilder.GetILGenerator();
            GenerateCollectionConstructorIL<T>(this.stubFieldInfo, constructorILGenerator);

            return this;
        }

        /// <summary>
        /// Override a property.
        /// </summary>
        /// <param name="propInfo">The target property.</param>
        /// <param name="propTypeMetadata">The type metadata of target property.</param>
        /// <param name="propDbKey">The database key of target property.</param>
        /// <param name="readOnly">True if the property is readonly.</param>
        /// <returns>The proxy type builder.</returns>
        public ProxyTypeBuilder OverrideProperty(
            PropertyInfo propInfo,
            TypeMetadata propTypeMetadata,
            string propDbKey,
            bool readOnly)
        {
            // TODO: Add check to avoid add same property twice.
            var propertyBuilder = this.typeBuilder.DefineProperty(propInfo.Name, PropertyAttributes.None, propInfo.PropertyType, null);

            // Setup getter.
            var getterBuilder = this.typeBuilder.DefineMethod(
                GetMethodPrefix + propInfo.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                propInfo.PropertyType,
                Type.EmptyTypes);
            var getterILGenerator = getterBuilder.GetILGenerator();

            // Setup setter.
            var setterBuilder = this.typeBuilder.DefineMethod(
                SetMethodPrefix + propInfo.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                null,
                new[] { propInfo.PropertyType });
            var setterILGenerator = setterBuilder.GetILGenerator();

            // Generate IL for getter.
            GenerateGetterIL(this.stubFieldInfo, propDbKey, propTypeMetadata, getterILGenerator, readOnly);

            // Generate IL for setter.
            if (readOnly)
            {
                GenerateNotAvailableSetterIL(setterILGenerator);
            }
            else
            {
                GenerateSetterIL(this.stubFieldInfo, propDbKey, propTypeMetadata, setterILGenerator);
            }

            // Binding to property.
            propertyBuilder.SetGetMethod(getterBuilder);
            propertyBuilder.SetSetMethod(setterBuilder);

            return this;
        }

        /// <summary>
        /// Override the GetElemAt/SetElemAt method.
        /// </summary>
        /// <param name="elemTypeMetadata">The element type.</param>
        /// <returns>The proxy type builder.</returns>
        public ProxyTypeBuilder OverrideElemMethods(
            TypeMetadata elemTypeMetadata,
            bool readOnly)
        {
            // Setup GetElemAt.
            var getterBuilder = this.typeBuilder.DefineMethod(
                "GetElemAt",
                MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot | MethodAttributes.Public,
                elemTypeMetadata.Type,
                new[] { typeof(int) });
            GenerateGetElemAtIL(elemTypeMetadata, getterBuilder.GetILGenerator(), readOnly);

            // Setup SetElemAt.
            var setterBuilder = this.typeBuilder.DefineMethod(
                "SetElemAt",
                MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.ReuseSlot | MethodAttributes.Public,
                null,
                new[] { typeof(int), elemTypeMetadata.Type });
            if (readOnly)
            {
                GenerateNotAvailableSetterIL(setterBuilder.GetILGenerator());
            }
            else
            {
                GenerateSetElemAtIL(elemTypeMetadata, setterBuilder.GetILGenerator());
            }

            return this;
        }

        /// <summary>
        /// Create the proxy type.
        /// </summary>
        /// <returns>The proxy type.</returns>
        public Type CreateType()
        {
            var proxyTypeInfo = this.typeBuilder.CreateTypeInfo();
            var proxyType = proxyTypeInfo.AsType();
            return proxyType;
        }

        /// <summary>
        /// Generate IL for the constructor.
        /// </summary>
        /// <param name="stubField">The proxy stub field.</param>
        /// <param name="ctorILGenerator">The constructor's IL generator.</param>
        private static void GenerateConstructorIL(
            FieldInfo stubField,
            ILGenerator ctorILGenerator)
        {
            ctorILGenerator.Emit(OpCodes.Ldarg_0);
            ctorILGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            ctorILGenerator.Emit(OpCodes.Ldarg_0);
            ctorILGenerator.Emit(OpCodes.Ldarg_1);
            ctorILGenerator.Emit(OpCodes.Stfld, stubField);
            ctorILGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate IL for the container type constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stubField">The proxy stub field</param>
        /// <param name="ctorILGenerator">The constructor's IL generator.</param>
        private static void GenerateCollectionConstructorIL<T>(
            FieldInfo stubField,
            ILGenerator ctorILGenerator)
        {
            ctorILGenerator.Emit(OpCodes.Ldarg_0);
            ctorILGenerator.Emit(OpCodes.Ldarg_1);
            ctorILGenerator.Emit(OpCodes.Ldarg_2);
            ctorILGenerator.Emit(OpCodes.Call, typeof(ListProxy<T>).GetConstructor(new[] { typeof(string), typeof(DynamicProxyStub) }));
            ctorILGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate IL for the getter.
        /// </summary>
        /// <param name="stubField">The proxy stub field.</param>
        /// <param name="dbKey">The database key of this property.</param>
        /// <param name="propMetadata">The type metadata for this property.</param>
        /// <param name="ilGenerator">The getter's IL generator.</param>
        /// <param name="readOnly">True if the proxy from the getter is readonly.</param>
        private static void GenerateGetterIL(
            FieldInfo stubField,
            string dbKey,
            TypeMetadata propMetadata,
            ILGenerator ilGenerator,
            bool readOnly)
        {
            // this._stub.
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, stubField);
            ilGenerator.Emit(OpCodes.Ldstr, dbKey);

            // this._stub.?Getter(dbKey).
            propMetadata.CallStubGetter(ilGenerator, readOnly);

            // return ?;
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate IL for the setter.
        /// </summary>
        /// <param name="stubField">The proxy stub field.</param>
        /// <param name="dbKey">The database key of this property.</param>
        /// <param name="propMetadata">The type metadata for this property.</param>
        /// <param name="ilGenerator">The setter's IL generator.</param>
        private static void GenerateSetterIL(
            FieldInfo stubField,
            string dbKey,
            TypeMetadata propMetadata,
            ILGenerator ilGenerator)
        {
            // this._stub.
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, stubField);
            ilGenerator.Emit(OpCodes.Ldstr, dbKey);

            // this._stub.?(value).
            ilGenerator.Emit(OpCodes.Ldarg_1);
            propMetadata.CallStubSetter(ilGenerator);

            // return ?;
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate IL for GetElemAt method.
        /// </summary>
        /// <param name="elemTypeMetadata">The element type metadata.</param>
        /// <param name="ilGenerator">The method's IL generator.</param>
        /// <param name="readOnly">True if the property is readonly.</param>
        private static void GenerateGetElemAtIL(
            TypeMetadata elemTypeMetadata,
            ILGenerator ilGenerator,
            bool readOnly)
        {
            // this._stub.
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, typeof(ListProxy<>).MakeGenericType(elemTypeMetadata.Type).GetField("stub", BindingFlags.NonPublic | BindingFlags.Instance));

            // this.ElemPrefix(index).
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Call, typeof(ListProxy<>).MakeGenericType(elemTypeMetadata.Type).GetMethod("GetElemDbKey", BindingFlags.NonPublic | BindingFlags.Instance));

            // this._stub.?Getter(elemPrefix).
            elemTypeMetadata.CallStubGetter(ilGenerator, readOnly);

            // return ?;
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate IL for SetElemAt method.
        /// </summary>
        /// <param name="elemTypeMetadata"></param>
        /// <param name="ilGenerator"></param>
        private static void GenerateSetElemAtIL(
            TypeMetadata elemTypeMetadata,
            ILGenerator ilGenerator)
        {
            // this._stub.
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, typeof(ListProxy<>).MakeGenericType(elemTypeMetadata.Type).GetField("stub", BindingFlags.NonPublic | BindingFlags.Instance));

            // this.ElemPrefix(index).
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Call, typeof(ListProxy<>).MakeGenericType(elemTypeMetadata.Type).GetMethod("GetElemDbKey", BindingFlags.NonPublic | BindingFlags.Instance));

            // this._stub.?Setter(elemPrefix, value).
            ilGenerator.Emit(OpCodes.Ldarg_2);
            elemTypeMetadata.CallStubSetter(ilGenerator);

            // return ?;
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate IL for the not available setter.
        /// </summary>
        /// <param name="ilGenerator">The setter's IL generator.</param>
        private static void GenerateNotAvailableSetterIL(
            ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Ldstr, "Current property is the entity's key and could not be override.");
            ilGenerator.Emit(OpCodes.Newobj, typeof(InvalidOperationException).GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Throw);
        }
    }
}
