namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blueve.ObjectRedisMapping;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using StackExchange.Redis;

    /// <summary>
    /// Test the <see cref="DynamicProxyGenerator"/>.
    /// </summary>
    [TestClass]
    public class DynamicProxyGeneratorTests
    {
        private Dictionary<string, string> db;
        private Mock<IDatabase> dbClient;
        private DbRecordBuilder dbRecordBuilder;
        private EntityKeyGenerator keyGenerator;
        private DynamicProxyStub stub;
        private DynamicProxyGenerator generator;

        [TestInitialize]
        public void Initialize()
        {
            this.db = new Dictionary<string, string>();
            this.dbClient = new Mock<IDatabase>();
            this.dbClient
                .Setup(accessor => accessor.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), null, When.Always, CommandFlags.None))
                .Returns<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>((k, v, t, w, c) =>
                {
                    this.db[k] = v;
                    return true;
                });
            this.dbClient
                .Setup(accessor => accessor.StringGet(It.IsAny<RedisKey>(), CommandFlags.None))
                .Returns<RedisKey, CommandFlags>((k, c) => this.db[k]);
            this.dbClient
                .Setup(accessor => accessor.KeyExists(It.IsAny<RedisKey>(), CommandFlags.None))
                .Returns<RedisKey, CommandFlags>((k, c) => this.db.ContainsKey(k));

            var typeRepo = new TypeRepository(new TypeMetadataGenerator(false));

            this.keyGenerator = new EntityKeyGenerator();
            this.dbRecordBuilder = new DbRecordBuilder(typeRepo, this.keyGenerator);
            this.stub = new DynamicProxyStub(typeRepo, this.dbClient.Object, this.dbRecordBuilder, this.keyGenerator);
            this.generator = new DynamicProxyGenerator(typeRepo, this.keyGenerator, this.stub, this.dbClient.Object);
        }

        [TestMethod]
        public void TestGenerateForEntity_PlainEntity()
        {
            this.db.Add("PlainEntityBlueve", "True");
            this.db.Add("PlainEntityBlueveUserId", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<PlainEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.UserId);

            proxyObj.UserName = "Blueve";
            Assert.AreEqual("Blueve", this.db["PlainEntityBlueveUserName"]);
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
            this.db.Add("NestedEntityBlueve", "True");
            this.db.Add("NestedEntityBlueveKey", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<NestedEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.Key);

            proxyObj.LeftChild = new NestedEntity
            {
                Key = "Youd"
            };
            Assert.AreEqual("Youd", proxyObj.LeftChild.Key);
            Assert.AreEqual("Youd", this.db["NestedEntityBlueveLeftChild"]);
            Assert.AreEqual("Youd", this.db["NestedEntityYoudKey"]);
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
            this.db.Add("PlainEntityBlueve", "True");
            this.db.Add("PlainEntityBlueveUserId", "1");

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
            this.db.Add("KeyIsObject_UseInterface_EntityKeyBlueve", "True");
            this.db.Add("KeyIsObject_UseInterface_EntityKeyBlueveKey", "True");
            this.db.Add("KeyIsObject_UseInterface_EntityKeyBlueveKeyValue", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<KeyIsObject_UseInterface_Entity>("KeyBlueve");
            Assert.AreEqual("Blueve", proxyObj.Key.Value);
        }

        [TestMethod]
        public void TestGenerateForEntity_KeyIsObject_UseInterface_Entity_UpdateKeyValue()
        {
            this.db.Add("KeyIsObject_UseInterface_EntityKeyBlueve", "True");
            this.db.Add("KeyIsObject_UseInterface_EntityKeyBlueveKey", "True");
            this.db.Add("KeyIsObject_UseInterface_EntityKeyBlueveKeyValue", "Blueve");

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
            this.db.Add("ListNodeEntity1", "True");
            this.db.Add("ListNodeEntity1Id", "1");

            var proxyObj = this.generator.GenerateForEntity<ListNodeEntity>("1");
            Assert.AreEqual(1, proxyObj.Id);

            proxyObj.Val = 1;
            Assert.AreEqual(1, proxyObj.Val);
            Assert.AreEqual("1", this.db["ListNodeEntity1Val"]);

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

            Assert.AreEqual("2", this.db["ListNodeEntity1Next"]);
            Assert.AreEqual("2", this.db["ListNodeEntity2Val"]);
            Assert.AreEqual("3", this.db["ListNodeEntity2Next"]);
            Assert.AreEqual("3", this.db["ListNodeEntity3Val"]);
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

        [TestMethod]
        public void TestGenerateForList_PrimitiveList()
        {
            this.db.Add("Prefix", "2");
            this.db.Add("Prefix0", "1992");
            this.db.Add("Prefix1", "2019");

            var proxyList = this.generator.GenerateForList<int>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual(1992, proxyList[0]);
            Assert.AreEqual(2019, proxyList[1]);

            CollectionAssert.AreEqual(new[] { 1992, 2019 }, proxyList.ToArray());

            proxyList[0] = 1993;
            Assert.AreEqual("1993", this.db["Prefix0"]);
        }

        [TestMethod]
        public void TestGenerateForList_PrimitiveList_OutOfRange()
        {
            this.db.Add("Prefix", "0");

            var proxyList = this.generator.GenerateForList<int>("Prefix");
            Assert.AreEqual(0, proxyList.Count);
            try
            {
                var elem = proxyList[0];
                Assert.Fail();
            }
            catch(IndexOutOfRangeException)
            {
            }
        }

        [TestMethod]
        public void TestGenerateForList_StringList()
        {
            this.db.Add("Prefix", "2");
            this.db.Add("Prefix0", "Tom");
            this.db.Add("Prefix1", "Jerry");

            var proxyList = this.generator.GenerateForList<string>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual("Tom", proxyList[0]);
            Assert.AreEqual("Jerry", proxyList[1]);

            CollectionAssert.AreEqual(new[] { "Tom", "Jerry" }, proxyList.ToArray());

            proxyList[0] = "Speike";
            Assert.AreEqual("Speike", this.db["Prefix0"]);
        }

        [TestMethod]
        public void TestGenerateForList_PlainObjectList()
        {
            this.db.Add("Prefix", "2");
            this.db.Add("Prefix0", "True");
            this.db.Add("Prefix0Name", "Tom");
            this.db.Add("Prefix1", "True");
            this.db.Add("Prefix1Value", "18");

            var proxyList = this.generator.GenerateForList<PlainObject>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual("Tom", proxyList[0].Name);
            Assert.AreEqual("18", proxyList[1].Value);

            proxyList[0] = new PlainObject { Name = "Speike" };
            Assert.AreEqual("Speike", this.db["Prefix0Name"]);
        }

        [TestMethod]
        public void TestGenerateForList_PlainEntityList()
        {
            this.db.Add("Prefix", "2");
            this.db.Add("Prefix0", "1");
            this.db.Add("Prefix1", "2");
            this.db.Add("PlainEntity1", "True");
            this.db.Add("PlainEntity1UserId", "1");
            this.db.Add("PlainEntity1UserName", "Tom");
            this.db.Add("PlainEntity2", "True");
            this.db.Add("PlainEntity2UserId", "2");
            this.db.Add("PlainEntity2UserName", "Jerry");

            var proxyList = this.generator.GenerateForList<PlainEntity>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual("Tom", proxyList[0].UserName);
            Assert.AreEqual("Jerry", proxyList[1].UserName);

            proxyList[0] = new PlainEntity { UserId = "3", UserName = "Speike" };
            Assert.AreEqual("3", this.db["Prefix0"]);
            Assert.AreEqual("True", this.db["PlainEntity3"]);
            Assert.AreEqual("3", this.db["PlainEntity3UserId"]);
            Assert.AreEqual("Speike", this.db["PlainEntity3UserName"]);
        }

        [TestMethod]
        public void TestGenerateForList_PrimitiveListList()
        {
            this.db.Add("Prefix", "2");
            this.db.Add("Prefix0", "2");
            this.db.Add("Prefix00", "1");
            this.db.Add("Prefix01", "2");
            this.db.Add("Prefix1", "2");
            this.db.Add("Prefix10", "3");
            this.db.Add("Prefix11", "4");

            var proxyList = this.generator.GenerateForList<IList<int>>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual(1, proxyList[0][0]);
            Assert.AreEqual(2, proxyList[0][1]);
            Assert.AreEqual(3, proxyList[1][0]);
            Assert.AreEqual(4, proxyList[1][1]);

            proxyList[1][1] = 99;
            proxyList[0][1] = 98;
            Assert.AreEqual("98", this.db["Prefix01"]);
            Assert.AreEqual("99", this.db["Prefix11"]);
        }
    }
}
