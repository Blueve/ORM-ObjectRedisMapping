namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// The dynamic proxy generator.
    /// </summary>
    internal class DynamicProxyGenerator : IProxyGenerator
    {
        /// <summary>
        /// The type repository.
        /// </summary>
        private readonly ITypeRepository typeRepo;

        /// <summary>
        /// The databse accessor.
        /// </summary>
        private readonly IDbAccessor dbAccessor;

        /// <summary>
        /// The databse record builder.
        /// </summary>
        private readonly IDbRecordBuilder dbRecordBuilder;

        /// <summary>
        /// The entity key generator.
        /// </summary>
        private readonly EntityKeyGenerator entityKeyGenerator;

        /// <summary>
        /// The current database key prefix.
        /// </summary>
        private readonly string prefix;

        /// <summary>
        /// Initialize an instance of <see cref="DynamicProxyGenerator"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="dbAccessor">The databse accessor.</param>
        /// <param name="dbRecordBuilder">The databse record builder.</param>
        /// <param name="entityKeyGenerator">The entity key generator.</param>
        /// <param name="prefix">The current database key prefix.</param>
        public DynamicProxyGenerator(
            ITypeRepository typeRepo,
            IDbAccessor dbAccessor,
            IDbRecordBuilder dbRecordBuilder,
            EntityKeyGenerator entityKeyGenerator,
            string prefix = "")
        {
            this.typeRepo = typeRepo;
            this.dbAccessor = dbAccessor;
            this.dbRecordBuilder = dbRecordBuilder;
            this.entityKeyGenerator = entityKeyGenerator;
            this.prefix = prefix;
        }

        /// <inheritdoc/>
        public T GenerateForEntity<T>(string entityKey)
            where T : class
        {
            var type = typeof(T);
            var typeMetadata = this.typeRepo.GetOrAdd(type);
            if (typeMetadata.ValueType != ObjectValueType.Entity)
            {
                throw new ArgumentException("The given type is not an Entity.");
            }

            var dbKey = this.entityKeyGenerator.GetDbKey(typeMetadata, entityKey);
            return this.GenerateForObject<T>(dbKey);
        }

        /// <inheritdoc/>
        public T GenerateForObject<T>(string dbPrefix)
            where T : class
        {
            var type = typeof(T);
            var typeMetadata = this.typeRepo.GetOrAdd(type);

            var domain = AppDomain.CurrentDomain;
            var assembly = new AssemblyName(Guid.NewGuid().ToString());
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("Proxy");
            var typeBuilder = moduleBuilder.DefineType(type.FullName + "Proxy", TypeAttributes.Public | TypeAttributes.Class, type);

            // Prepare dependency fields.
            var stubFieldBuilder = typeBuilder.DefineField("_stub", typeof(IDbAccessor), FieldAttributes.Private | FieldAttributes.InitOnly);

            // Build a constructor and inject IDbAccessor.
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { typeof(DynamicProxyStub) });
            var constructorILGenerator = constructorBuilder.GetILGenerator();
            this.GenerateConstructorIL(stubFieldBuilder, constructorILGenerator);

            // Generate stub for each virtual property.
            foreach (var propInfo in typeMetadata.Properties)
            {
                var propTypeMetadata = this.typeRepo.GetOrAdd(propInfo.PropertyType);
                var propertyBuilder = typeBuilder.DefineProperty(propInfo.Name, PropertyAttributes.None, propInfo.PropertyType, null);

                // Setup getter.
                var getterBuilder = typeBuilder.DefineMethod(
                    "get_" + propInfo.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    propInfo.PropertyType,
                    Type.EmptyTypes);
                var getterILGenerator = getterBuilder.GetILGenerator();

                // Setup setter.
                var setterBuilder = typeBuilder.DefineMethod(
                    "set_" + propInfo.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    null,
                    new[] { propInfo.PropertyType });
                var setterILGenerator = setterBuilder.GetILGenerator();

                // Generate IL.
                // TODO: Generate a special setter for entity key property to avoid change entity key.
                var propDbKey = $"{dbPrefix}{propInfo.Name}";
                this.GeneratePropertyIL(
                            stubFieldBuilder,
                            propDbKey,
                            propTypeMetadata,
                            getterILGenerator, setterILGenerator);

                // Binding to property.
                propertyBuilder.SetGetMethod(getterBuilder);
                propertyBuilder.SetSetMethod(setterBuilder);
            }

            // Create the proxy type.
            var proxyTypeInfo = typeBuilder.CreateTypeInfo();
            var proxyType = proxyTypeInfo.AsType();

            // Create an instrance of the proxy, every object.
            var stub = new DynamicProxyStub(this.typeRepo, this.dbAccessor, this.dbRecordBuilder, new EntityKeyGenerator(new EntityKeyValueFormatter()));
            return Activator.CreateInstance(proxyType, stub) as T;
        }

        /// <summary>
        /// Generate IL for the constructor.
        /// </summary>
        /// <param name="stubField">The proxy stub field.</param>
        /// <param name="ctorILGenerator">The constructor's IL generator.</param>
        private void GenerateConstructorIL(
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
        /// Generate IL for the property.
        /// </summary>
        /// <param name="stubField">The proxy stub field.</param>
        /// <param name="dbKey">The databse key of this property.</param>
        /// <param name="propMetadata">The type metadata for this property.</param>
        /// <param name="getterILGenerator">The getter's IL generator.</param>
        /// <param name="setterILGenerator">The setter's IL generator.</param>
        private void GeneratePropertyIL(
            FieldInfo stubField,
            string dbKey,
            TypeMetadata propMetadata,
            ILGenerator getterILGenerator,
            ILGenerator setterILGenerator)
        {
            // this._stub.
            getterILGenerator.Emit(OpCodes.Ldarg_0);
            getterILGenerator.Emit(OpCodes.Ldfld, stubField);
            setterILGenerator.Emit(OpCodes.Ldarg_0);
            setterILGenerator.Emit(OpCodes.Ldfld, stubField);

            // this._stub.?().
            switch (propMetadata.ValueType)
            {
                case ObjectValueType.Primitive:
                case ObjectValueType.String:
                    getterILGenerator.Emit(OpCodes.Ldstr, dbKey);
                    getterILGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub).GetMethod(propMetadata.Name + "Getter", new[] { typeof(string) }));

                    setterILGenerator.Emit(OpCodes.Ldstr, dbKey);
                    setterILGenerator.Emit(OpCodes.Ldarg_1);
                    setterILGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub).GetMethod(propMetadata.Name + "Setter", new[] { typeof(string), propMetadata.Type }));
                    break;

                case ObjectValueType.Entity:
                    getterILGenerator.Emit(OpCodes.Ldstr, dbKey);
                    getterILGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub)
                            .GetMethods()
                            .First(m => m.Name.Equals("EntityGetter")).MakeGenericMethod(propMetadata.Type));

                    setterILGenerator.Emit(OpCodes.Ldstr, dbKey);
                    setterILGenerator.Emit(OpCodes.Ldarg_1);
                    setterILGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub)
                            .GetMethods()
                            .First(m => m.Name.Equals("EntitySetter")).MakeGenericMethod(propMetadata.Type));
                    break;

                case ObjectValueType.Object:
                    getterILGenerator.Emit(OpCodes.Ldstr, dbKey);
                    getterILGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub)
                            .GetMethods()
                            .First(m => m.Name.Equals("ObjectGetter")).MakeGenericMethod(propMetadata.Type));

                    setterILGenerator.Emit(OpCodes.Ldstr, dbKey);
                    setterILGenerator.Emit(OpCodes.Ldarg_1);
                    setterILGenerator.Emit(
                        OpCodes.Call,
                        typeof(DynamicProxyStub)
                            .GetMethods()
                            .First(m => m.Name.Equals("ObjectSetter")).MakeGenericMethod(propMetadata.Type));
                    break;

                default:
                    throw new NotSupportedException();
            }

            // return ?;
            getterILGenerator.Emit(OpCodes.Ret);
            setterILGenerator.Emit(OpCodes.Ret);
        }
    }
}
