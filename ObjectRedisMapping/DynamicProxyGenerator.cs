﻿namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Blueve.ObjectRedisMapping.Proxy;
    using StackExchange.Redis;

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
        /// The dynamic proxy stub.
        /// </summary>
        private readonly DynamicProxyStub dynamicProxyStub;

        /// <summary>
        /// The database client.
        /// </summary>
        private readonly IDatabase dbClient;

        /// <summary>
        /// True if the proxy from the getter is readonly.
        /// </summary>
        private readonly bool Readonly;

        /// <summary>
        /// Initialize an instance of <see cref="DynamicProxyGenerator"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="dynamicProxyStub">The dynamic proxy stub.</param>
        /// <param name="dbClient">The database client.</param>
        /// <param name="isReadonly">True if the proxy from the getter is readonly.</param>
        public DynamicProxyGenerator(
            TypeRepository typeRepo,
            DynamicProxyStub dynamicProxyStub,
            IDatabase dbClient,
            bool isReadonly = false)
        {
            this.typeRepo = typeRepo;
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
            var typeMetadata = this.typeRepo.GetOrRegister(type) as EntityMetadata;
            if (typeMetadata == null)
            {
                throw new ArgumentException("The given type must be an entity type.");
            }

            var proxyTypeBuilder = new ProxyTypeBuilder(type);

            var dbKey = EntityKeyGenerator.GetDbKey(typeMetadata, entityKey);
            if (!this.dbClient.KeyExists(dbKey))
            {
                return default(T);
            }

            // Generate proxy for key property.
            var keyPropTypeMetadata = this.typeRepo.Get(typeMetadata.KeyProperty.PropertyType);
            proxyTypeBuilder.InjectStub().CreateCtor().OverrideProperty(typeMetadata.KeyProperty, keyPropTypeMetadata, string.Concat(dbKey, typeMetadata.KeyProperty.Name), true);
            
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
                return default;
            }

            var type = typeof(T);
            var typeMetadata = this.typeRepo.GetOrRegister(type) as ObjectMetadata;
            var proxyTypeBuilder = new ProxyTypeBuilder(type).InjectStub().CreateCtor();

            return this.GenerateForObjectInternal<T>(proxyTypeBuilder, typeMetadata, dbPrefix);
        }

        /// <summary>
        /// Generate a proxy for the given list type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="dbPrefix">The prefix of database key.</param>
        /// <returns>The proxy.</returns>
        public IList<T> GenerateForList<T>(string dbPrefix)
        {
            if (!this.dbClient.KeyExists(dbPrefix))
            {
                return default;
            }

            var type = typeof(IList<T>);
            var typeMetadata = this.typeRepo.GetOrRegister(type) as ListMetadata;
            var proxyTypeBuilder = new ProxyTypeBuilder(typeof(ListProxy<T>))
                .OverrideElemMethods(this.typeRepo.GetOrRegister(typeMetadata.InnerType), this.Readonly)
                .InjectStub(typeof(ListProxy<T>).GetField("stub", BindingFlags.NonPublic | BindingFlags.Instance))
                .CreateCtor<T>();

            // Create the proxy type.
            var proxyType = proxyTypeBuilder.CreateType();

            // Create an instrance of the proxy.
            return Activator.CreateInstance(proxyType, dbPrefix, this.dynamicProxyStub) as IList<T>;
        }

        /// <summary>
        /// Generate a proxy for the given object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="proxyTypeBuilder">The proxy type builder.</param>
        /// <param name="typeMetadata">The type metadata of object.</param>
        /// <param name="dbPrefix">The prefix of database key.</param>
        /// <returns>The proxy.</returns>
        private T GenerateForObjectInternal<T>(ProxyTypeBuilder proxyTypeBuilder, ObjectMetadata typeMetadata, string dbPrefix)
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

            // Create an instrance of the proxy.
            return Activator.CreateInstance(proxyType, this.dynamicProxyStub) as T;
        }
    }
}
