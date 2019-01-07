namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test the <see cref="DynamicProxyGenerator"/>.
    /// </summary>
    [TestClass]
    public class DynamicProxyGeneratorTests
    {
        private Dictionary<string, string> db;
        private Mock<IDbAccessor> dbAccessor;
        private EntityKeyGenerator keyGenerator;
        private DynamicProxyGenerator generator;

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
            this.keyGenerator = new EntityKeyGenerator(new EntityKeyValueFormatter());
            this.generator = new DynamicProxyGenerator(
                TypeRepository.CreateInstance(),
                this.dbAccessor.Object,
                this.keyGenerator,
                string.Empty);
        }

        [TestMethod]
        public void TestGenerateEntityProxy_PlainEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity00000006BlueveUserId", "Blueve");

            var proxyObj = this.generator.Generate<PlainEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.UserId);

            proxyObj.UserName = "Blueve";
            Assert.AreEqual("Blueve", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity00000006BlueveUserName"]);
        }

        [TestMethod]
        public void TestGenerateEntityProxy_NestedEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity00000006BlueveKey", "Blueve");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity00000004YoudKey", "Youd");

            var proxyObj = this.generator.Generate<NestedEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.Key);

            proxyObj.LeftChild = new NestedEntity
            {
                Key = "Youd"
            };
            Assert.AreEqual("Youd", proxyObj.LeftChild.Key);
            Assert.AreEqual("Youd", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity00000006BlueveLeftChild"]);
        }
    }
}
