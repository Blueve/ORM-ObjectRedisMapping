namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

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

        private readonly FieldBuilder stubFieldBuilder;
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

            // Prepare dependency fields.
            this.stubFieldBuilder = this.typeBuilder.DefineField(
                StubFieldName, typeof(IDatabaseClient), FieldAttributes.Private | FieldAttributes.InitOnly);

            // Build a constructor and inject IDatabaseClient.
            var constructorBuilder = this.typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(DynamicProxyStub) });
            var constructorILGenerator = constructorBuilder.GetILGenerator();
            GenerateConstructorIL(this.stubFieldBuilder, constructorILGenerator);
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
            GenerateGetterIL(this.stubFieldBuilder, propDbKey, propTypeMetadata, getterILGenerator, readOnly);

            // Generate IL for setter.
            if (readOnly)
            {
                GenerateNotAvailableSetterIL(setterILGenerator);
            }
            else
            {
                GenerateSetterIL(this.stubFieldBuilder, propDbKey, propTypeMetadata, setterILGenerator);
            }

            // Binding to property.
            propertyBuilder.SetGetMethod(getterBuilder);
            propertyBuilder.SetSetMethod(setterBuilder);

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
            bool readOnly = false)
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

            // this._stub.?().
            ilGenerator.Emit(OpCodes.Ldarg_1);
            propMetadata.CallStubSetter(ilGenerator);

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
