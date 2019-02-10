namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test the <see cref="TypeMetadataGenerator"/>.
    /// </summary>
    [TestClass]
    public class TypeMetadataGeneratorTests
    {
        [TestMethod]
        public void TestGenerate_FullName_PlainEntity()
        {
            var generator = new TypeMetadataGenerator(true);
            var metadata = generator.Generate(typeof(PlainEntity)) as EntityMetadata;
            Assert.AreEqual("UserId", metadata.KeyProperty.Name);
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity", metadata.Name);
        }

        [TestMethod]
        public void TestGenerate_Name_PlainEntity()
        {
            var generator = new TypeMetadataGenerator(false);
            var metadata = generator.Generate(typeof(PlainEntity)) as EntityMetadata;
            Assert.AreEqual("UserId", metadata.KeyProperty.Name);
            Assert.AreEqual("PlainEntity", metadata.Name);
        }

        [TestMethod]
        public void TestGenerate_PrimitiveArray()
        {
            var generator = new TypeMetadataGenerator(false);
            var metadata = generator.Generate(typeof(int[])) as ListMetadata;
            Assert.AreEqual(typeof(int), metadata.InnerType);
            Assert.IsTrue(metadata.ReadOnly);
        }

        [TestMethod]
        public void TestGenerate_PlainObjectArray()
        {
            var generator = new TypeMetadataGenerator(false);
            var metadata = generator.Generate(typeof(PlainObject[])) as ListMetadata;
            Assert.AreEqual(typeof(PlainObject), metadata.InnerType);
            Assert.IsTrue(metadata.ReadOnly);
        }

        [TestMethod]
        public void TestGenerate_PrimitiveList()
        {
            var generator = new TypeMetadataGenerator(false);
            var metadata = generator.Generate(typeof(IList<int>)) as ListMetadata;
            Assert.AreEqual(typeof(int), metadata.InnerType);
            Assert.IsFalse(metadata.ReadOnly);
        }

        [TestMethod]
        public void TestGenerate_PlainObjectList()
        {
            var generator = new TypeMetadataGenerator(false);
            var metadata = generator.Generate(typeof(IList<PlainObject>)) as ListMetadata;
            Assert.AreEqual(typeof(PlainObject), metadata.InnerType);
            Assert.IsFalse(metadata.ReadOnly);
        }
    }
}
