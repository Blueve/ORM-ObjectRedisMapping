namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The type repository.
    /// </summary>
    internal class TypeRepository : ITypeRepository
    {
        /// <summary>
        /// The embedded types list.
        /// The embedded type is the entity or object's container. 
        /// </summary>
        private static readonly ISet<Type> EmbeddedTypes = new HashSet<Type>
        {
            typeof(List<>),
            typeof(Dictionary<,>),
            typeof(HashSet<>)
        };

        /// <summary>
        /// The type metadata dictionary.
        /// </summary>
        private readonly IDictionary<Type, TypeMetadata> typeMetadataDict = new Dictionary<Type, TypeMetadata>();

        /// <summary>
        /// The type metadata generator.
        /// </summary>
        private readonly TypeMetadataGenerator typeMetadataGenerator;

        /// <summary>
        /// Initialize an instance of <see cref="TypeRepository"/>.
        /// </summary>
        /// <param name="typeMetadataGenerator">The type metadata generator.</param>
        public TypeRepository(TypeMetadataGenerator typeMetadataGenerator)
        {
            this.typeMetadataGenerator = typeMetadataGenerator;
        }

        /// <inheritdoc/>
        public void Register<T>()
        {
            var type = typeof(T);
            this.Register(type);
        }

        /// <inheritdoc/>
        public void Register(Type type)
        {
            if (this.typeMetadataDict.ContainsKey(type))
            {
                return;
            }

            if (type.IsArray)
            {
                throw new InvalidOperationException(
                    $"The type [{type.FullName}] is array.");
            }

            if (type.IsGenericType && EmbeddedTypes.Contains(type.GetGenericTypeDefinition()))
            {
                throw new InvalidOperationException(
                    $"The type [{type.FullName}] is embedded type.");
            }

            // Generate the metadata and add it to container.
            var typeMetadata = this.typeMetadataGenerator.Generate(type);
            this.typeMetadataDict.Add(type, typeMetadata);

            // Try analyze the type and resolve internal type if possible.
            switch (typeMetadata.ValueType)
            {
                case ObjectValueType.Primitive:
                    break;

                case ObjectValueType.Entity:
                case ObjectValueType.Object:
                    foreach (var prop in typeMetadata.Properties)
                    {
                        this.Register(prop.PropertyType);
                    }

                    break;

                case ObjectValueType.List:
                    break;

                case ObjectValueType.Set:
                    break;
            }
        }

        /// <inheritdoc/>
        public TypeMetadata Get<T>()
        {
            var type = typeof(T);
            return this.Get(type);
        }

        /// <inheritdoc/>
        public TypeMetadata Get(Type type)
        {
            if (this.typeMetadataDict.TryGetValue(type, out var typeMetdata))
            {
                return typeMetdata;
            }

            throw new KeyNotFoundException($"The type {type.FullName} hasn't registered.");
        }

        /// <inheritdoc/>
        public bool TryGet(Type type, out TypeMetadata typeMetadata)
        {
            return this.typeMetadataDict.TryGetValue(type, out typeMetadata);
        }

        /// <inheritdoc/>
        public TypeMetadata GetOrAdd(Type type)
        {
            if (this.typeMetadataDict.TryGetValue(type, out var typeMetadata))
            {
                return typeMetadata;
            }

            this.Register(type);
            return this.Get(type);
        }

        /// <summary>
        /// Create an instance of <see cref="TypeRepository"/>.
        /// </summary>
        /// <returns>The instance of <see cref="TypeRepository"/>.</returns>
        public static TypeRepository CreateInstance()
        {
            return new TypeRepository(new TypeMetadataGenerator());
        }
    }
}
