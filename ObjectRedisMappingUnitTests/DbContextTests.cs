namespace Blueve.ObjectRedisMapping.UnitTests
{
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
        private Mock<IDbAccessor> dbAccessor;
        private IDbContext dbContext;

        [TestInitialize]
        public void Initialize()
        {
            this.db = new Dictionary<string, string>();
            this.dbAccessor = new Mock<IDbAccessor>();
            this.dbAccessor
                .Setup(accessor => accessor.Set(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((k, v) => this.db[k] = v);
            this.dbAccessor
                .Setup(accessor => accessor.Get(It.IsAny<string>()))
                .Returns<string>(k => this.db[k]);
            this.dbAccessor
                .Setup(accessor => accessor.KeyExists(It.IsAny<string>()))
                .Returns<string>(k => this.db.ContainsKey(k));

            var factory = new DbContextFactory();
            this.dbContext = factory.Create(this.dbAccessor.Object);
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
        public void TestFind_PlainEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity1", "True");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity1UserId", "1");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity1UserName", "Blueve");

            var entity = this.dbContext.Find<PlainEntity>("1");
            Assert.AreEqual("Blueve", entity.UserName);
        }
    }
}
