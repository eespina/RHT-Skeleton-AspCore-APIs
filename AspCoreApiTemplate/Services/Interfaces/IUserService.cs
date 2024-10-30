using AspCoreApiTemplate.ViewModels;
using System.Threading.Tasks;

namespace AspCoreApiTemplate.Services.Interfaces
{
    public interface IUserService
	{
		Task<OwnerViewModel> FindUserByUserName(string userName);
	}
}
