using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspCoreBase.Data.Entities
{
	public class OwnerUser  //TODO - inherit from User User (which should act/be an abstract class with, perhaps, an associated Interface)
	{
		[Key]
		public Guid OwnerUserId { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Notes { get; set; }
		public bool IsActive { get; set; }
		public Guid CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public Guid ModifiedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		//public AdminUser AdminUser { get; set; }

		//public List<UserExample> UserExamples { get; set; }
	}
}
