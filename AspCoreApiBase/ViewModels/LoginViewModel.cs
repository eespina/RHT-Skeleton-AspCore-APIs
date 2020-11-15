using AspCoreApiBase.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace AspCoreBase.ViewModels
{
	public class LoginViewModel
	{
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
        public string ChangedCredentialString { get; set; }
		public bool RememberMe { get; set; }
        public ErrorViewModel Error { get; set; }
    }
}
