namespace ObjectRedisMappingDemo
{
    using System;
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;
    using Blueve.RedisEmulator;
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
                Name = "Tom",
                Age = 18,
                Fellows = new List<Person>()
            };
            Console.WriteLine("Commit a Person{ Name=Blueve, Age=27 } to database");
            dbContext.Save(person);
            PrintDbStatus(redisEmulator.Explain());

            Console.WriteLine("Get the person from DB");
            person = dbContext.Find<Person>(person.Name);

            Console.WriteLine("The person get married with Person{ Name=Ada, Age=26 }");
            person.Fellows.Add(
                new Person
                {
                    Name = "Jerry",
                    Age = 10,
                    Fellows = new List<Person>()
                }
            );
            person.Fellows[0].Fellows.Add(person);
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
