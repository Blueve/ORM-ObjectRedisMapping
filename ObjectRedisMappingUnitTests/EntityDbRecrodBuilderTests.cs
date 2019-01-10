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

            // TODO: Find a better way to compare unordered result.
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity000000011UserId", records[1].Key);
            Assert.AreEqual("1", records[1].Value.Object);
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity000000011UserName", records[0].Key);
            Assert.AreEqual("Blueve", records[0].Value.Object);
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
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity000000011Key", records[1].Key);
            Assert.AreEqual("1", records[1].Value.Object);
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity000000011LeftChild", records[0].Key);
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity000000012", records[0].Value.Object);
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
            Assert.AreEqual("PrefixValue", records[0].Key);
            Assert.AreEqual("26", records[0].Value.Object);
            Assert.AreEqual("PrefixName", records[1].Key);
            Assert.AreEqual("Age", records[1].Value.Object);
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
            Assert.AreEqual("PrefixChildName", records[0].Key);
            Assert.AreEqual("Unknown", records[0].Value.Object);
            Assert.AreEqual("PrefixName", records[1].Key);
            Assert.AreEqual("Blueve", records[1].Value.Object);
        }
    }
}
