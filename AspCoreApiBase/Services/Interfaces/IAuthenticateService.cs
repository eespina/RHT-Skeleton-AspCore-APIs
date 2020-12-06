using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Services.Interfaces
{
	public interface IAuthenticateService
	{
		Task<SignInResult> PasswordSign(string decryptedUsername, string decryptedPassword);
		Task<bool> SignOutAsync();
		Task<OwnerViewModel> CreateToken(string decryptedUsername, string decryptedPassword);
		Task<bool> ChangeCredentialsAsync(string decryptedUsername, string decryptedPassword);
		Task<string> DecryptStringAES(string cipherText);
		Task<string> EncryptStringAES(string plainText);
	}
}
