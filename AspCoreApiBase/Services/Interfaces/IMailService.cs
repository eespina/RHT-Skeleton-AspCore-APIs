using System.Threading.Tasks;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Services.Interfaces
{
	public interface IMailService
	{
		Task<bool> SendMessage(MailViewModel mailViewModel);
		Task<bool> SaveEmailComposition(OwnerViewModel user, System.Security.Claims.ClaimsPrincipal currentUser);
	}
}
