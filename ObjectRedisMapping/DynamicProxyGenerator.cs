namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// The dynamic proxy generator.
    /// </summary>
    internal class DynamicProxyGenerator
    {
        private const string GetMethodPrefix = "get_";
        private const string SetMethodPrefix = "set_";
        private const string ProxyModuleName = "Proxy";
        private const string StubFieldName = "_stub";

        /// <summary>
        /// The type repository.
        /// </summary>
        private readonly TypeRepository typeRepo;

        /// <summary>
        /// The entity key generator.
        /// </summary>
        private readonly EntityKeyGenerator entityKeyGenerator;

        /// <summary>
        /// The dynamic proxy stub.
        /// </summary>
        private readonly DynamicProxyStub dynamicProxyStub;

        /// <summary>
        /// True if the proxy from the getter is readonly.
        /// </summary>
        private readonly bool isReadonly;

        /// <summary>
        /// Initialize an instance of <see cref="DynamicProxyGenerator"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="entityKeyGenerator">The entity key generator.</param>
        /// <param name="dynamicProxyStub">The dynamic proxy stub.</param>
        /// <param name="isReadonly">True if the proxy from the getter is readonly.</param>
        public DynamicProxyGenerator(
            TypeRepository typeRepo,
            EntityKeyGenerator entityKeyGenerator,
            DynamicProxyStub dynamicProxyStub,
            bool isReadonly = false)
        {
            this.typeRepo = typeRepo;
            this.entityKeyGenerator = entityKeyGenerator;
            this.dynamicProxyStub = dynamicProxyStub;
            this.isReadonly = isReadonly;
        }

        /// <summary>
        /// Generate a proxy for the given entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entityKey">The entity key.</param>
        /// <returns>The proxy.</returns>
        public T GenerateForEntity<T>(string entityKey)
            where T : class
        {
            var type = typeof(T);
            var typeMetadata = this.typeRepo.GetOrRegister(type);
            if (typeMetadata.ValueType != ObjectValueType.Entity)
            {
                throw new ArgumentException("The given type is not an Entity.");
            }

            var dbKey = this.entityKeyGenerator.GetDbKey(typeMetadata, entityKey);
            return this.GenerateForObject<T>(dbKey);
        }

        /// <summary>
        /// Generate a proxy for the given object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="entityKey">The prefix of database key.</param>
        /// <returns>The proxy.</returns>
        public T GenerateForObject<T>(string dbPrefix)
            where T : class
        {
            var type = typeof(T);
            var typeMetadata = this.typeRepo.GetOrRegister(type);

            var domain = AppDomain.CurrentDomain;
            var assembly = new AssemblyName(Guid.NewGuid().ToString());
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(ProxyModuleName);
            var typeBuilder = moduleBuilder.DefineType(type.FullName + ProxyModuleName, TypeAttributes.Public | TypeAttributes.Class, type);

            // Implement IProxy for all proxy type.
            typeBuilder.AddInterfaceImplementation(typeof(IProxy));

            // Prepare dependency fields.
            var stubFieldBuilder = typeBuilder.DefineField(StubFieldName, typeof(IDbAccessor), FieldAttributes.Private | FieldAttributes.InitOnly);

            // Build a constructor and inject IDbAccessor.
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(DynamicProxyStub) });
            var constructorILGenerator = constructorBuilder.GetILGenerator();
            GenerateConstructorIL(stubFieldBuilder, constructorILGenerator);

            // Generate stub for each virtual property.
            foreach (var propInfo in typeMetadata.Properties)
            {
                var propTypeMetadata = this.typeRepo.GetOrRegister(propInfo.PropertyType);
                var propertyBuilder = typeBuilder.DefineProperty(propInfo.Name, PropertyAttributes.None, propInfo.PropertyType, null);

                // Setup getter.
                var getterBuilder = typeBuilder.DefineMethod(
                    GetMethodPrefix + propInfo.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    propInfo.PropertyType,
                    Type.EmptyTypes);
                var getterILGenerator = getterBuilder.GetILGenerator();

                // Setup setter.
                var setterBuilder = typeBuilder.DefineMethod(
                    SetMethodPrefix + propInfo.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    null,
                    new[] { propInfo.PropertyType });
                var setterILGenerator = setterBuilder.GetILGenerator();

                var propDbKey = string.Concat(dbPrefix, propInfo.Name);
                var readonlyProp = propInfo == typeMetadata.KeyProperty || this.isReadonly;

                // Generate IL for getter.
                GenerateGetterIL(stubFieldBuilder, propDbKey, propTypeMetadata, getterILGenerator, readonlyProp);

                // Generate IL for setter.
                if (readonlyProp)
                {
                    GenerateNotAvailableSetterIL(stubFieldBuilder, setterILGenerator);
                }
                else
                {
                    GenerateSetterIL(stubFieldBuilder, propDbKey, propTypeMetadata, setterILGenerator);
                }

                // Binding to property.
                propertyBuilder.SetGetMethod(getterBuilder);
                propertyBuilder.SetSetMethod(setterBuilder);
            }

            // Create the proxy type.
            var proxyTypeInfo = typeBuilder.CreateTypeInfo();
            var proxyType = proxyTypeInfo.AsType();

            // Create an instrance of the proxy, every object.
            return Activator.CreateInstance(proxyType, this.dynamicProxyStub) as T;
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
        /// <param name="dbKey">The databse key of this property.</param>
        /// <param name="propMetadata">The type metadata for this property.</param>
        /// <param name="ilGenerator">The getter's IL generator.</param>
        /// <param name="isReadonly">True if the proxy from the getter is readonly.</param>
        private static void GenerateGetterIL(
            FieldInfo stubField,
            string dbKey,
            TypeMetadata propMetadata,
            ILGenerator ilGenerator,
            bool isReadonly = false)
        {
            // this._stub.
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, stubField);
            ilGenerator.Emit(OpCodes.Ldstr, dbKey);
            var stubMethodPrefix = DynamicProxyStub.GetStubMethodPrefix(propMetadata, isReadonly);
            var stubMethodName = string.Concat(stubMethodPrefix, "Getter");

            // this._stub.?Getter(dbKey).
            switch (propMetadata.ValueType)
            {
                case ObjectValueType.Primitive:
                case ObjectValueType.String:
                    ilGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub).GetMethod(stubMethodName, new[] { typeof(string) }));
                    break;

                case ObjectValueType.Entity:
                case ObjectValueType.Object:
                    ilGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub)
                            .GetMethods()
                            .First(m => m.Name.Equals(stubMethodName)).MakeGenericMethod(propMetadata.Type));
                    break;

                default:
                    throw new NotSupportedException();
            }

            // return ?;
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate IL for the setter.
        /// </summary>
        /// <param name="stubField">The proxy stub field.</param>
        /// <param name="dbKey">The databse key of this property.</param>
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
            var stubMethodPrefix = DynamicProxyStub.GetStubMethodPrefix(propMetadata);
            var stubMethodName = string.Concat(stubMethodPrefix, "Setter");

            // this._stub.?().
            switch (propMetadata.ValueType)
            {
                case ObjectValueType.Primitive:
                case ObjectValueType.String:
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub).GetMethod(stubMethodName, new[] { typeof(string), propMetadata.Type }));
                    break;

                case ObjectValueType.Entity:
                case ObjectValueType.Object:
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub)
                            .GetMethods()
                            .First(m => m.Name.Equals(stubMethodName)).MakeGenericMethod(propMetadata.Type));
                    break;

                default:
                    throw new NotSupportedException();
            }

            // return ?;
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Generate IL for the not available setter.
        /// </summary>
        /// <param name="stubField">The proxy stub field.</param>
        /// <param name="ilGenerator">The setter's IL generator.</param>
        private static void GenerateNotAvailableSetterIL(
            FieldInfo stubField,
            ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Ldstr, "Current property is the entity's key and could not be override.");
            ilGenerator.Emit(OpCodes.Newobj, typeof(InvalidOperationException).GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Throw);
        }
    }
}
