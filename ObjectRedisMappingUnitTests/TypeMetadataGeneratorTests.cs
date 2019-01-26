namespace Blueve.ObjectRedisMapping.UnitTests
{
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
            var metadata = generator.Generate(typeof(PlainEntity));
            Assert.AreEqual("UserId", metadata.KeyProperty.Name);
            Assert.AreEqual("Blueve.ObjectRedisMapping.UnitTests.Model.PlainEntity", metadata.Name);
        }

        [TestMethod]
        public void TestGenerate_Name_PlainEntity()
        {
            var generator = new TypeMetadataGenerator(false);
            var metadata = generator.Generate(typeof(PlainEntity));
            Assert.AreEqual("UserId", metadata.KeyProperty.Name);
            Assert.AreEqual("PlainEntity", metadata.Name);
        }
    }
}
