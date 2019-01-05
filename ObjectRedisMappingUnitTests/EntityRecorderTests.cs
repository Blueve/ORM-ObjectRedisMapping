namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test the <see cref="EntityRecorder"/>.
    /// </summary>
    [TestClass]
    public class EntityRecorderTests
    {
        private Mock<IDbAccessor> dbAccessor;
        private Mock<IEntityDbRecordBuilder> dbKeyValueBuilder;
        private EntityRecorder recorder;

        [TestInitialize]
        public void Initialize()
        {
            this.dbAccessor = new Mock<IDbAccessor>();
            this.dbKeyValueBuilder = new Mock<IEntityDbRecordBuilder>();
            this.recorder = new EntityRecorder(this.dbAccessor.Object, this.dbKeyValueBuilder.Object);
        }

        [TestMethod]
        public void TestCommit()
        {
            var stringDict = new Dictionary<string, string>();
            var entity = new PlainEntity
            {
                UserId = "1",
                UserName = "Blueve"
            };
            this.dbAccessor
                .Setup(accessor => accessor.Set(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((k, v) => stringDict.TryAdd(k, v));
            this.dbKeyValueBuilder
                .Setup(builder => builder.Generate(entity))
                .Returns(() => new[]
                {
                    new DbRecord("PlainEntity000000011UserName", new DbValue { Type = DbValueType.String, Object = "Blueve" })
                });

            // Commit the entity.
            this.recorder.Commit(entity);
            Assert.AreEqual("Blueve", stringDict["PlainEntity000000011UserName"]);
        }
    }
}
