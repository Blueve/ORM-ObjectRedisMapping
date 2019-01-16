namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test the <see cref="TypeRepository"/>.
    /// </summary>
    [TestClass]
    public class TypeRepositoryTests
    {
        private TypeRepository repo;

        [TestInitialize]
        public void Initialize()
        {
            this.repo = new TypeRepository(new TypeMetadataGenerator());
        }

        [TestMethod]
        public void TestRegister_Primitive()
        {
            this.repo.Register(typeof(int));
            var metadata = this.repo.Get(typeof(int));

            Assert.AreEqual(ObjectValueType.Primitive, metadata.ValueType);
        }

        [TestMethod]
        public void TestRegister_String()
        {
            this.repo.Register(typeof(string));
            var metadata = this.repo.Get(typeof(string));

            Assert.AreEqual(ObjectValueType.String, metadata.ValueType);
        }

        [TestMethod]
        public void TestRegister_PlainEntity()
        {
            this.repo.Register(typeof(PlainEntity));
            var metadata = this.repo.Get(typeof(PlainEntity));

            Assert.AreEqual(ObjectValueType.Entity, metadata.ValueType);
        }

        [TestMethod]
        public void TestRegister_PlainObject()
        {
            this.repo.Register(typeof(PlainObject));
            var metadata = this.repo.Get(typeof(PlainObject));

            Assert.AreEqual(ObjectValueType.Object, metadata.ValueType);
        }

        [TestMethod]
        public void TestRegister_KeyIsEntity_InvalidEntity()
        {
            try
            {
                this.repo.Register(typeof(KeyIsEntity_InvalidEntity));
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }
    }
}
