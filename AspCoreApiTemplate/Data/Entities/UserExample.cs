using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspCoreApiTemplate.Data.Entities
{
	public class UserExample
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public Guid UserExampleId { get; set; }

		public Guid UserId { get; set; }
		public int UserTypeId { get; set; }
		public Guid ExampleId { get; set; }

		public Guid ModifiedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		public Guid CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
