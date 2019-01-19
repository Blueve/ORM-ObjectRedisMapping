namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    public class ListNodeEntity
    {
        [EntityKey]
        public virtual int Id { get; set; }

        public virtual short Val { get; set; }

        public virtual ListNodeEntity Next { get; set; }
    }
}
