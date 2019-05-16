using AspCoreBase.Data.Entities.Authority;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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

        public AuthenticateService(ILogger<AuthenticateService> logger, UserManager<AuthorityUser> userManager, SignInManager<AuthorityUser> signInManager, IConfiguration config)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.config = config;
            this.userManager = userManager;
        }

        public async Task<SignInResult> PasswordSign(LoginViewModel model)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
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
        public async Task<OwnerViewModel> CreateToken(LoginViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);   //userManager needs to be initialized in the Constructor

            if (user != null)
            {
                OwnerViewModel ownerViewModel = new OwnerViewModel()
                {
                    UserName = model.Username,
                    Email = user.Email
                };

                //figure out if the login actually works
                try
                {
                    var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
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
                                expires: DateTime.UtcNow.AddMinutes(20),
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
                logger.LogWarning(model.Username + " username NOT found.");
            }

            return null;    //No User found Or Password incorrect
        }

        public async Task<bool> ChangeCredentialsAsync(LoginViewModel model)
        {
            try
            {
                var locallyAuthenticateUser = await PasswordSign(model);
                if (locallyAuthenticateUser != null)
                {
                    if (locallyAuthenticateUser.Succeeded)
                    {
                        var currentUser = userManager.Users.First(u => u.UserName == model.Username);
                        if (currentUser != null)
                        {
                            var removeCurrentCredentials = await userManager.RemovePasswordAsync(currentUser);
                            if (removeCurrentCredentials.Succeeded)
                            {
                                removeCurrentCredentials = await userManager.AddPasswordAsync(currentUser, model.ChangedCredentialString);
                                if (removeCurrentCredentials.Succeeded)
                                {
                                    return true;
                                }
                                else
                                {
                                    logger.LogWarning("WARNING inside SettingsController.ProfileManagement.HttpPost - Unable to Add Current Users New Credentials.");
                                }
                            }
                            else
                            {
                                logger.LogWarning("WARNING inside SettingsController.ProfileManagement.HttpPost - Unable to Remove Current Users Credentials.");
                            }

                            logger.LogWarning("WARNING inside SettingsController.ProfileManagement.HttpPost - ChangePasswordAsync method did not work.");
                        }
                        else
                        {
                            logger.LogWarning("WARNING inside SettingsController.ProfileManagement.HttpPost - userManager.Users did not contain the Current User.");
                        }
                    }
                    else
                    {
                        logger.LogWarning("WARNING inside SettingsController.ProfileManagement.HttpPost - Authentication attempt Failed. Credentials may be incorrect");
                    }
                }
                else
                {
                    logger.LogWarning("WARNING inside SettingsController.ProfileManagement.HttpPost - Authentication attempt Failed. Resulting attempt returned NULL for variable currentUser");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("ERROR inside SettingsController.ProfileManagement.HttpPost - User Credential Changing ERROR : " + ex);
            }
            return false;
        }

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
