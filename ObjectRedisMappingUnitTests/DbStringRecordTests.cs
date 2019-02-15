namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test the <see cref="DbRecordSubmitter"/>.
    /// </summary>
    [TestClass]
    public class DbStringRecordTests
    {
        private Mock<IDatabaseClient> dbClient;
        private IDictionary<string, string> dict;

        [TestInitialize]
        public void Initialize()
        {
            this.dbClient = new Mock<IDatabaseClient>();
            this.dict = new Dictionary<string, string>();
            this.dbClient
                .Setup(accessor => accessor.StringSet(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((k, v) => this.dict.TryAdd(k, v));
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
