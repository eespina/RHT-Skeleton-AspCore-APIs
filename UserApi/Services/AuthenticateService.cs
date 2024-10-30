using Extensions;
using Microsoft.Extensions.Options;
using UserApi.Models;
using UserApi.Services.Interfaces;

namespace UserApi.Services
{
    public class AuthenticateUserService : IAuthenticateUserService
    {
        private readonly ILogger<AuthenticateUserService> logger;
        private readonly ICredentialHandler credentialHandler;
        private AppSettings appSettings;

        public AuthenticateUserService(ILogger<AuthenticateUserService> logger, IOptions<AppSettings> appSettings, ICredentialHandler credentialHandler)
        {
            this.logger = logger;
            this.appSettings = appSettings.Value;
            this.credentialHandler = credentialHandler;
        }

        public Task<string> DecryptStringAES(string cipherText)
        {
            return credentialHandler.DecryptStringAES(cipherText, appSettings.EncryptionDecryptionKey);
        }
    }
}
