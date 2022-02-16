using AspCoreApiBase.Models;
using AspCoreBase.Data.Entities.Authority;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AspCoreBase.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly ILogger<AuthenticateService> logger;
        private readonly SignInManager<AuthorityUser> signInManager;    // Need this service to Login/Logout
        private readonly UserManager<AuthorityUser> userManager; //useful when dealing with TOKENS in cross-application security or client/server separation
        private readonly IConfiguration config;
        private AppSettings appSettings;

        public AuthenticateService(ILogger<AuthenticateService> logger, UserManager<AuthorityUser> userManager, SignInManager<AuthorityUser> signInManager, IConfiguration config, IOptions<AppSettings> appSettings)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.config = config;
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }

        public async Task<SignInResult> PasswordSign(string decryptedUsername, string decryptedPassword)
        {
            try
            {
                var rememberMe = false; //this should actually be part of a viewmodel from the client.
                var result = await signInManager.PasswordSignInAsync(decryptedUsername, decryptedPassword, rememberMe, false);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR using SignInManager to Sign in - " + ex);
                return null;
            }

        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                await signInManager.SignOutAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR Signing Out - " + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// I believe this is useful for Token use when the Client is NOT attached to this project/solution. This uses TOKENS cor cross-pplication security
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <returns>TokenHandleViewModel that contains a string token and its expiration date/time</returns>
        public async Task<OwnerViewModel> CreateToken(string decryptedUsername, string decryptedPassword)
        {
            var user = await userManager.FindByNameAsync(decryptedUsername);   //userManager needs to be initialized in the Constructor

            if (user != null)
            {
                OwnerViewModel ownerViewModel = new OwnerViewModel()
                {
                    UserName = decryptedUsername,
                    Email = user.Email
                };

                //figure out if the login actually works
                try
                {
                    var result = await signInManager.CheckPasswordSignInAsync(user, decryptedPassword, false);
                    //var result = signInManager.PasswordSignInAsync();   //This signs in the user with a cookie, THIS IS NOT WHAT WE NEED or want, hence it is commented out

                    if (result.Succeeded)
                    {
                        //Create The token
                        try
                        {
                            //// claims are a set of properties with well known values in them that can be stored in the token and used buy the client or when its passed back to the server
                            //var claims = new System.Collections.Generic.List<Claim>
                            //{
                            //    new Claim(ClaimTypes.Name, user.UserName),
                            //    new Claim(ClaimTypes.Role, "SuperAuthorizedPerson")
                            //};
                            #region CLAIMS UNUSED VERSION
                            //var claims = new[]
                            //{
                            //                   //JwtRegisteredClaimNames is a 'type' that contains names used by the token where 'Sub', as an example, is the name of the subject
                            //                   new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
                            //                   //, new Claim(JwtRegisteredClaimNames.Email, user.Email)
                            //                   //'jti' is a unique string thats representative of each token
                            //                   , new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                            //                   //this is going to be the username of the user and is mapped to the identity inside the User object every Controller (and View)
                            //                       //this way, our token will contain enough information to tie together the current user in the API to the actual AspCoreBaseUser that we need for the relational model
                            //                   , new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)

                            //                   //You can also take the standard user claims that are in the system and append them here, and they'd be added into the token
                            //                       // but JSON web tokens are complex as it has a few moving pieces that are important (too much for the tutorial)
                            //                   };
                            #endregion

                            //var tokenKey = config["Tokens:Key"];

                            // The 'key' is the secret used to encrypt the token
                            // This will be used when we read in the token as a request is made, as well
                            //as when we are generating the token so we know how to read/write the token
                            //parts of the token are encrypted and some parts are NOT
                            //information about the individuals or individual claims can be read without decrypting the token
                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                            var token = new JwtSecurityToken(
                                config["Tokens:Issuer"],
                                config["Tokens:Audience"],
                                //claims,
                                expires: DateTime.UtcNow.AddMinutes(15),
                                signingCredentials: creds
                                );

                            ownerViewModel.tokenHandleViewModel = new TokenHandleViewModel()
                            {
                                Token = new JwtSecurityTokenHandler().WriteToken(token),    // 'writeToken' returns an actual string
                                Expiration = token.ValidTo  // returns an actual expiration time
                                                            // also need to create the issuer and audience., this is done in the config file
                            };

                            // empty quote becaseu we do NOT actually have a source for this resources,
                            //were actually going to want to write a new object, which will be called 'results'
                            return ownerViewModel;
                        }
                        catch (Exception ex)
                        {
                            //throw new InvalidOperationException($"Cannot create Token {ex}"); //TODO - LOG
                            var NEEDtoLOGthis = ex.ToString();
                            return null;
                        }
                    }//END - if (result.Succeeded)
                    else
                    {
                        logger.LogWarning("Unsuccessful Credential attempt.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "ERROR inside AuthenticateService.cs CreateToken method");
                }
            }
            else
            {
                logger.LogWarning(decryptedUsername + " username NOT found.");
            }

            return null;    //No User found Or Password incorrect
        }

        public async Task<bool> EnsureAdministeringUserIsValid(string id, string decryptedOldPassword)
        {
            var user = await userManager.FindByIdAsync(id);   //userManager needs to be initialized in the Constructor

            if (user != null)
            {
                var result = await signInManager.CheckPasswordSignInAsync(user, decryptedOldPassword, false);
                if (result != null)
                {
                    if (result.Succeeded)
                    {
                        return true;
                    }
                    else
                    {
                        logger.LogWarning("WARNING inside EnsureAdministeringUserIsValid - Authentication attempt Failed. Credentials may be incorrect");
                    }
                }
                else
                {
                    logger.LogWarning("WARNING inside EnsureAdministeringUserIsValid - Authentication attempt Failed. Resulting attempt returned NULL for variable currentUser");
                }
            }
            else
            {
                logger.LogWarning("WARNING inside EnsureAdministeringUserIsValid - Authentication attempt Failed. Existing user NOT FOUND");
            }

            return false;
        }

        public async Task<string> ChangeCredentialsAsync(string id, string decryptedNewPassword)
        {
            try
            {
                var currentUser = await userManager.FindByIdAsync(id);   //userManager needs to be initialized in the Constructor
                if (currentUser != null)
                {
                    var removeCurrentCredentials = await userManager.RemovePasswordAsync(currentUser);
                    if (removeCurrentCredentials.Succeeded)
                    {
                        var newPassword = string.IsNullOrWhiteSpace(decryptedNewPassword) ? RandomTemporaryCredentialsGeneration() : decryptedNewPassword;
                        var addNewCredentials = await userManager.AddPasswordAsync(currentUser, newPassword);
                        if (addNewCredentials.Succeeded)
                        {
                            return newPassword;
                        }
                        else
                        {
                            logger.LogWarning("WARNING inside ChangeCredentialsAsync - Unable to Add Current Users New Credentials.");
                        }
                    }
                    else
                    {
                        logger.LogWarning("WARNING inside ChangeCredentialsAsync - Unable to Remove Current Users Credentials.");
                    }
                }
                else
                {
                    logger.LogWarning("WARNING inside ChangeCredentialsAsync - userManager.Users did not contain the Current User.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception inside ChangeCredentialsAsync.");
            }
            return string.Empty;
        }

        private string RandomTemporaryCredentialsGeneration()
        {
            var opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] { "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz", "0123456789", "!@#$%^&*" };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);   //RequireUppercase
            chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);   //RequireLowercase
            chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);   //RequireDigit
            chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);   //RequireNonAlphanumeric

            for (int i = chars.Count; i < opts.RequiredLength; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        #region ENCRYPT/DECRYPT
        public async Task<string> DecryptStringAES(string cipherText)
        {
            try
            {
                logger.LogInformation("INSIDE " + System.Reflection.MethodBase.GetCurrentMethod());
                var keybytes = Encoding.UTF8.GetBytes(appSettings.EncryptionDecryptionKey);
                var iv = Encoding.UTF8.GetBytes(appSettings.EncryptionDecryptionKey);

                var encrypted = Convert.FromBase64String(cipherText);
                var decriptedFromJavascript = await DecryptStringFromBytes(encrypted, keybytes, iv);
                return decriptedFromJavascript;

            }
            catch (Exception ex)
            {
                var t1 = ex.ToString(); //TODO - LOG Error
                throw;
            }
        }

        private async Task<string> DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch (Exception exception)
                {
                    plaintext = exception.ToString();
                }
            }

            return plaintext;
        }

        public async Task<string> EncryptStringAES(string plainText)
        {
            var keybytes = Encoding.UTF8.GetBytes(appSettings.EncryptionDecryptionKey);
            var iv = Encoding.UTF8.GetBytes(appSettings.EncryptionDecryptionKey);

            var encryoFromJavascript = await EncryptStringToBytes(plainText, keybytes, iv);
            return Convert.ToBase64String(encryoFromJavascript);
        }

        public async Task<byte[]> EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        #endregion

        ///// <summary>
        ///// In some cases, you might need to validate tokens without using the JwtBearer middleware. Using the middleware should always be the first choice,
        /////     since it plugs nicely (and automatically) into the ASP.NET Core authorization system. If you absolutely need to validate a JWT by hand,
        /////     you can use the JwtSecurityTokenHandler in the System.IdentityModel.Tokens.Jwt package.
        /////     It uses the same TokenValidationParameters class to specify the validation options
        ///// </summary>
        ///// <param name="jwt"></param>
        ///// <param name="signingKeys"></param>
        ///// <returns></returns>
        //private static JwtSecurityToken ValidateAndDecode(string jwt, System.Collections.Generic.IEnumerable<SecurityKey> signingKeys)
        //{
        //    var validationParameters = new TokenValidationParameters
        //    {
        //        // Clock skew compensates for server time drift.
        //        // We recommend 5 minutes or less:
        //        ClockSkew = TimeSpan.FromMinutes(5),
        //        // Specify the key used to sign the token:
        //        IssuerSigningKeys = signingKeys,
        //        RequireSignedTokens = true,
        //        // Ensure the token hasn't expired:
        //        RequireExpirationTime = true,
        //        ValidateLifetime = true,
        //        // Ensure the token audience matches our audience value (default true):
        //        ValidateAudience = true,
        //        ValidAudience = "api://default",
        //        // Ensure the token was issued by a trusted authorization server (default true):
        //        ValidateIssuer = true,
        //        ValidIssuer = "https://{yourOktaDomain}/oauth2/default"
        //    };

        //    try
        //    {
        //        var claimsPrincipal = new JwtSecurityTokenHandler()
        //            .ValidateToken(jwt, validationParameters, out var rawValidatedToken);

        //        return (JwtSecurityToken)rawValidatedToken;
        //        // Or, you can return the ClaimsPrincipal
        //        // (which has the JWT properties automatically mapped to .NET claims)
        //    }
        //    catch (SecurityTokenValidationException stvex)
        //    {
        //        // The token failed validation!
        //        // TODO: Log it or display an error.
        //        throw new Exception($"Token failed validation: {stvex.Message}");
        //    }
        //    catch (ArgumentException argex)
        //    {
        //        // The token was not well-formed or was invalid for some other reason.
        //        // TODO: Log it or display an error.
        //        throw new Exception($"Token was invalid: {argex.Message}");
        //    }
        //}
    }
}
