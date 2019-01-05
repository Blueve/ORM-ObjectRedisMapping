namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System.Linq;
    using Blueve.ObjectRedisMapping;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test the <see cref="EntityDbRecordBuilder"/>.
    /// </summary>
    [TestClass]
    public class EntityDbRecrodBuilderTests
    {
        [TestMethod]
        public void TestGenerate()
        {
            var typeRepo = TypeRepository.CreateInstance();
            var builder = new EntityDbRecordBuilder(typeRepo, new EntityKeyGenerator(new EntityKeyValueFormatter()));

            var entity = new PlainEntity
            {
                UserId = "1",
                UserName = "Blueve"
            };
            var records = builder.Generate(entity).ToArray();

            // TODO: Find a better way to compare unordered result.
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity000000011UserId", records[1].Key);
            Assert.AreEqual("1", records[1].Value.Object);
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity000000011UserName", records[0].Key);
            Assert.AreEqual("Blueve", records[0].Value.Object);

            var nestedEntity = new NestedEntity
            {
                Key = "1",
                LeftChild = new NestedEntity
                {
                    Key = "2"
                }
            };
            records = builder.Generate(nestedEntity).ToArray();
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity000000011Key", records[1].Key);
            Assert.AreEqual("1", records[1].Value.Object);
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity000000011LeftChild", records[0].Key);
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.NestedEntity000000012", records[0].Value.Object);
        }
    }
}
