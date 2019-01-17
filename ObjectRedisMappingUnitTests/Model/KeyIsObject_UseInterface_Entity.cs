namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    public class KeyIsObject_UseInterface_Entity
    {
        [EntityKey(UseInterface = true)]
        public virtual ObjectEntityKey Key { get; set; }
    }
}
