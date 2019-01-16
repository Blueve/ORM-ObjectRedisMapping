namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

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
                case ObjectValueType.String:
                    break;

                case ObjectValueType.Entity:
                    this.RegisterKeyProperty(typeMetadata.KeyProperty);
                    foreach (var prop in typeMetadata.Properties)
                    {
                        this.Register(prop.PropertyType);
                    }

                    break;

                case ObjectValueType.Object:
                    foreach (var prop in typeMetadata.Properties)
                    {
                        this.Register(prop.PropertyType);
                    }

                    break;

                case ObjectValueType.List:
                case ObjectValueType.Set:
                    throw new NotImplementedException();

                default:
                    throw new NotSupportedException();
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
        /// Register entity key property.
        /// </summary>
        /// <param name="keyProp">The key property.</param>
        /// <exception cref="ArgumentException">The key property type is entity or include entity.</exception>
        /// <exception cref="NotSupportedException">The key property type is not supported or include not supported type.</exception>
        private void RegisterKeyProperty(PropertyInfo keyProp)
        {
            var keyPropMetadata = this.GetOrAdd(keyProp.PropertyType);
            
            switch (keyPropMetadata.ValueType)
            {
                case ObjectValueType.Primitive:
                case ObjectValueType.String:
                case ObjectValueType.Struct:
                    break;

                case ObjectValueType.Entity:
                    throw new ArgumentException("Key property cannot be an entity type or include an entity type.");

                case ObjectValueType.Object:
                    foreach (var prop in keyPropMetadata.Properties)
                    {
                        this.RegisterKeyProperty(prop);
                    }

                    break;

                default:
                    throw new NotSupportedException($"Key property cannot be {keyPropMetadata.ValueType}");
            }
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
