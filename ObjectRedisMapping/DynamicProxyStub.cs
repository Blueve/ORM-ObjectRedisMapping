namespace Blueve.ObjectRedisMapping
{
    using System;

    /// <summary>
    /// The stub of dynamic proxy.
    /// </summary>
    public class DynamicProxyStub
    {
        /// <summary>
        /// The type repository.
        /// </summary>
        private readonly TypeRepository typeRepo;

        /// <summary>
        /// The database accessor.
        /// </summary>
        private readonly IDbAccessor dbAccessor;

        /// <summary>
        /// The database record builder.
        /// </summary>
        private readonly IDbRecordBuilder dbRecordBuilder;

        /// <summary>
        /// The entity key generator.
        /// </summary>
        private readonly EntityKeyGenerator entityKeyGenerator;

        /// <summary>
        /// The database record submitter.
        /// </summary>
        private readonly DbRecordSubmitter dbRecorderSubmitter;

        /// <summary>
        /// Initialzie an instance of <see cref="DynamicProxyStub"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="dbAccessor">The database accessor.</param>
        /// <param name="dbRecordBuilder">The database record builder.</param>
        /// <param name="entityKeyGenerator">The entity key generator.</param>
        internal DynamicProxyStub(
            TypeRepository typeRepo,
            IDbAccessor dbAccessor,
            IDbRecordBuilder dbRecordBuilder,
            EntityKeyGenerator entityKeyGenerator,
            DbRecordSubmitter dbRecordSubmitter)
        {
            this.typeRepo = typeRepo;
            this.dbAccessor = dbAccessor;
            this.dbRecordBuilder = dbRecordBuilder;
            this.entityKeyGenerator = entityKeyGenerator;
            this.dbRecorderSubmitter = dbRecordSubmitter;
        }

        /// <summary>
        /// The getter for <see cref="string"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The value.</returns>
        public string StringGetter(string dbKey)
        {
            return this.dbAccessor.KeyExists(dbKey) ? this.dbAccessor.Get(dbKey) : default;
        }

        /// <summary>
        /// The setter for <see cref="string"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void StringSetter(string dbKey, string value)
        {
            this.dbAccessor.Set(dbKey, value);
        }

        /// <summary>
        /// The getter for <see cref="short"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The value.</returns>
        public short Int16Getter(string dbKey)
        {
            return this.dbAccessor.KeyExists(dbKey) ? Convert.ToInt16(this.dbAccessor.Get(dbKey)) : default;
        }

        /// <summary>
        /// The setter for <see cref="short"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void Int16Setter(string dbKey, short value)
        {
            this.dbAccessor.Set(dbKey, value.ToString());
        }

        /// <summary>
        /// The getter for <see cref="int"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The value.</returns>
        public int Int32Getter(string dbKey)
        {
            return this.dbAccessor.KeyExists(dbKey) ? Convert.ToInt32(this.dbAccessor.Get(dbKey)) : default;
        }

        /// <summary>
        /// The setter for <see cref="int"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void Int32Setter(string dbKey, int value)
        {
            this.dbAccessor.Set(dbKey, value.ToString());
        }

        /// <summary>
        /// The getter for <see cref="long"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The value.</returns>
        public long Int64Getter(string dbKey)
        {
            return this.dbAccessor.KeyExists(dbKey) ? Convert.ToInt64(this.dbAccessor.Get(dbKey)) : default;
        }

        /// <summary>
        /// The setter for <see cref="long"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void Int64Setter(string dbKey, long value)
        {
            this.dbAccessor.Set(dbKey, value.ToString());
        }

        /// <summary>
        /// The getter for entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The proxy of entity.</returns>
        public T EntityGetter<T>(string dbKey)
            where T : class
        {
            if (!this.dbAccessor.KeyExists(dbKey))
            {
                return default;
            }

            var entityKey = this.dbAccessor.Get(dbKey);
            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.entityKeyGenerator, this, this.dbAccessor);
            return proxyGenerator.GenerateForEntity<T>(entityKey);
        }

        /// <summary>
        /// The setter for entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The entity or proxy of entity.</param>
        public void EntitySetter<T>(string dbKey, T value)
            where T : class
        {
            var typeMetadata = this.typeRepo.GetOrRegister(typeof(T));

            if (!(value is IProxy))
            {
                // If value is not a proxy, then commit value as an entity and then update the DB reference.
                var records = this.dbRecordBuilder.Generate(value);
                this.dbRecorderSubmitter.Commit(records);
            }

            var entityKey = this.entityKeyGenerator.GetEntityKey(typeMetadata, value);
            this.dbAccessor.Set(dbKey, entityKey);
        }

        /// <summary>
        /// The getter for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The proxy of object.</returns>
        public T ObjectGetter<T>(string dbKey)
            where T : class
        {
            if (!this.dbAccessor.KeyExists(dbKey))
            {
                return default;
            }

            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.entityKeyGenerator, this, this.dbAccessor);
            return proxyGenerator.GenerateForObject<T>(dbKey);
        }

        /// <summary>
        /// The readonly getter for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The proxy of object which all property are readonly.</returns>
        public T ReadonlyObjectGetter<T>(string dbKey)
            where T : class
        {
            if (!this.dbAccessor.KeyExists(dbKey))
            {
                return default;
            }

            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.entityKeyGenerator, this, this.dbAccessor, true);
            return proxyGenerator.GenerateForObject<T>(dbKey);
        }

        /// <summary>
        /// The settre for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The object or proxy of object.</param>
        public void ObjectSetter<T>(string dbKey, T value)
        {
            var records = this.dbRecordBuilder.Generate(value, dbKey);
            this.dbRecorderSubmitter.Commit(records);
        }

        /// <summary>
        /// Gets the stub method's prefix by a type metadata.
        /// </summary>
        /// <param name="typeMetadata">The type metadata.</param>
        /// <param name="isReadonly">True if the proxy from the getter is readonly.</param>
        /// <returns>The stub method prefix.</returns>
        internal static string GetStubMethodPrefix(TypeMetadata typeMetadata, bool isReadonly = false)
        {
            switch (typeMetadata.ValueType)
            {
                case ObjectValueType.Primitive:
                case ObjectValueType.String:
                    return typeMetadata.Name;

                case ObjectValueType.Entity:
                case ObjectValueType.Struct:
                case ObjectValueType.Object when !isReadonly:
                    return typeMetadata.ValueType.ToString();

                case ObjectValueType.Object when isReadonly:
                    return string.Concat("Readonly", nameof(ObjectValueType.Object));

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
