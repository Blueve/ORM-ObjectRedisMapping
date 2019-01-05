namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    using Blueve.ObjectRedisMapping;

    public class NestedEntity
    {
        [EntityKey]
        public virtual string Key { get; set; }

        public virtual NestedEntity LeftChild { get; set; }

        public virtual NestedEntity RightChild { get; set; }
    }
}
