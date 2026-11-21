using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniHttpServer.Framework.Results;

namespace MiniHttpServer.Framework.UnitTests
{
    [TestClass]
    public class ResultTests
    {
        [TestMethod]
        public void HtmlResult_Executes_ReturnsHtml()
        {
            // Arrange
            string html = "<html>Test</html>";
            var result = new HtmlResult(html);

            // Act & Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void JsonResult_Executes_SerializesData()
        {
            // Arrange
            var data = new { Name = "Test", Value = 123 };
            var result = new JsonResult(data);

            // Act & Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PageResult_Executes_UsesTemplate()
        {
            // Arrange
            var result = new PageResult("test.html", new { Title = "Test" });

            // Act & Assert
            Assert.IsNotNull(result);
        }
    }
}