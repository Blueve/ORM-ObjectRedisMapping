namespace ObjectRedisMappingDemo
{
    using System;
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;
    using ObjectRedisMappingDemo.Model;

    public static class Program
    {
        public static void Main(string[] args)
        {
            // Get a database context.
            var redisEmulator = new RedisEmulator();
            var dcContextFactory = new DbContextFactory();
            var dbContext = dcContextFactory.Create(redisEmulator);

            // Commit an entity.
            var person = new Person
            {
                Id = 1,
                Name = "Blueve",
                Age = 27
            };
            Console.WriteLine("Commit a Person{ Id=1, Name=Blueve, Age=27 } to database");
            dbContext.Save(person);
            PrintDbStatus(redisEmulator.Explain());

            Console.WriteLine("Get the person from DB");
            person = dbContext.Find<Person>(person.Id.ToString());

            Console.WriteLine("The person get married with Person{ Id=2, Name=Ada, Age=26 }");
            person.Partner = new Person
            {
                Id = 2,
                Name = "Ada",
                Age = 26
            };
            person.Partner.Partner = person;
            PrintDbStatus(redisEmulator.Explain());

            Console.ReadKey();
        }

        private static void PrintDbStatus(IEnumerable<string> records)
        {
            Console.WriteLine("DB Status:");
            foreach (var record in records)
            {
                Console.WriteLine(record);
            }

            Console.WriteLine();
        }
    }
}
