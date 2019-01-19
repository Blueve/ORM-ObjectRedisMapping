namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The database record builder.
    /// </summary>
    internal class DbRecordBuilder : IDbRecordBuilder
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
        /// Initialize an instance of <see cref="DbRecordBuilder"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="entityKeyGenerator">The entity key generator.</param>
        public DbRecordBuilder(
            TypeRepository typeRepo,
            EntityKeyGenerator entityKeyGenerator)
        {
            this.typeRepo = typeRepo;
            this.entityKeyGenerator = entityKeyGenerator;
        }

        /// <inheritdoc/>
        public IEnumerable<DbRecord> Generate<T>(T obj, string prefix = "")
        {
            var type = typeof(T);
            var typeMetadata = this.typeRepo.GetOrRegister(type);

            switch (typeMetadata.ValueType)
            {
                // Deal with all non-reference types here to avoid boxing.
                // TODO: Refactor primitive and string type to avoid generate an itor for a single record.
                case ObjectValueType.Primitive:
                case ObjectValueType.String:
                    // TODO: We can convert primitive to binary format to avoid information missing.
                    return Enumerable.Empty<DbRecord>().Append(DbRecord.GenerateStringRecord(prefix, obj.ToString()));

                // Deal with reference types.
                case ObjectValueType.Entity:
                case ObjectValueType.Object:
                    return this.Generate(typeMetadata, null, obj, prefix);

                case ObjectValueType.Struct:
                case ObjectValueType.List:
                case ObjectValueType.Set:
                    throw new NotImplementedException();

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Generate database records for the given object.
        /// </summary>
        /// <param name="typeMetadata">The type metadata of the object.</param>
        /// <param name="name">The name of current object's property, null if the object is not belong to an property.</param>
        /// <param name="obj">The object.</param>
        /// <param name="prefix">The prefix, default is an empty string.</param>
        /// <returns></returns>
        private IEnumerable<DbRecord> Generate(TypeMetadata typeMetadata, string name, object obj, string prefix = "")
        {
            // Traverse the property tree by using DFS.
            var states = new Stack<(Type type, string name, object val, string prefix)>();

            // Initialize the searching with the properties of the object.
            var basePrefix = typeMetadata.ValueType == ObjectValueType.Entity
                ? this.entityKeyGenerator.GetDbKey(typeMetadata, obj)
                : prefix;

            states.Push((typeMetadata.Type, name, obj, prefix));

            // Searching and build the database record.
            var visitedObjs = new HashSet<object>();
            var visitedEntities = new HashSet<string>();
            while (states.Count != 0)
            {
                var (curType, curName, curValue, curPrefix) = states.Pop();
                var curTypeMetadata = this.typeRepo.GetOrRegister(curType);

                // Process current property.
                curPrefix += curName;
                switch (curTypeMetadata.ValueType)
                {
                    case ObjectValueType.Primitive:
                    case ObjectValueType.String:
                        yield return DbRecord.GenerateStringRecord(curPrefix, curValue.ToString());
                        break;

                    case ObjectValueType.Entity:
                        // TODO: If entity is an proxy, stop to generate record for it, just create reference.
                        var entityKey = this.entityKeyGenerator.GetDbKey(curTypeMetadata, curValue);
                        var entityKeyValue = this.entityKeyGenerator.GetEntityKey(curTypeMetadata, curValue);
                        if (!visitedEntities.Contains(entityKey))
                        {
                            // For new entity, add it to records.
                            visitedEntities.Add(entityKey);
                            yield return new DbRecord(entityKey, new DbValue(DbValueType.String, true.ToString()));
                            ExpandProperties(states, entityKey, curValue, curTypeMetadata.Properties);
                        }

                        // For added entity, just record the reference.
                        if (curName != null)
                        {
                            yield return new DbRecord(curPrefix, new DbValue(DbValueType.String, entityKeyValue));
                        }

                        break;

                    case ObjectValueType.Object:
                        // TODO: Use path to find circular reference.
                        if (visitedObjs.Contains(curValue))
                        {
                            throw new NotSupportedException("Circular reference is not support Object type, consider use Entity instead.");
                        }

                        visitedObjs.Add(curValue);
                        ExpandProperties(states, curPrefix, curValue, curTypeMetadata.Properties);
                        break;

                    case ObjectValueType.Struct:
                    case ObjectValueType.List:
                    case ObjectValueType.Set:
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Expand an object's properties to states.
        /// </summary>
        /// <param name="states">The states.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="obj">The object.</param>
        /// <param name="properties">The object's properties.</param>
        private static void ExpandProperties(
            Stack<(Type type, string name, object val, string prefix)> states,
            string prefix,
            object obj,
            PropertyInfo[] properties)
        {
            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj);
                if (value == null)
                {
                    continue;
                }

                states.Push((prop.PropertyType, prop.Name, value, prefix));
            }
        }
    }
}
