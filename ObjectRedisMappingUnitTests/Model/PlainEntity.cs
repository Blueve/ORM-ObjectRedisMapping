namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    using Blueve.ObjectRedisMapping;

    public class PlainEntity
    {
        [EntityKey]
        public virtual string UserId { get; set; }

        public virtual string UserName { get; set; }
    }
}
