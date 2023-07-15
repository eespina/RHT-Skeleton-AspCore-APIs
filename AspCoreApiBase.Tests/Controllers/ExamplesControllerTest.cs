using AspCoreApiTemplate.Controllers;
using AspCoreApiTemplate.Services.Interfaces;
using AspCoreApiTemplate.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;


namespace AspCoreApiTemplate.Tests
{
    public class ExamplesControllerTest
    {
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<ILogger<UserController>> _loggerMock = new Mock<ILogger<UserController>>();
        private readonly Mock<IExampleService> _exampleServiceMock = new Mock<IExampleService>();
        private readonly Mock<IMailService> _mailServiceMock = new Mock<IMailService>();
        private readonly Mock<IAuthenticateService> _authenticateServiceMock = new Mock<IAuthenticateService>();

        [Fact(DisplayName = "Get_Returns_List_of_Users")]
        public void ValidateUsersFound()
        {
            //Arrange
            var userServiceMock = _userServiceMock.Setup(f => f.FindUsers()).ReturnsAsync(() => new List<OwnerViewModel>());
            var examplesControllerMock = new UserController(_userServiceMock.Object, _loggerMock.Object, _mailServiceMock.Object, _authenticateServiceMock.Object);

            //Act
            var result = examplesControllerMock.Get();

            //Assert
            Assert.NotNull(result);
        }
    }
}
