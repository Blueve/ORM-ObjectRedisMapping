namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blueve.ObjectRedisMapping;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Blueve.RedisEmulator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using StackExchange.Redis;

    /// <summary>
    /// Test the <see cref="DynamicProxyGenerator"/>.
    /// </summary>
    [TestClass]
    public class DynamicProxyGeneratorTests
    {
        private RedisDatabase db;
        private DbRecordBuilder dbRecordBuilder;
        private EntityKeyGenerator keyGenerator;
        private DynamicProxyStub stub;
        private DynamicProxyGenerator generator;

        [TestInitialize]
        public void Initialize()
        {
            this.db = new RedisDatabase();

            var typeRepo = new TypeRepository(new TypeMetadataGenerator(false));

            this.keyGenerator = new EntityKeyGenerator();
            this.dbRecordBuilder = new DbRecordBuilder(typeRepo, this.keyGenerator);
            this.stub = new DynamicProxyStub(typeRepo, this.db, this.dbRecordBuilder, this.keyGenerator);
            this.generator = new DynamicProxyGenerator(typeRepo, this.keyGenerator, this.stub, this.db);
        }

        [TestMethod]
        public void TestGenerateForEntity_PlainEntity()
        {
            this.db.StringSet("PlainEntityBlueve", "True");
            this.db.StringSet("PlainEntityBlueveUserId", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<PlainEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.UserId);

            proxyObj.UserName = "Blueve";
            Assert.AreEqual("Blueve", this.db.StringGet("PlainEntityBlueveUserName").ToString());
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
            this.db.StringSet("NestedEntityBlueve", "True");
            this.db.StringSet("NestedEntityBlueveKey", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<NestedEntity>("Blueve");
            Assert.AreEqual("Blueve", proxyObj.Key);

            proxyObj.LeftChild = new NestedEntity
            {
                Key = "Youd"
            };
            Assert.AreEqual("Youd", proxyObj.LeftChild.Key);
            Assert.AreEqual("Youd", this.db.StringGet("NestedEntityBlueveLeftChild").ToString());
            Assert.AreEqual("Youd", this.db.StringGet("NestedEntityYoudKey").ToString());
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
            this.db.StringSet("Prefix", "True");
            this.db.StringSet("PrefixName", "Age");
            this.db.StringSet("PrefixValue", "18");

            var proxyObj = this.generator.GenerateForObject<PlainObject>("Prefix");
            Assert.AreEqual("Age", proxyObj.Name);
            Assert.AreEqual("18", proxyObj.Value);

            proxyObj.Value = "19";
            Assert.AreEqual("19", this.db.StringGet("PrefixValue").ToString());
        }

        [TestMethod]
        public void TestGenerateForObject_NestedObject()
        {
            this.db.StringSet("Prefix", "True");
            this.db.StringSet("PrefixName", "Blueve");
            this.db.StringSet("PrefixChild", "True");
            this.db.StringSet("PrefixChildName", "Youd");

            var proxyObj = this.generator.GenerateForObject<NestedObject>("Prefix");
            Assert.AreEqual("Blueve", proxyObj.Name);
            Assert.AreEqual("Youd", proxyObj.Child.Name);

            proxyObj.Child = new NestedObject
            {
                Name = "Ada"
            };
            Assert.AreEqual("Ada", this.db.StringGet("PrefixChildName").ToString());
        }

        [TestMethod]
        public void TestGenerateForEntity_PlainEntity_OverrideKey()
        {
            this.db.StringSet("PlainEntityBlueve", "True");
            this.db.StringSet("PlainEntityBlueveUserId", "1");

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
            this.db.StringSet("KeyIsObject_UseInterface_EntityKeyBlueve", "True");
            this.db.StringSet("KeyIsObject_UseInterface_EntityKeyBlueveKey", "True");
            this.db.StringSet("KeyIsObject_UseInterface_EntityKeyBlueveKeyValue", "Blueve");

            var proxyObj = this.generator.GenerateForEntity<KeyIsObject_UseInterface_Entity>("KeyBlueve");
            Assert.AreEqual("Blueve", proxyObj.Key.Value);
        }

        [TestMethod]
        public void TestGenerateForEntity_KeyIsObject_UseInterface_Entity_UpdateKeyValue()
        {
            this.db.StringSet("KeyIsObject_UseInterface_EntityKeyBlueve", "True");
            this.db.StringSet("KeyIsObject_UseInterface_EntityKeyBlueveKey", "True");
            this.db.StringSet("KeyIsObject_UseInterface_EntityKeyBlueveKeyValue", "Blueve");

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
            this.db.StringSet("ListNodeEntity1", "True");
            this.db.StringSet("ListNodeEntity1Id", "1");

            var proxyObj = this.generator.GenerateForEntity<ListNodeEntity>("1");
            Assert.AreEqual(1, proxyObj.Id);

            proxyObj.Val = 1;
            Assert.AreEqual(1, proxyObj.Val);
            Assert.AreEqual("1", this.db.StringGet("ListNodeEntity1Val").ToString());

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

            Assert.AreEqual("2", this.db.StringGet("ListNodeEntity1Next").ToString());
            Assert.AreEqual("2", this.db.StringGet("ListNodeEntity2Val").ToString());
            Assert.AreEqual("3", this.db.StringGet("ListNodeEntity2Next").ToString());
            Assert.AreEqual("3", this.db.StringGet("ListNodeEntity3Val").ToString());
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
            this.db.StringSet("Prefix", "2");
            this.db.StringSet("Prefix0", "1992");
            this.db.StringSet("Prefix1", "2019");

            var proxyList = this.generator.GenerateForList<int>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual(1992, proxyList[0]);
            Assert.AreEqual(2019, proxyList[1]);

            CollectionAssert.AreEqual(new[] { 1992, 2019 }, proxyList.ToArray());

            proxyList[0] = 1993;
            Assert.AreEqual("1993", this.db.StringGet("Prefix0").ToString());
        }

        [TestMethod]
        public void TestGenerateForList_PrimitiveList_OutOfRange()
        {
            this.db.StringSet("Prefix", "0");

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
            this.db.StringSet("Prefix", "2");
            this.db.StringSet("Prefix0", "Tom");
            this.db.StringSet("Prefix1", "Jerry");

            var proxyList = this.generator.GenerateForList<string>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual("Tom", proxyList[0]);
            Assert.AreEqual("Jerry", proxyList[1]);

            CollectionAssert.AreEqual(new[] { "Tom", "Jerry" }, proxyList.ToArray());

            proxyList[0] = "Speike";
            Assert.AreEqual("Speike", this.db.StringGet("Prefix0").ToString());
        }

        [TestMethod]
        public void TestGenerateForList_PlainObjectList()
        {
            this.db.StringSet("Prefix", "2");
            this.db.StringSet("Prefix0", "True");
            this.db.StringSet("Prefix0Name", "Tom");
            this.db.StringSet("Prefix1", "True");
            this.db.StringSet("Prefix1Value", "18");

            var proxyList = this.generator.GenerateForList<PlainObject>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual("Tom", proxyList[0].Name);
            Assert.AreEqual("18", proxyList[1].Value);

            proxyList[0] = new PlainObject { Name = "Speike" };
            Assert.AreEqual("Speike", this.db.StringGet("Prefix0Name").ToString());
        }

        [TestMethod]
        public void TestGenerateForList_PlainEntityList()
        {
            this.db.StringSet("Prefix", "2");
            this.db.StringSet("Prefix0", "1");
            this.db.StringSet("Prefix1", "2");
            this.db.StringSet("PlainEntity1", "True");
            this.db.StringSet("PlainEntity1UserId", "1");
            this.db.StringSet("PlainEntity1UserName", "Tom");
            this.db.StringSet("PlainEntity2", "True");
            this.db.StringSet("PlainEntity2UserId", "2");
            this.db.StringSet("PlainEntity2UserName", "Jerry");

            var proxyList = this.generator.GenerateForList<PlainEntity>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual("Tom", proxyList[0].UserName);
            Assert.AreEqual("Jerry", proxyList[1].UserName);

            proxyList[0] = new PlainEntity { UserId = "3", UserName = "Speike" };
            Assert.AreEqual("3", this.db.StringGet("Prefix0").ToString());
            Assert.AreEqual("True", this.db.StringGet("PlainEntity3").ToString());
            Assert.AreEqual("3", this.db.StringGet("PlainEntity3UserId").ToString());
            Assert.AreEqual("Speike", this.db.StringGet("PlainEntity3UserName").ToString());
        }

        [TestMethod]
        public void TestGenerateForList_PrimitiveListList()
        {
            this.db.StringSet("Prefix", "2");
            this.db.StringSet("Prefix0", "2");
            this.db.StringSet("Prefix00", "1");
            this.db.StringSet("Prefix01", "2");
            this.db.StringSet("Prefix1", "2");
            this.db.StringSet("Prefix10", "3");
            this.db.StringSet("Prefix11", "4");

            var proxyList = this.generator.GenerateForList<IList<int>>("Prefix");
            Assert.AreEqual(2, proxyList.Count);
            Assert.AreEqual(1, proxyList[0][0]);
            Assert.AreEqual(2, proxyList[0][1]);
            Assert.AreEqual(3, proxyList[1][0]);
            Assert.AreEqual(4, proxyList[1][1]);

            proxyList[1][1] = 99;
            proxyList[0][1] = 98;
            Assert.AreEqual("98", this.db.StringGet("Prefix01").ToString());
            Assert.AreEqual("99", this.db.StringGet("Prefix11").ToString());
        }
    }
}
