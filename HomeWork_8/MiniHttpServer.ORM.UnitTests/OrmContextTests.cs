using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORM;
using System;
using System.Threading.Tasks;

namespace MiniHttpServer.ORM.UnitTests
{
    [TestClass]
    public class OrmContextTests
    {
        private const string _connectionString = "Host=localhost;Port=5432;Database=OrisDb;Username=postgres;Password=postgres;";

        [TestMethod]
        public async Task GetAllAsync_InvalidTableName_ThrowsArgumentException()
        {
            // Arrange
            var orm = new OrmContext(_connectionString);

            // Act & Assert
            bool exceptionThrown = false;
            try
            {
                await orm.GetAllAsync<User>("Invalid Table!");
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected ArgumentException was not thrown.");
        }

        [TestMethod]
        public async Task GetAllAsync_EmptyTableName_ThrowsArgumentException()
        {
            // Arrange
            var orm = new OrmContext(_connectionString);

            // Act & Assert
            bool exceptionThrown = false;
            try
            {
                await orm.GetAllAsync<User>("");
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected ArgumentException was not thrown.");
        }

        [TestMethod]
        public async Task GetAllAsync_NullTableName_ThrowsArgumentException()
        {
            // Arrange
            var orm = new OrmContext(_connectionString);

            // Act & Assert
            bool exceptionThrown = false;
            try
            {
                await orm.GetAllAsync<User>(null!);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected ArgumentException was not thrown.");
        }

        [TestMethod]
        public async Task InsertAsync_InvalidTableName_ThrowsArgumentException()
        {
            // Arrange
            var orm = new OrmContext(_connectionString);
            var user = new User { Name = "Test", Email = "test@test.com", Age = 25 };

            // Act & Assert
            bool exceptionThrown = false;
            try
            {
                await orm.InsertAsync("Invalid Table!", user);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected ArgumentException was not thrown.");
        }

        [TestMethod]
        public async Task UpdateAsync_InvalidTableName_ThrowsArgumentException()
        {
            // Arrange
            var orm = new OrmContext(_connectionString);
            var user = new User { Name = "Test", Email = "test@test.com", Age = 25 };

            // Act & Assert
            bool exceptionThrown = false;
            try
            {
                await orm.UpdateAsync("Invalid Table!", 1, user);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected ArgumentException was not thrown.");
        }

        [TestMethod]
        public async Task DeleteAsync_InvalidTableName_ThrowsArgumentException()
        {
            // Arrange
            var orm = new OrmContext(_connectionString);

            // Act & Assert
            bool exceptionThrown = false;
            try
            {
                await orm.DeleteAsync("Invalid Table!", 1);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected ArgumentException was not thrown.");
        }

        [TestMethod]
        public void OrmContext_NullConnectionString_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            bool exceptionThrown = false;
            try
            {
                new OrmContext(null!);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected ArgumentException was not thrown.");
        }

        [TestMethod]
        public void OrmContext_EmptyConnectionString_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            bool exceptionThrown = false;
            try
            {
                new OrmContext("");
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected ArgumentException was not thrown.");
        }

        [TestMethod]
        public void OrmContext_WhitespaceConnectionString_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            bool exceptionThrown = false;
            try
            {
                new OrmContext("   ");
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected ArgumentException was not thrown.");
        }
    }
}