namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
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
        private DbRecordBuilder dbRecordBuilder;
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
            this.dbRecordBuilder = new DbRecordBuilder(TypeRepository.CreateInstance(), this.keyGenerator);
            this.generator = new DynamicProxyGenerator(
                TypeRepository.CreateInstance(),
                this.dbAccessor.Object,
                this.dbRecordBuilder,
                this.keyGenerator,
                string.Empty);
        }

        [TestMethod]
        public void TestGenerateForEntity_PlainEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity00000006BlueveUserId", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<PlainEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.UserId);

            proxyObj.UserName = "Blueve";
            Assert.AreEqual("Blueve", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity00000006BlueveUserName"]);
        }

        [TestMethod]
        public void TestGenerateForEntity_NestedEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity00000006BlueveKey", "Blueve");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity00000004YoudKey", "Youd");

            var proxyObj = this.generator.GenerateForEntity<NestedEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.Key);

            proxyObj.LeftChild = new NestedEntity
            {
                Key = "Youd"
            };
            Assert.AreEqual("Youd", proxyObj.LeftChild.Key);
            Assert.AreEqual("Youd", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity00000006BlueveLeftChild"]);
        }

        [TestMethod]
        public void TestGenerateForObject_PlainObject()
        {
            this.db.Add("PrefixName", "Age");
            this.db.Add("PrefixValue", "18");

            var proxyObj = this.generator.GenerateForObject<PlainObject>("Prefix");
            Assert.AreEqual("Age", proxyObj.Name);
            Assert.AreEqual("18", proxyObj.Value);

            proxyObj.Value = "19";
            Assert.AreEqual("19", this.db["PrefixValue"]);
        }

        [TestMethod]
        public void TestGenerateForObject_NestedObject()
        {
            this.db.Add("PrefixName", "Blueve");
            this.db.Add("PrefixChildName", "Youd");

            var proxyObj = this.generator.GenerateForObject<NestedObject>("Prefix");
            Assert.AreEqual("Blueve", proxyObj.Name);
            Assert.AreEqual("Youd", proxyObj.Child.Name);

            proxyObj.Child = new NestedObject
            {
                Name = "Ada"
            };
            Assert.AreEqual("Ada", this.db["PrefixChildName"]);
        }

        [TestMethod]
        public void TestGenerateForEntity_PlainEntity_OverrideKey()
        {
            try
            {
                var proxyObj = this.generator.GenerateForEntity<PlainEntity>("Blueve");
                proxyObj.UserId = "2";
                Assert.Fail();
            }
            catch(InvalidOperationException)
            {
            }
        }
    }
}
