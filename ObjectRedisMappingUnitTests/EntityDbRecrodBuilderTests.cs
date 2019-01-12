namespace Blueve.ObjectRedisMapping.UnitTests
{
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
            var typeRepo = TypeRepository.CreateInstance();
            this.builder = new DbRecordBuilder(typeRepo, new EntityKeyGenerator(new EntityKeyValueFormatter()));
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
                new DbRecord("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity000000011UserId", new DbValue(DbValueType.String, "1")),
                new DbRecord("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity000000011UserName", new DbValue(DbValueType.String, "Blueve"))
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
                new DbRecord("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity000000011Key", new DbValue(DbValueType.String, "1")),
                new DbRecord("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity000000011LeftChild", new DbValue(DbValueType.String, "Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity000000012"))
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
                new DbRecord("PrefixName", new DbValue(DbValueType.String, "Blueve")),
                new DbRecord("PrefixChildName", new DbValue(DbValueType.String, "Unknown"))
            }, records);
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
    }
}
