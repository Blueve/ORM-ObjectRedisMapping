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
            return typeMetadata.GenerateDbRecords<T>(this, prefix, obj);
        }

        /// <inheritdoc/>
        public IEnumerable<DbRecord> GenerateStringRecord<T>(string prefix, T obj)
        {
            return Enumerable.Empty<DbRecord>().Append(DbRecord.GenerateStringRecord(prefix, obj.ToString()));
        }

        /// <inheritdoc/>
        public IEnumerable<DbRecord> GenerateObjectRecord(string prefix, object obj, ObjectMetadata typeMetadata)
        {
            // Traverse the property tree by using DFS.
            // This implemention is for better performance in case some object have too much layers.
            var states = new Stack<(Type type, string name, object val, string prefix, uint depth)>();
            var path = new Dictionary<uint, object>();

            // Initialize the searching with the properties of the object.
            states.Push((typeMetadata.Type, null, obj, prefix, 0u));

            // Searching and build the database record.
            var visitedEntities = new HashSet<string>();
            while (states.Count != 0)
            {
                var (curType, curName, curValue, curPrefix, depth) = states.Pop();
                var curTypeMetadata = this.typeRepo.GetOrRegister(curType);
                path[depth] = curValue;

                // Process current property.
                curPrefix += curName;
                switch (curTypeMetadata)
                {
                    case EntityMetadata entityType:
                        var entityKey = this.entityKeyGenerator.GetDbKey(entityType, curValue);
                        var entityKeyValue = this.entityKeyGenerator.GetEntityKey(entityType, curValue);

                        // For added entity, just record the reference.
                        if (curName != null)
                        {
                            yield return new DbRecord(curPrefix, new DbValue(DbValueType.String, entityKeyValue));
                        }
                        
                        if (!visitedEntities.Contains(entityKey) && !(curValue is IProxy))
                        {
                            // For new entity, add it to records.
                            visitedEntities.Add(entityKey);
                            yield return new DbRecord(entityKey, new DbValue(DbValueType.String, bool.TrueString));
                            ExpandProperties(
                                states,
                                entityKey,
                                curValue,
                                entityType.Properties.Append(entityType.KeyProperty),
                                depth + 1);
                        }

                        break;

                    case ObjectMetadata objType:
                        for (var i = 0u; i < depth; i++)
                        {
                            if (path[i] == curValue)
                            {
                                throw new NotSupportedException("Circular reference is not support Object type, consider use Entity instead.");
                            }
                        }

                        yield return new DbRecord(curPrefix, new DbValue(DbValueType.String, bool.TrueString));
                        ExpandProperties(states, curPrefix, curValue, objType.Properties, depth + 1);
                        break;

                    case TypeMetadata basicType when basicType.ValueType == ObjectValueType.Primitive || basicType.ValueType == ObjectValueType.String:
                        yield return DbRecord.GenerateStringRecord(curPrefix, curValue.ToString());
                        break;

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
        /// <param name="depth">The depth of current path.</param>
        /// <param name="path">Current path.</param>
        private static void ExpandProperties(
            Stack<(Type type, string name, object val, string prefix, uint depth)> states,
            string prefix,
            object obj,
            IEnumerable<PropertyInfo> properties,
            uint depth)
        {
            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj);
                if (value == null)
                {
                    continue;
                }

                states.Push((prop.PropertyType, prop.Name, value, prefix, depth));
            }
        }
    }
}
