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
        /// <param name="type">The type name.</param>
        /// <param name="properties">The peoperties of object.</param>
        public ListMetadata(
            Type type,
            string name,
            Type innerType)
            : base(type, ObjectValueType.List, name)
        {
            this.InnerType = innerType;
        }

        /// <summary>
        /// The inner type of current enumerable type.
        /// </summary>
        public Type InnerType { get; protected set; }

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
    }
}
