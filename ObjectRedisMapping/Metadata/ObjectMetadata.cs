namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    /// <summary>
    /// The type metadata for object.
    /// </summary>
    internal class ObjectMetadata : TypeMetadata
    {
        /// <summary>
        /// Initialize an instacne of <see cref="ObjectMetadata"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The type name.</param>
        /// <param name="properties">The peoperties of object.</param>
        public ObjectMetadata(
            Type type,
            string name,
            IEnumerable<PropertyInfo> properties)
            : base(type, ObjectValueType.Object, name)
        {
            this.Properties = properties.ToArray();
        }

        /// <summary>
        /// The properties which can be mapping to database.
        /// </summary>
        public PropertyInfo[] Properties { get; private set; }

        /// <inheritdoc/>
        protected override string StubGetterMethodName => "ObjectGetter";

        /// <inheritdoc/>
        protected override string StubSetterMethodName => "ObjectSetter";

        /// <inheritdoc/>
        public override void RegisterSubType(TypeRepository typeRepo)
        {
            foreach (var prop in this.Properties)
            {
                typeRepo.Register(prop.PropertyType);
            }
        }

        /// <inheritdoc/>
        public override void RegisterAsKeyType(TypeRepository typeRepo)
        {
            foreach (var prop in this.Properties)
            {
                typeRepo.RegisterKeyProperty(prop.PropertyType);
            }
        }

        /// <inheritdoc/>
        public override void CallStubGetter(ILGenerator ilGenerator, bool readOnly)
        {
            var stubMethodName = readOnly ? string.Concat("ReadOnly", this.StubGetterMethodName) : this.StubGetterMethodName;
            ilGenerator.Emit(
                OpCodes.Call,
                typeof(DynamicProxyStub)
                    .GetMethods()
                    .First(m => m.Name.Equals(stubMethodName)).MakeGenericMethod(this.Type));
        }

        /// <inheritdoc/>
        public override void CallStubSetter(ILGenerator ilGenerator)
        {
            ilGenerator.Emit(
                OpCodes.Call,
                typeof(DynamicProxyStub)
                    .GetMethods()
                    .First(m => m.Name.Equals(this.StubSetterMethodName)).MakeGenericMethod(this.Type));
        }

        /// <inheritdoc/>
        public override IEnumerable<IDbOperation> GenerateDbRecords<T>(IDbRecordBuilder dbRecordBuilder, string prefix, T value)
        {
            return dbRecordBuilder.GenerateObjectRecord(prefix, value, this);
        }
    }
}
