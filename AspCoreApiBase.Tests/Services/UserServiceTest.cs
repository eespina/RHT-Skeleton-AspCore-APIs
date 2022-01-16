using AspCoreBase.Data;
using AspCoreBase.Data.Entities;
using AspCoreBase.Data.Entities.Authority;
using AspCoreBase.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AspCoreApiBase.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IVillageDbRepository> _iVillageDbRepositoryMock = new Mock<IVillageDbRepository>();
        private readonly Mock<ILogger<UserService>> _loggerMock = new Mock<ILogger<UserService>>();
        private readonly Mock<UserManager<AuthorityUser>> _authorityUserMock = new Mock<UserManager<AuthorityUser>>(Mock.Of<IUserStore<AuthorityUser>>(), null, null, null, null, null, null, null, null);
        private readonly Mock<UserManager<AuthorityUser>> _userManagerMock = new Mock<UserManager<AuthorityUser>>(Mock.Of<IUserStore<AuthorityUser>>(), null, null, null, null, null, null, null, null);
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

        [Fact(DisplayName = "FindUsers_Returns_List_of_OwnerViewModel")]
        public void ValidateFindUsers()
        {
            //Arrange
            var dbMock = _iVillageDbRepositoryMock.Setup(g => g.GetVillageUserOwners()).ReturnsAsync(new List<OwnerUser>());

            _authorityUserMock.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new AuthorityUser { Id = "AuthorityUserId", FirstName = "TestFirstName", LastName = "TsetLastName" });
            _authorityUserMock.Setup(userManager => userManager.IsInRoleAsync(It.IsAny<AuthorityUser>(), "TestAuthorityUserRoleAsync")).ReturnsAsync(true);
            _userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new AuthorityUser { Id = "UserManagerId", FirstName = "TestFirstName", LastName = "TsetLastName" });
            _userManagerMock.Setup(userManager => userManager.IsInRoleAsync(It.IsAny<AuthorityUser>(), "TestUserManagerRoleAsync")).ReturnsAsync(true);

            var userServiceControllerMock = new UserService(_authorityUserMock.Object, _mapperMock.Object, _loggerMock.Object, _iVillageDbRepositoryMock.Object, _userManagerMock.Object);

            //Act
            var result = userServiceControllerMock.FindUsers();

            //Assert
            Assert.NotNull(result);
        }
    }
}
