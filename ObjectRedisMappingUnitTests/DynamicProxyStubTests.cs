namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Blueve.RedisEmulator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using StackExchange.Redis;

    /// <summary>
    /// Test the <see cref="DynamicProxyStub"/>.
    /// </summary>
    [TestClass]
    public class DynamicProxyStubTests
    {
        private RedisDatabase db;
        private DbRecordBuilder dbRecordBuilder;
        private EntityKeyGenerator keyGenerator;
        private DynamicProxyStub stub;

        [TestInitialize]
        public void Initialize()
        {
            this.db = new RedisDatabase();

            var typeRepo = new TypeRepository(new TypeMetadataGenerator(false));

            this.keyGenerator = new EntityKeyGenerator();
            this.dbRecordBuilder = new DbRecordBuilder(typeRepo, this.keyGenerator);
            this.stub = new DynamicProxyStub(typeRepo, this.db, this.dbRecordBuilder, this.keyGenerator);
        }

        [TestMethod]
        public void TestStringGetter()
        {
            this.db.StringSet("Key", "Value");
            Assert.AreEqual("Value", this.stub.StringGetter("Key"));
        }

        [TestMethod]
        public void TestStringGetter_Null()
        {
            Assert.IsNull(this.stub.StringGetter("Key"));
        }

        [TestMethod]
        public void TestStringSetter()
        {
            this.stub.StringSetter("Key", "Value");
            Assert.AreEqual("Value", this.db.StringGet("Key").ToString());
        }

        [TestMethod]
        public void TestInt16Getter()
        {
            this.db.StringSet("Key", "1");
            var result = this.stub.Int16Getter("Key");
            Assert.AreEqual(typeof(short), result.GetType());
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestInt16Getter_Default()
        {
            Assert.AreEqual(default, this.stub.Int16Getter("Key"));
        }

        [TestMethod]
        public void TestInt16Setter()
        {
            this.stub.Int16Setter("Key", 1);
            Assert.AreEqual("1", this.db.StringGet("Key").ToString());
        }

        [TestMethod]
        public void TestInt32Getter()
        {
            this.db.StringSet("Key", "1");
            var result = this.stub.Int32Getter("Key");
            Assert.AreEqual(typeof(int), result.GetType());
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestInt32Getter_Default()
        {
            Assert.AreEqual(default, this.stub.Int32Getter("Key"));
        }

        [TestMethod]
        public void TestInt32Setter()
        {
            this.stub.Int32Setter("Key", 1);
            Assert.AreEqual("1", this.db.StringGet("Key").ToString());
        }

        [TestMethod]
        public void TestInt64Getter()
        {
            this.db.StringSet("Key", "1");
            var result = this.stub.Int64Getter("Key");
            Assert.AreEqual(typeof(long), result.GetType());
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestInt64Getter_Default()
        {
            Assert.AreEqual(default, this.stub.Int64Getter("Key"));
        }

        [TestMethod]
        public void TestInt64Setter()
        {
            this.stub.Int64Setter("Key", 1);
            Assert.AreEqual("1", this.db.StringGet("Key").ToString());
        }

        [TestMethod]
        public void TestEntityGetter_PlainEntity_KeyNotExists()
        {
            var proxy = this.stub.EntityGetter<PlainEntity>("Key");
            Assert.IsNull(proxy);
        }

        [TestMethod]
        public void TestEntityGetter_PlainEntity_KeyExists()
        {
            this.db.StringSet("DbKey", "Value");
            this.db.StringSet("PlainEntityValue", "True");

            var proxy = this.stub.EntityGetter<PlainEntity>("DbKey");
            Assert.IsTrue(proxy is PlainEntity);
        }

        [TestMethod]
        public void TestEntitySetter_PlainEntity_NormalObject()
        {
            var obj = new PlainEntity
            {
                UserId = "1",
                UserName = "Blueve"
            };

            this.stub.EntitySetter("DbKey", obj);

            Assert.AreEqual("1", this.db.StringGet("DbKey").ToString());
            Assert.AreEqual("Blueve", this.db.StringGet("PlainEntity1UserName").ToString());
        }

        [TestMethod]
        public void TestEntitySetter_PlainEntity_ProxyObject()
        {
            var obj = new ProxyPlainEntity
            {
                UserId = "1",
                UserName = "Blueve"
            };

            this.stub.EntitySetter<PlainEntity>("DbKey", obj);
            Assert.AreEqual("1", this.db.StringGet("DbKey").ToString());
        }

        [TestMethod]
        public void TestObjectGetter_PlainObject()
        {
            this.db.StringSet("DbKey", "Value");
            this.db.StringSet("DbKeyName", "Blueve");

            var proxy = this.stub.ObjectGetter<PlainObject>("DbKey");
            Assert.AreEqual("Blueve", proxy.Name);
        }

        [TestMethod]
        public void TestObjectGetter_PlainObject_Null()
        {
            var proxy = this.stub.ObjectGetter<PlainObject>("DbKey");
            Assert.IsNull(proxy);
        }

        [TestMethod]
        public void TestReadonlyObjectGetter_PlainObject()
        {
            this.db.StringSet("DbKey", "True");
            this.db.StringSet("DbKeyName", "Blueve");

            var proxy = this.stub.ReadOnlyObjectGetter<PlainObject>("DbKey");
            try
            {
                proxy.Name = "Ada";
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void TestReadonlyObjectGetter_PlainObject_Null()
        {
            var proxy = this.stub.ReadOnlyObjectGetter<PlainObject>("DbKey");
            Assert.IsNull(proxy);
        }

        [TestMethod]
        public void TestObjectSetter_PlainObject()
        {
            var obj = new PlainObject
            {
                Name = "UserName",
                Value = "Blueve"
            };

            this.stub.ObjectSetter("DbKey", obj);
            Assert.AreEqual("UserName", this.db.StringGet("DbKeyName").ToString());
            Assert.AreEqual("Blueve", this.db.StringGet("DbKeyValue").ToString());
        }

        [TestMethod]
        public void TestListGetter_PrimitiveList()
        {
            this.db.StringSet("DbKey", "2");
            this.db.StringSet("DbKey0", "1992");
            this.db.StringSet("DbKey1", "2019");

            var list = this.stub.ListGetter<int>("DbKey");
            CollectionAssert.AreEqual(new[] { 1992, 2019 }, list.ToArray());
        }

        [TestMethod]
        public void TestReadOnlyListGetter_PrimitiveList()
        {
            this.db.StringSet("DbKey", "2");
            this.db.StringSet("DbKey0", "1992");
            this.db.StringSet("DbKey1", "2019");

            var list = this.stub.ReadOnlyListGetter<int>("DbKey");
            CollectionAssert.AreEqual(new[] { 1992, 2019 }, list.ToArray());

            try
            {
                list[0] = 1993;
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                list.Add(1993);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void TestListSetter_PrimitiveList()
        {
            IList<int> list = new[] { 1992, 2019 };

            this.stub.ListSetter<IList<int>, int>("DbKey", list);
            Assert.AreEqual("2", this.db.StringGet("DbKey").ToString());
            Assert.AreEqual("1992", this.db.StringGet("DbKey0").ToString());
            Assert.AreEqual("2019", this.db.StringGet("DbKey1").ToString());
        }

        [TestMethod]
        public void TestListSetter_PrimitiveList_Array()
        {
            var list = new[] { 1992, 2019 };

            this.stub.ListSetter<IList<int>, int>("DbKey", list);
            Assert.AreEqual("2", this.db.StringGet("DbKey").ToString());
            Assert.AreEqual("1992", this.db.StringGet("DbKey0").ToString());
            Assert.AreEqual("2019", this.db.StringGet("DbKey1").ToString());
        }
    }
}
