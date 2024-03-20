using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineStoreApi.Controllers;
using OnlineStoreApi.Dtos;
using OnlineStoreApi.Models;
using OnlineStoreApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineStoreApi.Tests
{
    [TestFixture]
    // Testing only that different routes return correct type of response
    public class UsersControllerTests
    {
        UsersController _usersController;
        Mock<IUserService> _userServiceMock;
        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _usersController = new UsersController(_userServiceMock.Object);
        }
        // naming = (MethodTesting)_Input State_ Expected Output

        // RegisterAsync
        [Test]
        public async Task RegisterAsync_ValidInput_OkResult()
        {
            // Arrange
            RegisterModel registerModel = new RegisterModel
            {
                Email = "test@example.com",
                UserName = "testuser",
                Password = "TestPassword",
            };
            _userServiceMock.Setup(s => s.RegisterUserAsync(registerModel))
                .ReturnsAsync(new AuthModel { IsAuthenticated = true });
            // Act
            var result = await _usersController.RegisterAsync(registerModel);

            // Assert
            _userServiceMock.Verify(x => x.RegisterUserAsync(registerModel), Times.Once());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult.Value, Is.InstanceOf<AuthModel>());

            var authModel = okObjectResult.Value as AuthModel;
            Assert.That(authModel.IsAuthenticated, Is.True);
        }
        [Test]
        public async Task RegisterAsync_InvalidInput_BadRequestResult()
        {
            // Arrange
            RegisterModel registerModel = new RegisterModel
            {
                Email = "",
                UserName = "testuser",
                Password = "TestPassword",
            };
            _userServiceMock.Setup(s => s.RegisterUserAsync(registerModel))
                .ReturnsAsync(new AuthModel { IsAuthenticated = false, Message = "Auth" });
            // Act
            var result = await _usersController.RegisterAsync(registerModel);

            // Assert
            _userServiceMock.Verify(x => x.RegisterUserAsync(registerModel), Times.Once());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.That(badRequestObjectResult.Value, Is.InstanceOf<string>());
        }
        // LoginAsync
        [Test]
        public async Task LoginAsync_ValidInput_OkResult()
        {
            // Arrange
            LoginModel loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "TestPassword",
            };
            _userServiceMock.Setup(s => s.LoginUserAsync(loginModel))
                .ReturnsAsync(new AuthModel { IsAuthenticated = true });
            // Act
            var result = await _usersController.LoginAsync(loginModel);

            // Assert
            _userServiceMock.Verify(x => x.LoginUserAsync(loginModel), Times.Once());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult.Value, Is.InstanceOf<AuthModel>());

            var authModel = okObjectResult.Value as AuthModel;
            Assert.That(authModel.IsAuthenticated, Is.True);
        }
        [Test]
        public async Task LoginAsync_InvalidInput_BadRequestResult()
        {
            // Arrange
            LoginModel loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "TestPassword",
            };
            _userServiceMock.Setup(s => s.LoginUserAsync(loginModel))
                .ReturnsAsync(new AuthModel { IsAuthenticated = false, Message = "Message" });
            // Act
            var result = await _usersController.LoginAsync(loginModel);

            // Assert
            _userServiceMock.Verify(x => x.LoginUserAsync(loginModel), Times.Once());
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.That(badRequestObjectResult?.Value, Is.InstanceOf<string>());
        }
        // RefreshUserTokenAsync
        [Test]
        public async Task RefreshUserTokenAsync_ValidInput_OkResult()
        {
            // Arrange
            RefreshTokenDto refreshTokenDto = new RefreshTokenDto
            {
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken",
            };
            _userServiceMock.Setup(s => s.RefreshUserTokenAsync(refreshTokenDto.AccessToken, refreshTokenDto.RefreshToken))
                .ReturnsAsync(new TokenModel
                {
                    AccessToken = new JwtSecurityToken(),
                    RefreshToken = new JwtSecurityToken()
                });
            // Act
            var result = await _usersController.RefreshUserTokenAsync(refreshTokenDto);

            // Assert
            _userServiceMock.Verify(x => x.RefreshUserTokenAsync(refreshTokenDto.AccessToken, refreshTokenDto.RefreshToken), Times.Once());
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;
            Assert.That(okObjectResult.Value, Is.InstanceOf<TokenModel>());

        }
    }
}
