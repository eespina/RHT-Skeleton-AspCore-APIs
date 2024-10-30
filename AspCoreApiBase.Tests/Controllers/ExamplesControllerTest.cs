using UserApi.Controllers;
using UserApi.Services.Interfaces;
using UserApi.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;


namespace UserApi.Tests
{
    public class ExamplesControllerTest
    {
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<ILogger<UserController>> _loggerMock = new Mock<ILogger<UserController>>();
        private readonly Mock<IAuthenticateUserService> _authenticateServiceMock = new Mock<IAuthenticateUserService>();

        [Fact(DisplayName = "Get_Returns_List_of_Users")]
        public void ValidateUsersFound()
        {
            //Arrange
            var userServiceMock = _userServiceMock.Setup(f => f.FindUsers()).ReturnsAsync(() => new List<OwnerViewModel>());
            var examplesControllerMock = new UserController(_userServiceMock.Object, _loggerMock.Object, _authenticateServiceMock.Object);

            //Act
            var result = examplesControllerMock.Get();

            //Assert
            Assert.NotNull(result);
        }
    }
}
