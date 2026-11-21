using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniHttpServer.Framework.Attributes;

namespace MiniHttpServer.Framework.UnitTests
{
    [TestClass]
    public class AttributeTests
    {
        [TestMethod]
        public void HttpGetAttribute_HasGetVerb()
        {
            // Arrange
            var attr = new HttpGetAttribute("/api/test");

            // Act & Assert
            Assert.AreEqual("GET", attr.Verb);
        }

        [TestMethod]
        public void HttpPostAttribute_HasPostVerb()
        {
            // Arrange
            var attr = new HttpPostAttribute("/api/test");

            // Act & Assert
            Assert.AreEqual("POST", attr.Verb);
        }

        [TestMethod]
        public void ControllerAttribute_HasRoute()
        {
            // Arrange
            var attr = new ControllerAttribute("/home");

            // Act & Assert
            Assert.AreEqual("/home", attr.Route);
        }
    }
}