using System;
using System.ComponentModel.DataAnnotations;

namespace AspCoreApiTemplate.Data.Entities
{
	public class AdminUser  //TODO - inherit from User User (which should act/be an abstract class with, perhaps, an associated Interface)
	{
		[Key]
		public Guid AdminUserId { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Notes { get; set; }
		public bool IsActive { get; set; }
		public DateTime StartDate { get; set; }
		public Guid CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public Guid ModifiedBy { get; set; }
		public DateTime ModifiedDate { get; set; }

		//public List<OwnerUser> OwnerUsers { get; set; }
		//public List<AdminUser> AdminUsers { get; set; }
	}
}
