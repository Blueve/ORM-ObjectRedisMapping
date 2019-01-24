namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test the <see cref="DbContext"/>.
    /// </summary>
    [TestClass]
    public class DbContextTests
    {
        private Dictionary<string, string> db;
        private Mock<IDatabaseClient> dbClient;
        private IDbContext dbContext;

        [TestInitialize]
        public void Initialize()
        {
            this.db = new Dictionary<string, string>();
            this.dbClient = new Mock<IDatabaseClient>();
            this.dbClient
                .Setup(accessor => accessor.StringSet(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((k, v) => this.db[k] = v);
            this.dbClient
                .Setup(accessor => accessor.StringGet(It.IsAny<string>()))
                .Returns<string>(k => this.db[k]);
            this.dbClient
                .Setup(accessor => accessor.KeyExists(It.IsAny<string>()))
                .Returns<string>(k => this.db.ContainsKey(k));

            var factory = new DbContextFactory();
            this.dbContext = factory.Create(this.dbClient.Object);
        }

        [TestMethod]
        public void TestCommit_PlainEntity()
        {
            var entity = new PlainEntity
            {
                UserId = "1",
                UserName = "Blueve"
            };

            this.dbContext.Commit(entity);
            Assert.AreEqual("Blueve", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity1UserName"]);
        }

        [TestMethod]
        public void TestCommit_PlainObject()
        {
            var entity = new PlainObject
            {
                Name = "UserName",
                Value = "Blueve"
            };

            try
            {
                this.dbContext.Commit(entity);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void TestFind_PlainEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity1", "True");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity1UserId", "1");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity1UserName", "Blueve");

            var entity = this.dbContext.Find<PlainEntity>("1");
            Assert.AreEqual("Blueve", entity.UserName);
        }

        [TestMethod]
        public void TestFind_PlainEntity_NotExists()
        {
            var entity = this.dbContext.Find<PlainEntity>("1");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void TestFind_PlainObject()
        {
            try
            {
                this.dbContext.Find<PlainObject>("1");
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}
