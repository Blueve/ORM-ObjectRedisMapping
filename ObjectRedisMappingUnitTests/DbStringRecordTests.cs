namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using StackExchange.Redis;

    /// <summary>
    /// Test the <see cref="DbRecordSubmitter"/>.
    /// </summary>
    [TestClass]
    public class DbStringRecordTests
    {
        private Mock<IDatabase> dbClient;
        private IDictionary<string, string> dict;

        [TestInitialize]
        public void Initialize()
        {
            this.dbClient = new Mock<IDatabase>();
            this.dict = new Dictionary<string, string>();
            this.dbClient
                .Setup(accessor => accessor.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, When.Always, CommandFlags.None))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((k, v, t, w, c) => this.dict.TryAdd(k.ToString(), v.ToString()));
        }

        [DataTestMethod]
        [DataRow("DbKey", "DbValue")]
        [DataRow("DbKey", "")]
        public void TestCommit_StringRecord(string key, string value)
        {
            var record = new DbStringRecord(key, value);
            record.AddOrUpdate(this.dbClient.Object);

            Assert.AreEqual(value, this.dict[key]);
        }
    }
}
