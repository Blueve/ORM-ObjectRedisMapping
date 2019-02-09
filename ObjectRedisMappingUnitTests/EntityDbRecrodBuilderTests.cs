namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Linq;
    using Blueve.ObjectRedisMapping;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test the <see cref="DbRecordBuilder"/>.
    /// </summary>
    [TestClass]
    public class EntityDbRecrodBuilderTests
    {
        private DbRecordBuilder builder;

        [TestInitialize]
        public void Initialize()
        {
            var typeRepo = new TypeRepository(new TypeMetadataGenerator(false));
            this.builder = new DbRecordBuilder(typeRepo, new EntityKeyGenerator());
        }

        [TestMethod]
        public void TestGenerate_PlainEntity()
        {
            var entity = new PlainEntity
            {
                UserId = "1",
                UserName = "Blueve"
            };
            var records = this.builder.Generate(entity).ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                new DbRecord("PlainEntity1", new DbValue(DbValueType.String, "True")),
                new DbRecord("PlainEntity1UserId", new DbValue(DbValueType.String, "1")),
                new DbRecord("PlainEntity1UserName", new DbValue(DbValueType.String, "Blueve"))
            }, records);
        }

        [TestMethod]
        public void TestGenerate_NestedEntity()
        {
            var nestedEntity = new NestedEntity
            {
                Key = "1",
                LeftChild = new NestedEntity
                {
                    Key = "2"
                }
            };
            var records = this.builder.Generate(nestedEntity).ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                new DbRecord("NestedEntity1", new DbValue(DbValueType.String, "True")),
                new DbRecord("NestedEntity1Key", new DbValue(DbValueType.String, "1")),
                new DbRecord("NestedEntity1LeftChild", new DbValue(DbValueType.String, "2")),
                new DbRecord("NestedEntity2", new DbValue(DbValueType.String, "True")),
                new DbRecord("NestedEntity2Key", new DbValue(DbValueType.String, "2"))
            }, records);
        }

        [TestMethod]
        public void TestGenerate_NestedEntity_CircularReference()
        {
            var nestedEntity = new NestedEntity
            {
                Key = "1",
                LeftChild = new NestedEntity
                {
                    Key = "2"
                }
            };
            nestedEntity.LeftChild.LeftChild = nestedEntity;
            var records = this.builder.Generate(nestedEntity).ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                new DbRecord("NestedEntity1", new DbValue(DbValueType.String, "True")),
                new DbRecord("NestedEntity1Key", new DbValue(DbValueType.String, "1")),
                new DbRecord("NestedEntity1LeftChild", new DbValue(DbValueType.String, "2")),
                new DbRecord("NestedEntity2", new DbValue(DbValueType.String, "True")),
                new DbRecord("NestedEntity2Key", new DbValue(DbValueType.String, "2")),
                new DbRecord("NestedEntity2LeftChild", new DbValue(DbValueType.String, "1"))
            }, records);
        }

        [TestMethod]
        public void TestGenerate_PlainObject()
        {
            var obj = new PlainObject
            {
                Name = "Age",
                Value = "26"
            };
            var records = this.builder.Generate(obj, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                new DbRecord("Prefix", new DbValue(DbValueType.String, "True")),
                new DbRecord("PrefixName", new DbValue(DbValueType.String, "Age")),
                new DbRecord("PrefixValue", new DbValue(DbValueType.String, "26"))
            }, records);
        }

        [TestMethod]
        public void TestGenerate_NestedObject()
        {
            var obj = new NestedObject
            {
                Name = "Blueve",
                Child = new NestedObject
                {
                    Name = "Unknown"
                }
            };
            var records = this.builder.Generate(obj, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                new DbRecord("Prefix", new DbValue(DbValueType.String, "True")),
                new DbRecord("PrefixName", new DbValue(DbValueType.String, "Blueve")),
                new DbRecord("PrefixChild", new DbValue(DbValueType.String, "True")),
                new DbRecord("PrefixChildName", new DbValue(DbValueType.String, "Unknown"))
            }, records);
        }

        [TestMethod]
        public void TestGenerate_NestedObject_CircularReference()
        {
            var obj = new NestedObject
            {
                Name = "Blueve",
                Child = new NestedObject
                {
                    Name = "Unknown"
                }
            };
            obj.Child.Child = obj;

            try
            {
                this.builder.Generate(obj).ToArray();
                Assert.Fail();
            }
            catch (NotSupportedException)
            {
            }
        }

        [DataTestMethod]
        [DataRow("Blueve", "Prefix", "Blueve")]
        [DataRow("Ada", "Prefix", "Ada")]
        public void TestGenerate_String(string value, string expectedKey, string expectedValue)
        {
            var records = this.builder.Generate(value, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                DbRecord.GenerateStringRecord(expectedKey, expectedValue)
            }, records);
        }

        [DataTestMethod]
        [DataRow(0, "Prefix", "0")]
        [DataRow(-1, "Prefix", "-1")]
        [DataRow(128, "Prefix", "128")]
        public void TestGenerate_Int32(int value, string expectedKey, string expectedValue)
        {
            var records = this.builder.Generate(value, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                DbRecord.GenerateStringRecord(expectedKey, expectedValue)
            }, records);
        }

        [TestMethod]
        public void TestGenerate_PrimitiveArray()
        {
            var elems = new[] { 1, 9, 9, 2 };
            var records = this.builder.Generate(elems, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                DbRecord.GenerateStringRecord("Prefix", 4.ToString()),
                DbRecord.GenerateStringRecord("Prefix0", "1"),
                DbRecord.GenerateStringRecord("Prefix1", "9"),
                DbRecord.GenerateStringRecord("Prefix2", "9"),
                DbRecord.GenerateStringRecord("Prefix3", "2"),
            }, records);
        }

        [TestMethod]
        public void TestGenerate_StringArray()
        {
            var elems = new[] { "1", "9", "9", "2" };
            var records = this.builder.Generate(elems, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                DbRecord.GenerateStringRecord("Prefix", 4.ToString()),
                DbRecord.GenerateStringRecord("Prefix0", "1"),
                DbRecord.GenerateStringRecord("Prefix1", "9"),
                DbRecord.GenerateStringRecord("Prefix2", "9"),
                DbRecord.GenerateStringRecord("Prefix3", "2"),
            }, records);
        }

        [TestMethod]
        public void TestGenerate_PlainObjectArray()
        {
            var elems = new[] { new PlainObject { Name = "Tom", Value = "1" }, new PlainObject { Name = "Jerry", Value = "2" } };
            var records = this.builder.Generate(elems, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                DbRecord.GenerateStringRecord("Prefix", 2.ToString()),
                DbRecord.GenerateStringRecord("Prefix0", "True"),
                DbRecord.GenerateStringRecord("Prefix0Name", "Tom"),
                DbRecord.GenerateStringRecord("Prefix0Value", "1"),
                DbRecord.GenerateStringRecord("Prefix1", "True"),
                DbRecord.GenerateStringRecord("Prefix1Name", "Jerry"),
                DbRecord.GenerateStringRecord("Prefix1Value", "2"),
            }, records);
        }

        [TestMethod]
        public void TestGenerate_PlainEntityArray()
        {
            var elems = new[] { new PlainEntity { UserId = "1" }, new PlainEntity { UserId = "2" } };
            var records = this.builder.Generate(elems, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                DbRecord.GenerateStringRecord("Prefix", 2.ToString()),
                DbRecord.GenerateStringRecord("Prefix0", "1"),
                DbRecord.GenerateStringRecord("Prefix1", "2"),
                DbRecord.GenerateStringRecord("PlainEntity1", "True"),
                DbRecord.GenerateStringRecord("PlainEntity1UserId", "1"),
                DbRecord.GenerateStringRecord("PlainEntity2", "True"),
                DbRecord.GenerateStringRecord("PlainEntity2UserId", "2"),
            }, records);
        }
    }
}
