namespace ObjectRedisMappingDemo
{
    using System.Collections.Generic;
    using System.Linq;
    using Blueve.ObjectRedisMapping;

    internal class RedisEmulator : IDatabaseClient
    {
        private readonly ISet<string> keys = new HashSet<string>();
        private readonly IDictionary<string, string> stringDict = new Dictionary<string, string>();

        public IEnumerable<string> Explain()
        {
            foreach (var kv in this.stringDict.OrderBy(kv => kv.Key))
            {
                yield return $"{kv.Key, -50}: {kv.Value}";
            }
        }

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
