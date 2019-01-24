﻿namespace Blueve.ObjectRedisMapping.UnitTests
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
        private Mock<IDatabaseClient> dbClient;
        private DbRecordBuilder dbRecordBuilder;
        private EntityKeyGenerator keyGenerator;
        private DynamicProxyStub stub;
        private DynamicProxyGenerator generator;

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

            var typeRepo = new TypeRepository(new TypeMetadataGenerator());
            var dbRecordSubmitter = new DbRecordSubmitter(this.dbClient.Object);

            this.keyGenerator = new EntityKeyGenerator();
            this.dbRecordBuilder = new DbRecordBuilder(typeRepo, this.keyGenerator);
            this.stub = new DynamicProxyStub(typeRepo, this.dbClient.Object, this.dbRecordBuilder, this.keyGenerator, dbRecordSubmitter);
            this.generator = new DynamicProxyGenerator(typeRepo, this.keyGenerator, this.stub, this.dbClient.Object);
        }

        [TestMethod]
        public void TestGenerateForEntity_PlainEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntityBlueve", "True");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntityBlueveUserId", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<PlainEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.UserId);

            proxyObj.UserName = "Blueve";
            Assert.AreEqual("Blueve", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntityBlueveUserName"]);
        }

        [TestMethod]
        public void TestGenerateForEntity_PlainEntity_NotExists()
        {
            var proxyObj = this.generator.GenerateForEntity<PlainEntity>("Blueve");
            Assert.IsNull(proxyObj);
        }

        [TestMethod]
        public void TestGenerateForEntity_NestedEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntityBlueve", "True");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntityBlueveKey", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<NestedEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.Key);

            proxyObj.LeftChild = new NestedEntity
            {
                Key = "Youd"
            };
            Assert.AreEqual("Youd", proxyObj.LeftChild.Key);
            Assert.AreEqual("Youd", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntityBlueveLeftChild"]);
            Assert.AreEqual("Youd", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntityYoudKey"]);
        }

        [TestMethod]
        public void TestGenerateForEntity_PlainObject()
        {
            try
            {
                this.generator.GenerateForEntity<PlainObject>("Blueve");
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestGenerateForObject_PlainObject()
        {
            this.db.Add("Prefix", "True");
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
            this.db.Add("Prefix", "True");
            this.db.Add("PrefixName", "Blueve");
            this.db.Add("PrefixChild", "True");
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
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntityBlueve", "True");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntityBlueveUserId", "1");

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
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.KeyIsObject_UseInterface_EntityKeyBlueve", "True");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.KeyIsObject_UseInterface_EntityKeyBlueveKey", "True");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.KeyIsObject_UseInterface_EntityKeyBlueveKeyValue", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<KeyIsObject_UseInterface_Entity>("KeyBlueve");
            Assert.AreEqual("Blueve", proxyObj.Key.Value);
        }

        [TestMethod]
        public void TestGenerateForEntity_KeyIsObject_UseInterface_Entity_UpdateKeyValue()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.KeyIsObject_UseInterface_EntityKeyBlueve", "True");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.KeyIsObject_UseInterface_EntityKeyBlueveKey", "True");
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

        [TestMethod]
        public void TestGenerateForEntity_ListNodeEntity()
        {
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.ListNodeEntity1", "True");
            this.db.Add("Blueve.ObjectRedisMapping.UnitTests.Model.ListNodeEntity1Id", "1");

            var proxyObj = this.generator.GenerateForEntity<ListNodeEntity>("1");
            Assert.AreEqual(1, proxyObj.Id);

            proxyObj.Val = 1;
            Assert.AreEqual(1, proxyObj.Val);
            Assert.AreEqual("1", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.ListNodeEntity1Val"]);

            var tail = new ListNodeEntity
            {
                Id = 2,
                Val = 2,
                Next = new ListNodeEntity
                {
                    Id = 3,
                    Val = 3
                }
            };

            proxyObj.Next = tail;

            Assert.AreEqual("2", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.ListNodeEntity1Next"]);
            Assert.AreEqual("2", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.ListNodeEntity2Val"]);
            Assert.AreEqual("3", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.ListNodeEntity2Next"]);
            Assert.AreEqual("3", this.db["Blueve.ObjectRedisMapping.UnitTests.Model.ListNodeEntity3Val"]);
        }

        [TestMethod]
        public void TestGenerateForEntity_Null()
        {
            Assert.IsNull(this.generator.GenerateForEntity<PlainEntity>("Key"));
        }

        [TestMethod]
        public void TestGenerateForObject_Null()
        {
            Assert.IsNull(this.generator.GenerateForObject<PlainObject>("Key"));
        }
    }
}
