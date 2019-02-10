namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    /// <summary>
    /// The type metadata for list.
    /// </summary>
    internal class ListMetadata : TypeMetadata
    {
        /// <summary>
        /// Initialize an instacne of <see cref="ListMetadata"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The type name.</param>
        /// <param name="innerType">The inner type.</param>
        /// <param name="readOnly">Indicate that if the current list is read-only.</param>
        public ListMetadata(
            Type type,
            string name,
            Type innerType,
            bool readOnly)
            : base(type, ObjectValueType.List, name)
        {
            this.InnerType = innerType;
            this.ReadOnly = readOnly;
        }

        /// <summary>
        /// The inner type of current enumerable type.
        /// </summary>
        public Type InnerType { get; protected set; }

        /// <summary>
        /// Indicate that if the current list is read-only.
        /// </summary>
        public bool ReadOnly { get; protected set; }

        /// <inheritdoc/>
        protected override string StubGetterMethodName => "ListGetter";

        /// <inheritdoc/>
        protected override string StubSetterMethodName => "ListSetter";

        /// <inheritdoc/>
        public override void RegisterSubType(TypeRepository typeRepo)
        {
            typeRepo.Register(this.InnerType);
        }

        /// <inheritdoc/>
        public override void RegisterAsKeyType(TypeRepository typeRepo)
        {
            typeRepo.RegisterKeyProperty(this.InnerType);
        }

        /// <inheritdoc/>
        public override void CallStubGetter(ILGenerator ilGenerator, bool readOnly)
        {
            var stubMethodName = readOnly ? string.Concat("ReadOnly", this.StubGetterMethodName) : this.StubGetterMethodName;
            ilGenerator.Emit(
                OpCodes.Call,
                typeof(DynamicProxyStub)
                    .GetMethods()
                    .First(m => m.Name.Equals(stubMethodName)).MakeGenericMethod(this.InnerType));
        }

        /// <inheritdoc/>
        public override void CallStubSetter(ILGenerator ilGenerator)
        {
            ilGenerator.Emit(
                OpCodes.Call,
                typeof(DynamicProxyStub)
                    .GetMethods()
                    .First(m => m.Name.Equals(this.StubSetterMethodName)).MakeGenericMethod(this.InnerType));
        }

        /// <inheritdoc/>
        public override IEnumerable<IDbOperation> GenerateDbRecords<T>(IDbRecordBuilder dbRecordBuilder, string prefix, T value)
        {
            return dbRecordBuilder.GenerateObjectRecord(prefix, value, this);
        }
    }
}
