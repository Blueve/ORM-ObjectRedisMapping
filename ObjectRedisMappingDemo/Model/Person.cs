namespace ObjectRedisMappingDemo.Model
{
    using Blueve.ObjectRedisMapping;

    public class Person
    {
        [EntityKey]
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual short Age { get; set; }

        public virtual Person Partner { get; set; }
    }
}
