namespace ObjectRedisMappingDemo.Model
{
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;

    public class Person
    {
        [EntityKey]
        public virtual string Name { get; set; }

        public virtual short Age { get; set; }

        public virtual IList<Person> Fellows { get; set; }
    }
}
