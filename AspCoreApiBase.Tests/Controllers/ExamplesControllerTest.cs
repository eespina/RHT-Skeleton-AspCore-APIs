using AspCoreApiBase.Controllers;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;


namespace AspCoreApiBase.Tests
{
    public class ExamplesControllerTest
    {
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<ILogger<UserController>> _loggerMock = new Mock<ILogger<UserController>>();
        private readonly Mock<IPropertyService> _propertyServiceMock = new Mock<IPropertyService>();
        private readonly Mock<IMailService> _mailServiceMock = new Mock<IMailService>();
        private readonly Mock<IAuthenticateService> _authenticateServiceMock = new Mock<IAuthenticateService>();

        [Fact(DisplayName = "Get_Returns_List_of_Users")]
        public void ValidateUsersFound()
        {
            //Arrange
            var userServiceMock = _userServiceMock.Setup(f => f.FindUsers()).ReturnsAsync(() => new List<OwnerViewModel>());
            var examplesControllerMock = new UserController(_userServiceMock.Object, _loggerMock.Object, _propertyServiceMock.Object, _mailServiceMock.Object, _authenticateServiceMock.Object);

            //Act
            var result = examplesControllerMock.Get();

            //Assert
            Assert.NotNull(result);
        }
    }
}
