using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Services.Interfaces
{
	public interface IUserService
	{
		Task<IEnumerable<UserViewModel>> FindUsers();
		Task<UserViewModel> CreateNewUser(UserViewModel userOwnerViewModel, System.Security.Claims.ClaimsPrincipal currentUser);
	}
}
