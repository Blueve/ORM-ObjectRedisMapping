namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The database record builder for entity.
    /// </summary>
    internal class EntityDbRecordBuilder : IEntityDbRecordBuilder
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
        /// The database key prefix.
        /// </summary>
        private readonly string prefix;

        /// <summary>
        /// Initialize an instance of <see cref="EntityDbRecordBuilder"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="entityKeyGenerator">The entity key generator.</param>
        /// <param name="prefix">The prefix, default is an empty string.</param>
        public EntityDbRecordBuilder(
            TypeRepository typeRepo,
            EntityKeyGenerator entityKeyGenerator,
            string prefix = "")
        {
            this.typeRepo = typeRepo;
            this.entityKeyGenerator = entityKeyGenerator;
            this.prefix = prefix;
        }

        /// <inheritdoc/>
        public IEnumerable<DbRecord> Generate<T>(T entity)
        {
            var type = typeof(T);
            return this.Generate(type, entity);
        }

        /// <summary>
        /// Generate database records for the given object.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private IEnumerable<DbRecord> Generate(Type type, object obj)
        {
            // Traverse the property tree by using DFS.
            var states = new Stack<(PropertyInfo prop, object val, string dbKey)>();

            // Initialize the searching with the properties of the object.
            // TODO: If the object is an entity, build the record from entity key.
            //       If the object is an object, build the record from the builder's prefix.
            var typeMetadata = this.typeRepo.GetOrAdd(type);
            var entityPrefix = this.entityKeyGenerator.GetDbKey(typeMetadata, obj);
            foreach (var prop in typeMetadata.Properties)
            {
                var value = prop.GetValue(obj);
                if (value == null)
                {
                    continue;
                }

                states.Push((prop, value, entityPrefix));
            }

            // Searching and build the database record.
            var visitedObjs = new Dictionary<object, string>();
            while (states.Count != 0)
            {
                var (curProp, curValue, curPrefix) = states.Pop();

                var propType = curProp.PropertyType;
                var propTypeMetadata = this.typeRepo.GetOrAdd(propType);

                // Process current property.
                curPrefix += $"{curProp.Name}";
                DbValue dbValue;
                switch (propTypeMetadata.ValueType)
                {
                    case ObjectValueType.Primitive:
                    case ObjectValueType.String:
                        dbValue = new DbValue
                        {
                            Type = DbValueType.String,
                            Object = curValue.ToString()
                        };
                        yield return new DbRecord(curPrefix, dbValue);
                        break;

                    case ObjectValueType.Entity:
                        dbValue = new DbValue
                        {
                            Type = DbValueType.String,
                            Object = this.entityKeyGenerator.GetDbKey(propTypeMetadata, curValue)
                        };
                        yield return new DbRecord(curPrefix, dbValue);
                        break;

                    case ObjectValueType.Object:
                        if (visitedObjs.TryGetValue(curValue, out var objDbKey))
                        {
                            // The object has appeared before, record its Guid as the database record.
                            dbValue = new DbValue
                            {
                                Type = DbValueType.String,
                                Object = objDbKey
                            };
                            yield return new DbRecord(curPrefix, dbValue);
                            break;
                        }

                        visitedObjs.Add(curValue, curPrefix);
                        foreach (var prop in propTypeMetadata.Properties)
                        {
                            var value = prop.GetValue(curValue);
                            if (value == null)
                            {
                                continue;
                            }

                            states.Push((prop, value, curPrefix));
                        }

                        break;

                    case ObjectValueType.List:
                    case ObjectValueType.Set:
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
