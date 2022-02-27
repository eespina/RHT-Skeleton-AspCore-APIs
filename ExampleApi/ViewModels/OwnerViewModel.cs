using ExampleApi.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExampleApi.ViewModels
{
    public class OwnerViewModel
	{
		public Guid UserId { get; set; }

		[Required]
		[MinLength(2, ErrorMessage = "First Name is Too Short!")]
		public string FirstName { get; set; }

		[Required]
		[MinLength(2, ErrorMessage = "Last Name is Too Short!")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "The email address is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }
		[Required]
		public string UserName { get; set; }

		//[Required]
		//[StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
		//[RegularExpression("^((?=.*[a-z])(?=.*[A-Z])(?=.*\\d)).+$", ErrorMessage = "Password must contain All qualifying characters")]
		//public string Password { get; set; }

        //[Required]
        public UserTypeViewModel UserType { get; set; }    //NEEDED in Admin Portal only

        public Guid CurrentAdministeringUser { get; set; }  //this should be null and replaced in the Admin portal
        public string AdministeringUserEmail { get; set; }  //Used mainly fro REGISTRATION of users (trying to get 'User' Claims, but unsucessful)

        [MaxLength(1000, ErrorMessage = "Notes are Too Long!")]
		public string Notes { get; set; }
		//[Required]
		public bool IsActive { get; set; }
        public bool IsChangingCredentials { get; set; }

        public ExampleViewModel Example { get; set; }
		public string ExampleName { get; set; }
		public Guid ExampleId { get; set; }
		public string TemporaryCredentials { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public TokenHandleViewModel tokenHandleViewModel { get; set; }
		public ErrorViewModel Error { get; set; }
	}
}
