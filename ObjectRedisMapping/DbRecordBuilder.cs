namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections;
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
        /// Initialize an instance of <see cref="DbRecordBuilder"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        public DbRecordBuilder(TypeRepository typeRepo)
        {
            this.typeRepo = typeRepo;
        }

        /// <inheritdoc/>
        public IEnumerable<IDbOperation> Generate<T>(T obj, string prefix = "")
        {
            var type = typeof(T);
            var typeMetadata = this.typeRepo.GetOrRegister(type);
            return typeMetadata.GenerateDbRecords<T>(this, prefix, obj);
        }

        /// <inheritdoc/>
        public IEnumerable<IDbOperation> GenerateStringRecord<T>(string prefix, T obj)
        {
            return Enumerable.Empty<IDbOperation>().Append(new DbStringRecord(prefix, obj.ToString()));
        }

        /// <inheritdoc/>
        public IEnumerable<IDbOperation> GenerateObjectRecord(string prefix, object obj, TypeMetadata typeMetadata, bool generateRefForProxy = true)
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
                        var entityKey = EntityKeyGenerator.GetDbKey(entityType, curValue);
                        var entityKeyValue = EntityKeyGenerator.GetEntityKey(entityType, curValue);

                        // For added entity, just record the reference.
                        if (curName != null)
                        {
                            yield return new DbStringRecord(curPrefix, entityKeyValue);
                        }
                        
                        if (!visitedEntities.Contains(entityKey) && !(curValue is IProxy && generateRefForProxy))
                        {
                            // For new entity, add it to records.
                            visitedEntities.Add(entityKey);
                            yield return new DbStringRecord(entityKey, bool.TrueString);
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

                        yield return new DbStringRecord(curPrefix, bool.TrueString);
                        ExpandProperties(states, curPrefix, curValue, objType.Properties, depth + 1);
                        break;

                    case ListMetadata listType:
                        var list = curValue as IList;
                        yield return new DbStringRecord(curPrefix, list.Count.ToString());
                        for (int i = 0; i < list.Count; i++)
                        {
                            states.Push((listType.InnerType, i.ToString(), list[i], curPrefix, depth + 1));
                        }

                        break;

                    case TypeMetadata basicType when basicType.ValueType == ObjectValueType.Primitive || basicType.ValueType == ObjectValueType.String:
                        yield return new DbStringRecord(curPrefix, curValue.ToString());
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
