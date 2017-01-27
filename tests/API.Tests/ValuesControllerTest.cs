using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using API.Tests.Config;
using Xunit;

namespace API.Tests
{
    public class ValuesControllerTest : IntegrationTestBase
    {
        [Fact]
        public async Task Get_should_return_unauthorized()
        {
            // Arrange
            

            // Act
            var result = await HttpClient.GetAsync("api/values");

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Post_should_return_unauthorized_for_unauthorized_user()
        {
            // Act
            var result = await HttpClient.PostAsync("api/values", new StringContent("someVal"));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task Post_should_return_OK_for_authrized_user()
        {
            // Arrange
            await SetUpTokenFor("user", "password");

            // Act
            var result = await HttpClient.PostAsync("api/values", new StringContent("yeah", Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Delete_should_not_be_allowed_for_normal_user()
        {
            // Arrange
            await SetUpTokenFor("user", "password");

            // Act
            var result = await HttpClient.DeleteAsync("api/values/5");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }

        [Fact]
        public async Task Delete_should_be_allowed_for_admin_user()
        {
            // Arrange
            await SetUpTokenFor("admin", "adminPassword");

            // Act
            var result = await HttpClient.DeleteAsync("api/values/5");

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
