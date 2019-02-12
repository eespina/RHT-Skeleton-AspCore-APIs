using AspCoreBase.Data;
using AspCoreBase.Data.Entities;
using AspCoreBase.Data.Entities.Authority;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreBase.Services
{
	[Authorize]
	public class UserService : IUserService
	{
		private readonly ILogger<UserService> logger;
		private readonly IVillageDbRepository villageDbRepository;
		private readonly IMapper mapper;
		private readonly UserManager<AuthorityUser> authorityUser;
		private readonly UserManager<AuthorityUser> userManager;

		public UserService(UserManager<AuthorityUser> AuthorityUser, IMapper mapper, ILogger<UserService> logger, IVillageDbRepository repository, UserManager<AuthorityUser> userManager)
		{
			this.authorityUser = AuthorityUser;
			this.villageDbRepository = repository;
			this.mapper = mapper;
			this.logger = logger;
			this.userManager = userManager;
		}

		public async Task<IEnumerable<OwnerViewModel>> FindUsers()
		{
			var users = await villageDbRepository.GetVillageUserOwners();
			if (users.Any())
			{
				var villageUsersMapped = mapper.Map<IEnumerable<OwnerUser>, IEnumerable<OwnerViewModel>>(users);
				return villageUsersMapped;
			}
			return null;
		}

		public async Task<OwnerViewModel> CreateNewUser(OwnerViewModel userViewModel, System.Security.Claims.ClaimsPrincipal currentUser)
		{
			try
			{
				var adminUserId = currentUser.Identities.First().Claims.First(i => i.Type.Split('/').Last() == "nameidentifier").Value;

				if (await authorityUser.FindByIdAsync(adminUserId) != null)
				{
					var user = await authorityUser.FindByEmailAsync(userViewModel.Email);
					if (user == null)
					{
						userViewModel.CurrentAdministeringUser = new Guid(adminUserId);
						IdentityResult result = new IdentityResult();
						user = new AuthorityUser()
						{
							FirstName = userViewModel.FirstName,
							LastName = userViewModel.LastName,
							Email = userViewModel.Email,
							UserName = userViewModel.UserName
						};

						//var temporaryCredentials = randomTemporaryCredentialsGeneration();	//TODO - RESTORE THIS when email Invitiations Services are in order

						//CREATE Village Authority User
						result = await authorityUser.CreateAsync(user, userViewModel.Password);

						if (result != IdentityResult.Success)
						{
							logger.LogWarning("ERROR inside UserService.CreateNewOwnerUser.Authority - For some reason, AuthorityUser Creation was NOT 'Success'ful. User was Not Created");
							result = null;
						}
						else//CREATE eVillage User
						{
							userViewModel.CreatedBy = userViewModel.CurrentAdministeringUser;
							userViewModel.CreatedDate = DateTime.Now;
							userViewModel.ModifiedBy = userViewModel.CurrentAdministeringUser;
							userViewModel.ModifiedDate = DateTime.Now;
							userViewModel.IsActive = true;
							userViewModel.UserId = new Guid(user.Id);
							Int16.TryParse(userViewModel.TypeNumber, out short userTypeIdFromViewModel);
							userViewModel.UserType = GetUserType(userTypeIdFromViewModel);

							if (await CreateVillageUser(userViewModel))
							{
								return userViewModel;
							}

							logger.LogWarning("ERROR inside UserService.CreateNewOwnerUser - User EXISTS - Did not create New Owner");
							return null;
						}
					}

					logger.LogWarning("ERROR inside UserService.CreateNewOwnerUser - User EXISTS - Did not create New Owner");
					return null;
				}

				logger.LogWarning("ERROR inside UserService.CreateNewOwnerUser - Admin " + adminUserId + "Not Found - Did not create New Owner");
				return null;
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside UserService.CreateNewOwnerUser - " + ex);
				return null;
			}
		}
		
		#region PRIVATE HELPER METHODS FOR USER SERVICE
		private string RandomTemporaryCredentialsGeneration()
		{
			var opts = new PasswordOptions()
			{
				RequiredLength = 8,
				RequireDigit = true,
				RequireLowercase = true,
				RequireNonAlphanumeric = true,
				RequireUppercase = true
			};

			string[] randomChars = new[] { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz", "0123456789", "!@#$%^&*" };

			Random rand = new Random(Environment.TickCount);
			List<char> chars = new List<char>();

			chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);   //RequireUppercase
			chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);   //RequireLowercase
			chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);   //RequireDigit
			chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);   //RequireNonAlphanumeric

			for (int i = chars.Count; i < opts.RequiredLength; i++)
			{
				string rcs = randomChars[rand.Next(0, randomChars.Length)];
				chars.Insert(rand.Next(0, chars.Count),
					rcs[rand.Next(0, rcs.Length)]);
			}

			return new string(chars.ToArray());
		}

		private async Task<bool> CreateVillageUser(OwnerViewModel user)
		{
			try
			{
				if (user.UserType.Id == 1)
				{
					return await CreateAdminUser(user);
				}
				else if (user.UserType.Id == 2 || user.UserType.Id == 3)
				{
					return await CreateOwnerUser(user);
				}
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside UserService.CreateVillageUser - " + ex);
			}
			return false;
		}

		private async Task<bool> CreateOwnerUser(OwnerViewModel user)
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
					ModifiedBy = user.CurrentAdministeringUser,
					ModifiedDate = DateTime.Now,
					CreatedBy = user.CurrentAdministeringUser,
					CreatedDate = DateTime.Now
				};

				villageDbRepository.AddEntity(ownerUser);

				return await villageDbRepository.SaveAllAsync();
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside UserService.createVillageOwnerUser - " + ex);
				return false;
			}
		}

		private async Task<bool> CreateAdminUser(OwnerViewModel user)
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
					ModifiedBy = user.CurrentAdministeringUser,
					ModifiedDate = DateTime.Now,
					CreatedBy = user.CurrentAdministeringUser,
					CreatedDate = DateTime.Now,
					StartDate = DateTime.Now
				};

				villageDbRepository.AddEntity(adminUser);

				return await villageDbRepository.SaveAllAsync();
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside UserService.createVillageOwnerUser - " + ex);
				return false;
			}
		}

		private UserType GetUserType(short typeName)
		{
			var type = new UserType();

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
					type.Name = "Renter";
					break;
				case 4:
					type.Id = 4;
					type.Name = "Doorman";
					break;
				case 5:
					type.Id = 5;
					type.Name = "Maintenance";
					break;
				case 6:
					type.Id = 6;
					type.Name = "Janitor";
					break;
				case 7:
					type.Id = 7;
					type.Name = "Vendor";
					break;
				case 8:
					type.Id = 8;
					type.Name = "Unassigned";
					break;
				default:
					type.Id = 8;
					type.Name = "Unassigned";
					break;
			}

			return type;
		}
		#endregion
	}
}
