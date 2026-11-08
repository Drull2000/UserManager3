using Moq;
using UserManager.Controllers;
using UserManager.DTO;
using UserManager.Models;
using UserManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace UserManager.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly UsersController _usersController;
        private readonly Mock<IUserService> _userServiceMock;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();

            _usersController = new UsersController(
                _userServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithUsers()
        {
            var users = new List<UserDTO>
            {
                new() { Id = Guid.NewGuid(), Email = "test@test.com" }
            };

            _userServiceMock.Setup(service => service.GetUsersAsync()).ReturnsAsync(users);

            var result = await _usersController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Same(users, okResult.Value);
            _userServiceMock.Verify(service => service.GetUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenNoUsers_ReturnsOkWithEmptyList()
        {
            var users = new List<UserDTO>();
            _userServiceMock.Setup(service => service.GetUsersAsync()).ReturnsAsync(users);

            var result = await _usersController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Same(users, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenUserExists_ReturnsOkWithUser()
        {
            var userId = Guid.NewGuid();
            var user = new UserDTO { Id = userId, Email = "test@test.com", FirstName = "Name", LastName = "LastName" };

            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var result = await _usersController.GetById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Same(user, okResult.Value);
        }

        [Fact]
        public async Task GetById_WhenUserDoesNotExist_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((UserDTO)null!);

            var result = await _usersController.GetById(userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionWithCreatedUser()
        {
            var request = new CreateUserDTO
            {
                Email = "test@test.com",
                FirstName = "Name",
                LastName = "LastName",
                Role = "User"
            };
            var createdUser = new UserDTO { Id = Guid.NewGuid(), Email = request.Email };

            _userServiceMock.Setup(service => service.CreateAsync(request)).ReturnsAsync(createdUser);

            var result = await _usersController.Create(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(UsersController.GetById), createdResult.ActionName);
            Assert.Equal(createdUser.Id, createdResult.RouteValues!["id"]);
            Assert.Same(createdUser, createdResult.Value);
            _userServiceMock.Verify(service => service.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenUserExists_ReturnsNoContent()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(service => service.DeleteAsync(userId)).ReturnsAsync(true);

            var result = await _usersController.Delete(userId);

            Assert.IsType<NoContentResult>(result);
            _userServiceMock.Verify(service => service.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenUserDoesNotExist_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            _userServiceMock.Setup(service => service.DeleteAsync(userId)).ReturnsAsync(false);

            var result = await _usersController.Delete(userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
