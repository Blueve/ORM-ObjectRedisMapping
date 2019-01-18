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
                    return this.Generate(typeMetadata, obj, prefix);

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
        /// <param name="obj">The object.</param>
        /// <param name="prefix">The prefix, default is an empty string.</param>
        /// <returns></returns>
        private IEnumerable<DbRecord> Generate(TypeMetadata typeMetadata, object obj, string prefix = "")
        {
            // Traverse the property tree by using DFS.
            var states = new Stack<(PropertyInfo prop, object val, string prefix)>();

            // Initialize the searching with the properties of the object.
            var basePrefix = typeMetadata.ValueType == ObjectValueType.Entity
                ? this.entityKeyGenerator.GetDbKey(typeMetadata, obj)
                : prefix;

            foreach (var prop in typeMetadata.Properties)
            {
                var value = prop.GetValue(obj);
                if (value == null)
                {
                    continue;
                }

                states.Push((prop, value, basePrefix));
            }

            // Searching and build the database record.
            var visitedObjs = new Dictionary<object, string>();
            while (states.Count != 0)
            {
                var (curProp, curValue, curPrefix) = states.Pop();

                var propType = curProp.PropertyType;
                var propTypeMetadata = this.typeRepo.GetOrRegister(propType);

                // Process current property.
                curPrefix += curProp.Name;
                switch (propTypeMetadata.ValueType)
                {
                    case ObjectValueType.Primitive:
                    case ObjectValueType.String:
                        yield return DbRecord.GenerateStringRecord(curPrefix, curValue.ToString());
                        break;

                    case ObjectValueType.Entity:
                        var entityKey = this.entityKeyGenerator.GetDbKey(propTypeMetadata, curValue);
                        yield return new DbRecord(curPrefix, new DbValue(DbValueType.String, entityKey));
                        break;

                    case ObjectValueType.Object:
                        if (visitedObjs.TryGetValue(curValue, out var objDbKey))
                        {
                            // The object has appeared before, record its Guid as the database record.
                            yield return new DbRecord(curPrefix, new DbValue(DbValueType.String, objDbKey));
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
                    case ObjectValueType.Struct:
                    case ObjectValueType.List:
                    case ObjectValueType.Set:
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
