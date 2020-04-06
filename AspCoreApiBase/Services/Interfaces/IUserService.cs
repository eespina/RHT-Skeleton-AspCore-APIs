using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Services.Interfaces
{
	public interface IUserService
	{
		Task<IEnumerable<OwnerViewModel>> FindUsers();
        Task<OwnerViewModel> FindUser(string userName);
        Task<OwnerViewModel> CreateNewUser(OwnerViewModel userOwnerViewModel);
	}
}
