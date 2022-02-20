using System.Threading.Tasks;
using AspCoreApiTemplate.ViewModels;

namespace AspCoreApiTemplate.Services.Interfaces
{
	public interface IMailService
	{
		Task<bool> SendMessage(MailViewModel mailViewModel);
		Task<bool> SaveEmailComposition(OwnerViewModel user, System.Security.Claims.ClaimsPrincipal currentUser);
	}
}
