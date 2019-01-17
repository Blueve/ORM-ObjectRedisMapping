namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    public class ObjectEntityKey : IEntityKey
    {
        public virtual string Value { get; set; }

        public string GetKey()
        {
            return $"Key{this.Value}";
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
