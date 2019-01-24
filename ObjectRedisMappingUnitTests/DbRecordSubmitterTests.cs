namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test the <see cref="DbRecordSubmitter"/>.
    /// </summary>
    [TestClass]
    public class DbRecordSubmitterTests
    {
        private Mock<IDatabaseClient> dbClient;
        private DbRecordSubmitter submitter;

        [TestInitialize]
        public void Initialize()
        {
            this.dbClient = new Mock<IDatabaseClient>();
            this.submitter = new DbRecordSubmitter(this.dbClient.Object);
        }

        [DataTestMethod]
        [DataRow("DbKey", "DbValue")]
        [DataRow("DbKey", "")]
        public void TestCommit_StringRecord(string key, string value)
        {
            var stringDict = new Dictionary<string, string>();
            this.dbClient
                .Setup(accessor => accessor.StringSet(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((k, v) => stringDict.TryAdd(k, v));

            // Commit the entity.
            this.submitter.Commit(DbRecord.GenerateStringRecord(key, value));
            Assert.AreEqual(value, stringDict[key]);
        }
    }
}
