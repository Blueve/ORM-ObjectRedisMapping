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
        /// The database client.
        /// </summary>
        private readonly IDatabaseClient dbClient;

        /// <summary>
        /// True if the proxy from the getter is readonly.
        /// </summary>
        private readonly bool Readonly;

        /// <summary>
        /// Initialize an instance of <see cref="DynamicProxyGenerator"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="entityKeyGenerator">The entity key generator.</param>
        /// <param name="dynamicProxyStub">The dynamic proxy stub.</param>
        /// <param name="dbClient">The database client.</param>
        /// <param name="isReadonly">True if the proxy from the getter is readonly.</param>
        public DynamicProxyGenerator(
            TypeRepository typeRepo,
            EntityKeyGenerator entityKeyGenerator,
            DynamicProxyStub dynamicProxyStub,
            IDatabaseClient dbClient,
            bool isReadonly = false)
        {
            this.typeRepo = typeRepo;
            this.entityKeyGenerator = entityKeyGenerator;
            this.dynamicProxyStub = dynamicProxyStub;
            this.dbClient = dbClient;
            this.Readonly = isReadonly;
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
            var typeMetadata = this.typeRepo.GetOrRegister(type) as EntityTypeMetadata;
            if (typeMetadata == null)
            {
                throw new ArgumentException("The given type must be an entity type.");
            }

            var proxyTypeBuilder = new ProxyTypeBuilder(type);

            var dbKey = this.entityKeyGenerator.GetDbKey(typeMetadata, entityKey);
            if (!this.dbClient.KeyExists(dbKey))
            {
                return default(T);
            }

            // Generate proxy for key property.
            var keyPropTypeMetadata = this.typeRepo.Get(typeMetadata.KeyProperty.PropertyType);
            proxyTypeBuilder.OverrideProperty(typeMetadata.KeyProperty, keyPropTypeMetadata, string.Concat(dbKey, typeMetadata.KeyProperty.Name), true);
            
            // Generate proxies for other properties.
            return this.GenerateForObjectInternal<T>(proxyTypeBuilder, typeMetadata, dbKey);
        }

        /// <summary>
        /// Generate a proxy for the given object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbPrefix">The prefix of database key.</param>
        /// <returns>The proxy.</returns>
        public T GenerateForObject<T>(string dbPrefix)
            where T : class
        {
            if (!this.dbClient.KeyExists(dbPrefix))
            {
                return null;
            }

            var type = typeof(T);
            var typeMetadata = this.typeRepo.GetOrRegister(type) as ObjectTypeMetadata;
            var proxyTypeBuilder = new ProxyTypeBuilder(type);

            return this.GenerateForObjectInternal<T>(proxyTypeBuilder, typeMetadata, dbPrefix);
        }

        /// <summary>
        /// Generate a proxy for the given object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="proxyTypeBuilder">The proxy type builder.</param>
        /// <param name="typeMetadata">The type metadata of object.</param>
        /// <param name="dbPrefix">The prefix of database key.</param>
        /// <returns>The proxy.</returns>
        private T GenerateForObjectInternal<T>(ProxyTypeBuilder proxyTypeBuilder, ObjectTypeMetadata typeMetadata, string dbPrefix)
            where T : class
        {
            // Generate proxy each property.
            foreach (var propInfo in typeMetadata.Properties)
            {
                var propDbKey = string.Concat(dbPrefix, propInfo.Name);
                var propTypeMetadata = this.typeRepo.Get(propInfo.PropertyType);
                proxyTypeBuilder.OverrideProperty(propInfo, propTypeMetadata, propDbKey, this.Readonly);
            }

            // Create the proxy type.
            var proxyType = proxyTypeBuilder.CreateType();

            // Create an instrance of the proxy, every object.
            return Activator.CreateInstance(proxyType, this.dynamicProxyStub) as T;
        }
    }
}
