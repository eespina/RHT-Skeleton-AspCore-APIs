using UserApi.ViewModels;

namespace UserApi.Services.Interfaces
{
    public interface IUserService
	{
		Task<IEnumerable<OwnerViewModel>> FindUsers();
        Task<OwnerViewModel> FindUserByUserName(string userName);
		Task<OwnerViewModel> FindUserByEmail(string email);
		Task<OwnerViewModel> CreateNewUser(OwnerViewModel userOwnerViewModel, string password);
		Task<OwnerViewModel> UpdateUser(OwnerViewModel ownerViewModel);
		Task<OwnerViewModel> PutUser_iDontKnow();
        //Task<bool> CreateExampleUserConnection(OwnerViewModel u);
        Task<bool> DeleteUser(string userId);
        Task<string> DecryptStringAES(string cipherText);
        Task<string> EncryptStringAES(string plainText);
    }
}
