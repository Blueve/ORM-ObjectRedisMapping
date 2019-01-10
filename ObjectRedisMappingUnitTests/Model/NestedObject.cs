namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    public class NestedObject
    {
        public virtual string Name { get; set; }

        public virtual NestedObject Child { get; set; }
    }
}
