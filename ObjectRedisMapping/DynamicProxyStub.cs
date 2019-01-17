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
        /// The database record submitter.
        /// </summary>
        private readonly DbRecordSubmitter dbRecorderSubmitter;

        /// <summary>
        /// Initialzie an instance of <see cref="DynamicProxyStub"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="dbAccessor">The databse accessor.</param>
        /// <param name="dbRecordBuilder">The databse record builder.</param>
        /// <param name="entityKeyGenerator">The entity key generator.</param>
        internal DynamicProxyStub(
            ITypeRepository typeRepo,
            IDbAccessor dbAccessor,
            IDbRecordBuilder dbRecordBuilder,
            EntityKeyGenerator entityKeyGenerator)
        {
            this.typeRepo = typeRepo;
            this.dbAccessor = dbAccessor;
            this.dbRecordBuilder = dbRecordBuilder;
            this.entityKeyGenerator = entityKeyGenerator;

            // TODO: This submitter can be inject, but I put it here util factory done.
            this.dbRecorderSubmitter = new DbRecordSubmitter(this.dbAccessor);
        }

        /// <summary>
        /// The getter for <see cref="string"/> type.
        /// </summary>
        /// <param name="dbKey">The databse key.</param>
        /// <returns>The value.</returns>
        public string StringGetter(string dbKey)
        {
            return this.dbAccessor.Get(dbKey);
        }

        /// <summary>
        /// The setter for <see cref="string"/> type.
        /// </summary>
        /// <param name="dbKey">The databse key.</param>
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
            return Convert.ToInt16(this.dbAccessor.Get(dbKey));
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
            return Convert.ToInt32(this.dbAccessor.Get(dbKey));
        }

        /// <summary>
        /// The setter for <see cref="int"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void Int32Setter(string dbKey, short value)
        {
            this.dbAccessor.Set(dbKey, value.ToString());
        }

        /// <summary>
        /// The getter for <see cref="long"/> type.
        /// </summary>
        /// <param name="dbKey">The databse key.</param>
        /// <returns>The value.</returns>
        public long Int64Getter(string dbKey)
        {
            return Convert.ToInt64(this.dbAccessor.Get(dbKey));
        }

        /// <summary>
        /// The setter for <see cref="long"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void Int64Setter(string dbKey, short value)
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
            var entityKey = this.dbAccessor.Get(dbKey);
            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.dbAccessor, this.dbRecordBuilder, this.entityKeyGenerator, dbKey);
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
            // TODO: If value is not a proxy, then commit value as an entity and then update the DB reference.
            var typeMetadata = this.typeRepo.GetOrAdd(typeof(T));
            var entityKey = this.entityKeyGenerator.GetEntityKey(typeMetadata, value);
            this.dbAccessor.Set(dbKey, entityKey);
        }

        /// <summary>
        /// The getter for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The databse key.</param>
        /// <returns>The proxy of object.</returns>
        public T ObjectGetter<T>(string dbKey)
            where T : class
        {
            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.dbAccessor, this.dbRecordBuilder, this.entityKeyGenerator, dbKey);
            return proxyGenerator.GenerateForObject<T>(dbKey);
        }

        /// <summary>
        /// The readonly getter for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The databse key.</param>
        /// <returns>The proxy of object which all property are readonly.</returns>
        public T ReadonlyObjectGetter<T>(string dbKey)
            where T : class
        {
            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.dbAccessor, this.dbRecordBuilder, this.entityKeyGenerator, dbKey, true);
            return proxyGenerator.GenerateForObject<T>(dbKey);
        }

        /// <summary>
        /// The settre for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The databse key.</param>
        /// <param name="value">The object or proxy of object.</param>
        public void ObjectSetter<T>(string dbKey, T value)
        {
            var records = this.dbRecordBuilder.Generate(value, dbKey);
            this.dbRecorderSubmitter.Commit(records);
        }
    }
}
