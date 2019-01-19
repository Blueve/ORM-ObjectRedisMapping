namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    public class KeyIsObject_UseInterface_NotImplement_Entity
    {
        [EntityKey(UseInterface = true)]
        public virtual PlainObject Key { get; set; }
    }
}
