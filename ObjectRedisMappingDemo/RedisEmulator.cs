namespace ObjectRedisMappingDemo
{
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;

    internal class RedisEmulator : IDatabaseClient
    {
        private readonly ISet<string> keys = new HashSet<string>();
        private readonly IDictionary<string, string> stringDict = new Dictionary<string, string>();

        public bool KeyExists(string key)
        {
            return this.keys.Contains(key);
        }

        public string StringGet(string key)
        {
            return this.stringDict[key];
        }

        public void StringSet(string key, string value)
        {
            this.keys.Add(key);
            this.stringDict[key] = value;
        }
    }
}
