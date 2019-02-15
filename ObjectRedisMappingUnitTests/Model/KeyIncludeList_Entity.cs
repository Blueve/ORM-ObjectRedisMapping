namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    public class KeyIncludeList_Entity
    {
        [EntityKey(UseInterface = true)]
        public virtual ListObject Key { get; set; }
    }
}
