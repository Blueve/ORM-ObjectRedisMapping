namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// The type metadata.
    /// </summary>
    internal class TypeMetadata
    {
        /// <summary>
        /// Initialze an instance of <see cref="TypeMetadata"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="valueType">The value type.</param>
        /// <param name="name">The type name.</param>
        public TypeMetadata(
            Type type,
            ObjectValueType valueType,
            string name)
        {
            this.Type = type;
            this.ValueType = valueType;
            this.Name = name;
        }

        /// <summary>
        /// The origin type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The object value type.
        /// </summary>
        public ObjectValueType ValueType { get; protected set; }

        /// <summary>
        /// The type name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the stub getter method name.
        /// </summary>
        protected virtual string StubGetterMethodName => string.Concat(this.Name, "Getter");

        /// <summary>
        /// Gets the stub setter method name.
        /// </summary>
        protected virtual string StubSetterMethodName => string.Concat(this.Name, "Setter");

        /// <summary>
        /// Register sub-types to type repository.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        public virtual void RegisterSubType(TypeRepository typeRepo)
        {
        }

        /// <summary>
        /// Register key type to type repository.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        public virtual void RegisterKeyType(TypeRepository typeRepo)
        {
        }

        /// <summary>
        /// Build getter that call a getter which defined in dynamic proxy stub.
        /// </summary>
        /// <param name="ilGenerator">The getter's IL generator.</param>
        /// <param name="readonly">True if the proxy from the getter is readonly.</param>
        public virtual void CallStubGetter(ILGenerator ilGenerator, bool readOnly)
        {
            ilGenerator.Emit(
                OpCodes.Call,
                typeof(DynamicProxyStub).GetMethod(this.StubGetterMethodName, new[] { typeof(string) }));
        }

        /// <summary>
        /// Build setter that call a setter which defined in dynamic proxy stub.
        /// </summary>
        /// <param name="ilGenerator"></param>
        public virtual void CallStubSetter(ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(
                OpCodes.Call,
                typeof(DynamicProxyStub).GetMethod(this.StubSetterMethodName, new[] { typeof(string), this.Type }));
        }

        /// <summary>
        /// Generate database records for the given value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="dbRecordBuilder">The database record builder.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="value">The object value.</param>
        /// <returns></returns>
        public virtual IEnumerable<DbRecord> GenerateDbRecords<T>(IDbRecordBuilder dbRecordBuilder, string prefix, T value)
        {
            return dbRecordBuilder.GenerateStringRecord<T>(prefix, value);
        }
    }
}
