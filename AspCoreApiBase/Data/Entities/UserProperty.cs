using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspCoreBase.Data.Entities
{
	public class UserProperty
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public Guid UserPropertyId { get; set; }

		public Guid UserId { get; set; }
		public int UserTypeId { get; set; }
		public Guid PropertyId { get; set; }
		public DateTime? MoveInDate { get; set; }
		public DateTime? MoveOutDate { get; set; }

		public Guid ModifiedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		public Guid CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
