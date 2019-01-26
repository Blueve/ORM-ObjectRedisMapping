namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test the <see cref="EntityKeyGenerator"/>.
    /// </summary>
    [TestClass]
    public class EntityKeyGeneratorTests
    {
        private TypeRepository typeRepo;
        private EntityKeyGenerator generator;

        [TestInitialize]
        public void Initialize()
        {
            this.typeRepo = new TypeRepository(new TypeMetadataGenerator(false));
            this.generator = new EntityKeyGenerator();
        }

        [TestMethod]
        public void TestGetEntityKey_PlainEntity()
        {
            var typeMetadata = this.typeRepo.GetOrRegister(typeof(PlainEntity)) as EntityTypeMetadata;
            var key = this.generator.GetEntityKey(typeMetadata, new PlainEntity
            {
                UserId = "5"
            });
            Assert.AreEqual("5", key);
        }

        [TestMethod]
        public void TestGetEntityKey_KeyIsObject_UseInterface_Entity()
        {
            var typeMetadata = this.typeRepo.GetOrRegister(typeof(KeyIsObject_UseInterface_Entity)) as EntityTypeMetadata;
            var key = this.generator.GetEntityKey(typeMetadata, new KeyIsObject_UseInterface_Entity
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
            var typeMetadata = this.typeRepo.GetOrRegister(typeof(KeyIsObject_UseInterface_NotImplement_Entity)) as EntityTypeMetadata;

            try
            {
                var key = this.generator.GetEntityKey(typeMetadata, new KeyIsObject_UseInterface_NotImplement_Entity
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
