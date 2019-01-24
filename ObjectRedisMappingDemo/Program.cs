namespace ObjectRedisMappingDemo
{
    using Blueve.ObjectRedisMapping;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Get a database context.
            var redisEmulator = new RedisEmulator();
            var dbContext = DbContextFactory.GetDbContext(redisEmulator);
        }
    }
}
