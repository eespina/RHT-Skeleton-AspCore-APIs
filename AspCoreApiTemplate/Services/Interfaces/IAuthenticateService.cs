using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using AspCoreApiTemplate.ViewModels;

namespace AspCoreApiTemplate.Services.Interfaces
{
	public interface IAuthenticateService
	{
		Task<SignInResult> PasswordSign(string decryptedUsername, string decryptedPassword);
		Task<bool> SignOutAsync();
		Task<OwnerViewModel> CreateToken(string decryptedUsername, string decryptedPassword);
		//Task<string> ChangeCredentialsAsync(string userName, string decryptedOldPassword, string decryptedNewPassword);
		Task<bool> EnsureAdministeringUserIsValid(string id, string decryptedOldPassword);
		Task<string> ChangeCredentialsAsync(string id, string decryptedNewPassword);
		Task<string> DecryptStringAES(string cipherText);
		Task<string> EncryptStringAES(string plainText);
	}
}
