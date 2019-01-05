namespace Blueve.ObjectRedisMapping.UnitTests
{
    using System.Collections.Generic;
    using Blueve.ObjectRedisMapping;
    using Blueve.ObjectRedisMapping.UnitTests.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Test the <see cref="DynamicProxyGenerator"/>.
    /// </summary>
    [TestClass]
    public class DynamicProxyGeneratorTests
    {
        private Mock<IDbAccessor> dbAccessor;

        [TestInitialize]
        public void Initialize()
        {
            this.dbAccessor = new Mock<IDbAccessor>();
        }

        [TestMethod]
        public void TestGenerateEntityProxy()
        {
            var stringDict = new Dictionary<string, string>();
            this.dbAccessor
                .Setup(accessor => accessor.Set(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((k, v) => stringDict.TryAdd(k, v));
            this.dbAccessor
                .Setup(accessor => accessor.Get("00000006BlueveUserId"))
                .Returns(() => "Blueve");
            var generator = new DynamicProxyGenerator(TypeRepository.CreateInstance(), this.dbAccessor.Object, string.Empty);

            var proxyOfPlainEntity = generator.Generate<PlainEntity>("00000006Blueve");
            Assert.AreEqual("Blueve", proxyOfPlainEntity.UserId);

            proxyOfPlainEntity.UserName = "Blueve";
            Assert.AreEqual("Blueve", stringDict["00000006BlueveUserName"]);
        }
    }
}
