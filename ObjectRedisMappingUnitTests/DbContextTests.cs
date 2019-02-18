namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Blueve.RedisEmulator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test the <see cref="DbContext"/>.
    /// </summary>
    [TestClass]
    public class DbContextTests
    {
        private RedisDatabase db;
        private IDbContext dbContext;

        [TestInitialize]
        public void Initialize()
        {
            var factory = new DbContextFactory();
            this.db = new RedisDatabase();
            this.dbContext = factory.Create(this.db);
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
            Assert.AreEqual("Blueve", this.db.StringGet("PlainEntity1UserName").ToString());
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
            this.db.StringSet("PlainEntity1", "True");
            this.db.StringSet("PlainEntity1UserId", "1");
            this.db.StringSet("PlainEntity1UserName", "Blueve");

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
