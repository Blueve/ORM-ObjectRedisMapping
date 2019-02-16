namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using StackExchange.Redis;

    /// <summary>
    /// Test the <see cref="DbContext"/>.
    /// </summary>
    [TestClass]
    public class DbContextTests
    {
        private Dictionary<string, string> db;
        private Mock<IDatabase> dbClient;
        private IDbContext dbContext;

        [TestInitialize]
        public void Initialize()
        {
            this.db = new Dictionary<string, string>();
            this.dbClient = new Mock<IDatabase>();
            this.dbClient
                .Setup(accessor => accessor.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, When.Always, CommandFlags.None))
                .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((k, v, t, w, c) => this.db[k] = v);
            this.dbClient
                .Setup(accessor => accessor.StringGet(It.IsAny<RedisKey>(), CommandFlags.None))
                .Returns<RedisKey, CommandFlags>((k, c) => this.db[k]);
            this.dbClient
                .Setup(accessor => accessor.KeyExists(It.IsAny<RedisKey>(), CommandFlags.None))
                .Returns<RedisKey, CommandFlags>((k, c) => this.db.ContainsKey(k));

            var factory = new DbContextFactory();
            this.dbContext = factory.Create(this.dbClient.Object);
        }

        [TestMethod]
        public void TestSave_PlainEntity()
        {
            var entity = new PlainEntity
            {
                UserId = "1",
                UserName = "Blueve"
            };

            this.dbContext.Save(entity);
            Assert.AreEqual("Blueve", this.db["PlainEntity1UserName"]);
        }

        [TestMethod]
        public void TestSave_PlainObject()
        {
            var entity = new PlainObject
            {
                Name = "UserName",
                Value = "Blueve"
            };

            try
            {
                this.dbContext.Save(entity);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void TestFind_PlainEntity()
        {
            this.db.Add("PlainEntity1", "True");
            this.db.Add("PlainEntity1UserId", "1");
            this.db.Add("PlainEntity1UserName", "Blueve");

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
