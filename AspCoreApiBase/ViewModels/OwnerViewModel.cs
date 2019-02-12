using System;
using System.ComponentModel.DataAnnotations;
using AspCoreBase.Data.Entities;

namespace AspCoreBase.ViewModels
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

		[Required]
		[StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
		[RegularExpression("^((?=.*[a-z])(?=.*[A-Z])(?=.*\\d)).+$", ErrorMessage = "Password must contain All qualifying characters")]
		public string ChosenCredentials { get; set; }

		public UserType UserType { get; set; }

		public string TypeNumber { get; set; }

		public Guid CurrentAdministeringUser { get; set; }

		[MaxLength(1000, ErrorMessage = "Notes are Too Long!")]
		public string Notes { get; set; }
		[Required]
		public bool IsActive { get; set; }

		public PropertyViewModel Property { get; set; }
		public string PropertyName { get; set; }
		public Guid PropertyId { get; set; }
		public string TemporaryCredentials { get; set; }

		[Required]
		public Guid CreatedBy { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; }
		[Required]
		public Guid ModifiedBy { get; set; }
		[Required]
		public DateTime ModifiedDate { get; set; }
	}

	public class UserType
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
