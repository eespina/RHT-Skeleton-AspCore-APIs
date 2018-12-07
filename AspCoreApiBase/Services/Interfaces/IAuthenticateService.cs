using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Services.Interfaces
{
	public interface IAuthenticateService
	{
		Task<SignInResult> PasswordSign(LoginViewModel model);
		Task<bool> SignOutAsync();
		Task<TokenHandleViewModel> CreateToken(LoginViewModel model);
		Task<bool> ChangeCredentialsAsync(LoginViewModel model);
	}
}
