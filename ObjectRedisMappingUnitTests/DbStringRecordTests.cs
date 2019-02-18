namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Blueve.ObjectRedisMapping;
    using Blueve.RedisEmulator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using StackExchange.Redis;

    /// <summary>
    /// Test the <see cref="DbRecordSubmitter"/>.
    /// </summary>
    [TestClass]
    public class DbStringRecordTests
    {
        private RedisDatabase db;

        [TestInitialize]
        public void Initialize()
        {
            this.db = new RedisDatabase();
        }

        [DataTestMethod]
        [DataRow("DbKey", "DbValue")]
        [DataRow("DbKey", "")]
        public async Task TestCommit_StringRecord(string key, string value)
        {
            var record = new DbStringRecord(key, value);
            await record.AddOrUpdate(this.db);

            Assert.AreEqual(value, this.db.StringGet(key).ToString());
        }
    }
}
