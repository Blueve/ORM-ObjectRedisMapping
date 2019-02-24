namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test the <see cref="EntityKeyGenerator"/>.
    /// </summary>
    [TestClass]
    public class EntityKeyGeneratorTests
    {
        private TypeRepository typeRepo;

        [TestInitialize]
        public void Initialize()
        {
            this.typeRepo = new TypeRepository(new TypeMetadataGenerator(false));
        }

        [TestMethod]
        public void TestGetEntityKey_PlainEntity()
        {
            var typeMetadata = this.typeRepo.GetOrRegister(typeof(PlainEntity)) as EntityMetadata;
            var key = EntityKeyGenerator.GetEntityKey(typeMetadata, new PlainEntity
            {
                UserId = "5"
            });
            Assert.AreEqual("5", key);
        }

        [TestMethod]
        public void TestGetEntityKey_KeyIsObject_UseInterface_Entity()
        {
            var typeMetadata = this.typeRepo.GetOrRegister(typeof(KeyIsObject_UseInterface_Entity)) as EntityMetadata;
            var key = EntityKeyGenerator.GetEntityKey(typeMetadata, new KeyIsObject_UseInterface_Entity
            {
                Key = new ObjectEntityKey
                {
                    Value = "5"
                }
            });
            Assert.AreEqual("Key5", key);
        }

        [TestMethod]
        public void TestGetEntityKey_KeyIsObject_UseInterface_NotImplement_Entity()
        {
            var typeMetadata = this.typeRepo.GetOrRegister(typeof(KeyIsObject_UseInterface_NotImplement_Entity)) as EntityMetadata;

            try
            {
                var key = EntityKeyGenerator.GetEntityKey(typeMetadata, new KeyIsObject_UseInterface_NotImplement_Entity
                {
                    Key = new PlainObject
                    {
                        Name = "Blueve"
                    }
                });
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }
    }
}
