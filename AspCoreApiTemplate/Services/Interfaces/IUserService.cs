using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreApiTemplate.ViewModels;

namespace AspCoreApiTemplate.Services.Interfaces
{
	public interface IUserService
	{
		Task<IEnumerable<OwnerViewModel>> FindUsers();
        Task<OwnerViewModel> FindUserByUserName(string userName);
		Task<OwnerViewModel> FindUserByEmail(string email);
		Task<OwnerViewModel> CreateNewUser(OwnerViewModel userOwnerViewModel, string password);
		Task<OwnerViewModel> UpdateUser(OwnerViewModel ownerViewModel);
		Task<bool> DeleteUser(string userId);
	}
}
