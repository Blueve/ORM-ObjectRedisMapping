namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    public class KeyIsEntity_InvalidEntity
    {
        [EntityKey]
        public virtual PlainEntity Key { get; set; }
    }
}
