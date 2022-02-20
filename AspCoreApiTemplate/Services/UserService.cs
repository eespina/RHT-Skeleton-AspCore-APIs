using AspCoreApiTemplate.ViewModels;
using AspCoreApiTemplate.Data;
using AspCoreApiTemplate.Data.Entities;
using AspCoreApiTemplate.Data.Entities.Authority;
using AspCoreApiTemplate.Services.Interfaces;
using AspCoreApiTemplate.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreApiTemplate.Services
{
    [Authorize]
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly IExampleDbRepository exampleDbRepository;
        private readonly IMapper mapper;
        private readonly UserManager<AuthorityUser> authorityUser;
        private readonly UserManager<AuthorityUser> userManager;

        public UserService(UserManager<AuthorityUser> AuthorityUser, IMapper mapper, ILogger<UserService> logger, IExampleDbRepository repository, UserManager<AuthorityUser> userManager)
        {
            this.authorityUser = AuthorityUser;
            this.exampleDbRepository = repository;
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<OwnerViewModel>> FindUsers()
        {
            var users = await exampleDbRepository.GetExampleUserOwners();
            if (users.Any())
            {
                var exampleUsersMapped = mapper.Map<IEnumerable<OwnerUser>, IEnumerable<OwnerViewModel>>(users);
                return exampleUsersMapped;
            }
            return null;
        }

        public async Task<OwnerViewModel> FindUserByUserName(string userName)
        {
            var user = await exampleDbRepository.GetExampleUserOwnerByUserName(userName);
            if (user != null)
            {
                var exampleUserMapped = mapper.Map<OwnerUser, OwnerViewModel>(user);
                exampleUserMapped.CurrentAdministeringUser = user.OwnerUserId;
                exampleUserMapped.UserId = user.OwnerUserId;
                return exampleUserMapped;
            }
            return null;
        }

        public async Task<OwnerViewModel> FindUserByEmail(string email)
        {
            var user = await exampleDbRepository.GetExampleUserOwnerByEmail(email);
            if (user != null)
            {
                var exampleUserMapped = mapper.Map<OwnerUser, OwnerViewModel>(user);
                exampleUserMapped.CurrentAdministeringUser = user.OwnerUserId;
                exampleUserMapped.UserId = user.OwnerUserId;
                return exampleUserMapped;
            }
            return null;
        }

        public async Task<OwnerViewModel> UpdateUser(OwnerViewModel ownerViewModel)
        {
            var user = await exampleDbRepository.GetExampleUserOwnerByEmail(ownerViewModel.Email);
            if (user != null)
            {
                user.Email = ownerViewModel.Email;
                user.FirstName = ownerViewModel.FirstName;
                user.LastName = ownerViewModel.LastName;
                user.IsActive = ownerViewModel.IsActive;
                user.Notes = ownerViewModel.Notes;
                user.UserName = ownerViewModel.UserName;
                user.ModifiedBy = ownerViewModel.ModifiedBy;//could possibly be CurrentAdministeringUser (there's even a 'AdministeringUserEmail' property as well, lol)
                user.ModifiedDate = DateTime.Now;
                await exampleDbRepository.UpdateEntity(user);
                var isSaved = await exampleDbRepository.SaveAllAsync();
                if (isSaved)
                {
                    return ownerViewModel;
                }
            }
            return null;
        }

        public async Task<OwnerViewModel> CreateNewUser(OwnerViewModel userViewModel, string password)
        {
            try
            {
                var user = await authorityUser.FindByEmailAsync(userViewModel.Email);
                if (user == null)
                {
                    AuthorityUser existingAdminUser;
                    existingAdminUser = await authorityUser.FindByEmailAsync(userViewModel.AdministeringUserEmail);
                    if (existingAdminUser != null)
                    {
                        userViewModel.CurrentAdministeringUser = new Guid(existingAdminUser.Id.ToString());
                        IdentityResult result = new IdentityResult();
                        user = new AuthorityUser()
                        {
                            FirstName = userViewModel.FirstName,
                            LastName = userViewModel.LastName,
                            Email = userViewModel.Email,
                            UserName = userViewModel.UserName
                        };

                        //var temporaryCredentials = randomTemporaryCredentialsGeneration();	//TODO - RESTORE THIS when email Invitiations Services are in order

                        //CREATE example Authority User
                        result = await authorityUser.CreateAsync(user, password);

                        if (result != IdentityResult.Success)
                        {
                            logger.LogWarning("ERROR inside UserService.CreateNewOwnerUser.Authority - For some reason, AuthorityUser Creation was NOT 'Success'ful. User was Not Created");
                            result = null;
                        }
                        else//CREATE example User
                        {
                            userViewModel.UserId = new Guid(user.Id);
                            userViewModel.IsActive = true;
                            userViewModel.UserType = GetUserType(userViewModel.UserType.Id);
                            userViewModel.CreatedBy = userViewModel.CurrentAdministeringUser;
                            userViewModel.CreatedDate = DateTime.Now;
                            userViewModel.ModifiedBy = userViewModel.CurrentAdministeringUser;
                            userViewModel.ModifiedDate = DateTime.Now;

                            if (await CreateExampleUser(userViewModel))
                            {
                                return userViewModel;
                            }

                            logger.LogWarning("ERROR inside UserService.CreateNewOwnerUser - Authority User Created, but did not create New User");
                            return null;
                        }
                    }

                    logger.LogWarning("ERROR inside UserService.CreateNewOwnerUser - Admin User Not Found - Did not create New Owner");
                    return null;
                }

                logger.LogWarning("ERROR inside UserService.CreateNewOwnerUser - User EXISTS - Did not create New Owner");
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR inside UserService.CreateNewOwnerUser - " + ex);
                return null;
            }
        }

        public async Task<bool> DeleteUser(string userId)
        {
            try
            {
                var example = await exampleDbRepository.GetExampleUserOwnerByOwnerUserId(userId);
                if (example != null)
                {
                    await exampleDbRepository.DeleteEntityAsync(example);
                    var isownerUserDeleted = await exampleDbRepository.SaveAllAsync();
                    if (isownerUserDeleted)
                    {
                        var user = await authorityUser.FindByIdAsync(userId);
                        if (user != null)
                        {
                            var isAuthorityUserDeleted = await authorityUser.DeleteAsync(user);
                            if (isAuthorityUserDeleted.Succeeded)
                                return true;
                            else
                                logger.LogWarning("WARNING DeleteAsync NOT Succeeded - User NOT DELETED.");
                        }
                        else
                        {
                            logger.LogWarning("WARNING inside UserService.DeleteUser - User is NULL.");
                        }
                    }
                }
                else
                {
                    logger.LogWarning("WARNING inside UserService.DeleteUser - OwnerUser is NULL.");
                }

                return false;
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR inside UserService.DeleteUser when calling it's Service counterpart - " + ex);
                return false;
            }
        }

        #region PRIVATE HELPER METHODS FOR USER SERVICE

        /// <summary>
        /// The theory HERE is that if you did not need to seperate the Admin user from whatever user is going to be needed
        /// in a real scenario, you can just neglect one or delete and combine user/account tech
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> CreateExampleUser(OwnerViewModel user)
        {
            try
            {
                if (user.UserType.Id == 1)
                {
                    return await CreateExampleAdminUser(user);
                }
                else if (user.UserType.Id == 2 || user.UserType.Id == 3)
                {
                    return await CreateExampleOwnerUser(user);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR inside UserService.CreateExampleUser - " + ex);
            }
            return false;
        }

        private async Task<bool> CreateExampleOwnerUser(OwnerViewModel user)
        {
            try
            {
                var ownerUser = new OwnerUser
                {
                    OwnerUserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsActive = true,
                    UserName = user.UserName,
                    Notes = user.Notes,
                    ModifiedBy = user.CurrentAdministeringUser,
                    ModifiedDate = DateTime.Now,
                    CreatedBy = user.CurrentAdministeringUser,
                    CreatedDate = DateTime.Now
                };

                exampleDbRepository.AddEntity(ownerUser);

                return await exampleDbRepository.SaveAllAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR inside UserService.CreateExampleOwnerUser - " + ex);
                return false;
            }
        }

        /// <summary>
        /// This should NOT be available in the Owner's portal. Only in the Admins portal.
        /// ALSO, the OwnerViewModel parameter should be switched to an AdminViewModel
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> CreateExampleAdminUser(OwnerViewModel user)
        {
            try
            {
                var adminUser = new AdminUser
                {
                    AdminUserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsActive = true,
                    UserName = user.UserName,
                    Notes = user.Notes,
                    ModifiedBy = user.CurrentAdministeringUser,
                    ModifiedDate = DateTime.Now,
                    CreatedBy = user.CurrentAdministeringUser,
                    CreatedDate = DateTime.Now,
                    StartDate = DateTime.Now
                };

                exampleDbRepository.AddEntity(adminUser);

                return await exampleDbRepository.SaveAllAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR inside UserService.CreateExampleAdminUser - " + ex);
                return false;
            }
        }

        private UserTypeViewModel GetUserType(int typeName)
        {
            var type = new UserTypeViewModel();

            switch (typeName)
            {
                case 1:
                    type.Id = 1;
                    type.Name = "Admin";
                    break;
                case 2:
                    type.Id = 2;
                    type.Name = "Owner";
                    break;
                case 3:
                    type.Id = 3;
                    type.Name = "Vendor";
                    break;
                case 4:
                    type.Id = 4;
                    type.Name = "Unassigned";
                    break;
                default:
                    type.Id = 5;
                    type.Name = "Unassigned";
                    break;
            }

            return type;
        }

        #endregion
    }
}
