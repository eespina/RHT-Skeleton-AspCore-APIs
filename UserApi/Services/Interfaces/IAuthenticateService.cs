namespace UserApi.Services.Interfaces
{
    public interface IAuthenticateUserService
	{
		Task<string> DecryptStringAES(string cipherText);
	}
}
