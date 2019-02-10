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
    public class DbRecordBuilderTests
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
                new DbStringRecord("PlainEntity1", "True"),
                new DbStringRecord("PlainEntity1UserId", "1"),
                new DbStringRecord("PlainEntity1UserName", "Blueve")
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
                new DbStringRecord("NestedEntity1", "True"),
                new DbStringRecord("NestedEntity1Key", "1"),
                new DbStringRecord("NestedEntity1LeftChild", "2"),
                new DbStringRecord("NestedEntity2", "True"),
                new DbStringRecord("NestedEntity2Key", "2")
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
                new DbStringRecord("NestedEntity1", "True"),
                new DbStringRecord("NestedEntity1Key", "1"),
                new DbStringRecord("NestedEntity1LeftChild", "2"),
                new DbStringRecord("NestedEntity2", "True"),
                new DbStringRecord("NestedEntity2Key", "2"),
                new DbStringRecord("NestedEntity2LeftChild", "1")
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
                new DbStringRecord("Prefix", "True"),
                new DbStringRecord("PrefixName", "Age"),
                new DbStringRecord("PrefixValue", "26")
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
                new DbStringRecord("Prefix", "True"),
                new DbStringRecord("PrefixName", "Blueve"),
                new DbStringRecord("PrefixChild", "True"),
                new DbStringRecord("PrefixChildName", "Unknown")
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
                new DbStringRecord(expectedKey, expectedValue)
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
                new DbStringRecord(expectedKey, expectedValue)
            }, records);
        }

        [TestMethod]
        public void TestGenerate_PrimitiveArray()
        {
            var elems = new[] { 1, 9, 9, 2 };
            var records = this.builder.Generate(elems, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                new DbStringRecord("Prefix", 4.ToString()),
                new DbStringRecord("Prefix0", "1"),
                new DbStringRecord("Prefix1", "9"),
                new DbStringRecord("Prefix2", "9"),
                new DbStringRecord("Prefix3", "2"),
            }, records);
        }

        [TestMethod]
        public void TestGenerate_StringArray()
        {
            var elems = new[] { "1", "9", "9", "2" };
            var records = this.builder.Generate(elems, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                new DbStringRecord("Prefix", 4.ToString()),
                new DbStringRecord("Prefix0", "1"),
                new DbStringRecord("Prefix1", "9"),
                new DbStringRecord("Prefix2", "9"),
                new DbStringRecord("Prefix3", "2"),
            }, records);
        }

        [TestMethod]
        public void TestGenerate_PlainObjectArray()
        {
            var elems = new[] { new PlainObject { Name = "Tom", Value = "1" }, new PlainObject { Name = "Jerry", Value = "2" } };
            var records = this.builder.Generate(elems, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                new DbStringRecord("Prefix", 2.ToString()),
                new DbStringRecord("Prefix0", "True"),
                new DbStringRecord("Prefix0Name", "Tom"),
                new DbStringRecord("Prefix0Value", "1"),
                new DbStringRecord("Prefix1", "True"),
                new DbStringRecord("Prefix1Name", "Jerry"),
                new DbStringRecord("Prefix1Value", "2"),
            }, records);
        }

        [TestMethod]
        public void TestGenerate_PlainEntityArray()
        {
            var elems = new[] { new PlainEntity { UserId = "1" }, new PlainEntity { UserId = "2" } };
            var records = this.builder.Generate(elems, "Prefix").ToArray();

            CollectionAssert.AreEquivalent(new[]
            {
                new DbStringRecord("Prefix", 2.ToString()),
                new DbStringRecord("Prefix0", "1"),
                new DbStringRecord("Prefix1", "2"),
                new DbStringRecord("PlainEntity1", "True"),
                new DbStringRecord("PlainEntity1UserId", "1"),
                new DbStringRecord("PlainEntity2", "True"),
                new DbStringRecord("PlainEntity2UserId", "2"),
            }, records);
        }
    }
}
