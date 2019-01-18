namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The type repository.
    /// </summary>
    internal class TypeRepository
    {
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

        /// <summary>
        /// Register a type to repository.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        public void Register<T>()
        {
            var type = typeof(T);
            this.Register(type);
        }

        /// <summary>
        /// Register a type to repository.
        /// </summary>
        /// <param name="type">The type.</param>
        public void Register(Type type)
        {
            if (this.typeMetadataDict.ContainsKey(type))
            {
                return;
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

        /// <summary>
        /// Gets the type metadata of a given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The type's type metadata.</returns>
        public TypeMetadata Get<T>()
        {
            var type = typeof(T);
            return this.Get(type);
        }

        /// <summary>
        /// Gets the type metadata of a given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type's type metadata.</returns>
        public TypeMetadata Get(Type type)
        {
            if (this.typeMetadataDict.TryGetValue(type, out var typeMetdata))
            {
                return typeMetdata;
            }

            throw new KeyNotFoundException($"The type {type.FullName} hasn't registered.");
        }

        /// <summary>
        /// Try get the type metadata of a given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeMetadata">The type's type metadata.</param>
        /// <returns>True if get the metadata succeed.</returns>
        public bool TryGet(Type type, out TypeMetadata typeMetadata)
        {
            return this.typeMetadataDict.TryGetValue(type, out typeMetadata);
        }

        /// <summary>
        /// Gets the type's metadata and create one if the metadata not registered before.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type's type metadata.</returns>
        public TypeMetadata GetOrRegister(Type type)
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
            var keyPropMetadata = this.GetOrRegister(keyProp.PropertyType);
            
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
