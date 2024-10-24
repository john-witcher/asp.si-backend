// using Xunit;
// using Moq;
// using Api.Services;
// using Microsoft.AspNetCore.Mvc.Testing;
// using System.Threading.Tasks;
// using System.Net.Http;
// using System.Net;
// using Api.Models.Db;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.DependencyInjection.Extensions;
// using Microsoft.Extensions.Logging;
// using System.Collections.Generic;
// using Newtonsoft.Json;
// using System.Text;
// using FluentAssertions;
//
// namespace Api.IntegrationTests
// {
//     public class RoleControllerTests : IClassFixture<WebApplicationFactory<Program>>
//     {
//         private readonly HttpClient _client;
//         private readonly Mock<RoleService> _roleServiceMock;
//
//         public RoleControllerTests(WebApplicationFactory<Program> factory)
//         {
//             _roleServiceMock = new Mock<RoleService>();
//
//             // Create test client with mocked RoleService
//             _client = factory.WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureServices(services =>
//                 {
//                     services.RemoveAll(typeof(RoleService));
//                     services.AddScoped(_ => _roleServiceMock.Object);
//                 });
//             }).CreateClient();
//         }
//
//         // Test for GET: /api/roles
//         [Fact]
//         public async Task GetAllRoles_ReturnsOkAndRoles()
//         {
//             // Arrange
//             var roles = new List<IdentityRole>
//             {
//                 new IdentityRole("Admin"),
//                 new IdentityRole("User")
//             };
//             _roleServiceMock.Setup(service => service.GetAllRoles()).Returns(roles);
//
//             // Act
//             var response = await _client.GetAsync("/api/roles");
//
//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.OK);
//             var responseString = await response.Content.ReadAsStringAsync();
//             var result = JsonConvert.DeserializeObject<List<IdentityRole>>(responseString);
//             result.Should().BeEquivalentTo(roles);
//         }
//
//         // Test for GET: /api/roles/{roleName}
//         [Fact]
//         public async Task GetRoleByName_WhenRoleExists_ReturnsOk()
//         {
//             // Arrange
//             var roleName = "Admin";
//             var role = new IdentityRole(roleName);
//             _roleServiceMock.Setup(service => service.GetRoleByNameAsync(roleName)).ReturnsAsync(role);
//
//             // Act
//             var response = await _client.GetAsync($"/api/roles/{roleName}");
//
//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.OK);
//             var responseString = await response.Content.ReadAsStringAsync();
//             var result = JsonConvert.DeserializeObject<IdentityRole>(responseString);
//             result.Should().BeEquivalentTo(role);
//         }
//
//         [Fact]
//         public async Task GetRoleByName_WhenRoleDoesNotExist_ReturnsNotFound()
//         {
//             // Arrange
//             var roleName = "NonExistentRole";
//             _roleServiceMock.Setup(service => service.GetRoleByNameAsync(roleName)).ThrowsAsync(new RoleNotFoundException(roleName));
//
//             // Act
//             var response = await _client.GetAsync($"/api/roles/{roleName}");
//
//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//         }
//
//         // Test for POST: /api/roles
//         [Fact]
//         public async Task AddRole_ReturnsOk_WhenRoleIsAddedSuccessfully()
//         {
//             // Arrange
//             var roleName = "Manager";
//             _roleServiceMock.Setup(service => service.AddRoleAsync(roleName)).ReturnsAsync(new IdentityRole(roleName));
//
//             var content = new StringContent(JsonConvert.SerializeObject(roleName), Encoding.UTF8, "application/json");
//
//             // Act
//             var response = await _client.PostAsync("/api/roles", content);
//
//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.OK);
//             var responseString = await response.Content.ReadAsStringAsync();
//             var result = JsonConvert.DeserializeObject<IdentityRole>(responseString);
//             result.Name.Should().Be(roleName);
//         }
//
//         // Test for DELETE: /api/roles/{roleName}
//         [Fact]
//         public async Task RemoveRole_ReturnsOk_WhenRoleIsDeletedSuccessfully()
//         {
//             // Arrange
//             var roleName = "User";
//             _roleServiceMock.Setup(service => service.RemoveRoleAsync(roleName)).ReturnsAsync(true);
//
//             // Act
//             var response = await _client.DeleteAsync($"/api/roles/{roleName}");
//
//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.OK);
//         }
//
//         [Fact]
//         public async Task RemoveRole_ReturnsNotFound_WhenRoleDoesNotExist()
//         {
//             // Arrange
//             var roleName = "NonExistentRole";
//             _roleServiceMock.Setup(service => service.RemoveRoleAsync(roleName)).ThrowsAsync(new RoleNotFoundException(roleName));
//
//             // Act
//             var response = await _client.DeleteAsync($"/api/roles/{roleName}");
//
//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//         }
//     }
// }
