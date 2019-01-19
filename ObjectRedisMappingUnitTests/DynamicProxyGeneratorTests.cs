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
        private DynamicProxyStub stub;
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

            var typeRepo = new TypeRepository(new TypeMetadataGenerator());
            var dbRecordSubmitter = new DbRecordSubmitter(this.dbAccessor.Object);

            this.keyGenerator = new EntityKeyGenerator();
            this.dbRecordBuilder = new DbRecordBuilder(typeRepo, this.keyGenerator);
            this.stub = new DynamicProxyStub(typeRepo, this.dbAccessor.Object, this.dbRecordBuilder, this.keyGenerator, dbRecordSubmitter);
            this.generator = new DynamicProxyGenerator(typeRepo, this.keyGenerator, this.stub);
        }

        [TestMethod]
        public void TestGenerateForEntity_PlainEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntityBlueveUserId", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<PlainEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.UserId);

            proxyObj.UserName = "Blueve";
            Assert.AreEqual("Blueve", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntityBlueveUserName"]);
        }

        [TestMethod]
        public void TestGenerateForEntity_NestedEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntityBlueveKey", "Blueve");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntityYoudKey", "Youd");

            var proxyObj = this.generator.GenerateForEntity<NestedEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.Key);

            proxyObj.LeftChild = new NestedEntity
            {
                Key = "Youd"
            };
            Assert.AreEqual("Youd", proxyObj.LeftChild.Key);
            Assert.AreEqual("Youd", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntityBlueveLeftChild"]);
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

        [TestMethod]
        public void TestGenerateForEntity_KeyIsObject_UseInterface_Entity_ReadKeyValue()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.KeyIsObject_UseInterface_EntityKeyBlueveKeyValue", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<KeyIsObject_UseInterface_Entity>("KeyBlueve");
            Assert.AreEqual("Blueve", proxyObj.Key.Value);
        }

        [TestMethod]
        public void TestGenerateForEntity_KeyIsObject_UseInterface_Entity_UpdateKeyValue()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.KeyIsObject_UseInterface_EntityKeyBlueveKeyValue", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<KeyIsObject_UseInterface_Entity>("KeyBlueve");
            try
            {
                proxyObj.Key.Value = "Ada";
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}
