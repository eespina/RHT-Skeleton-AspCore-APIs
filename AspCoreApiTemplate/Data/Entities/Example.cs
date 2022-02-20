using System;
using System.ComponentModel.DataAnnotations;

namespace AspCoreApiTemplate.Data.Entities
{
    public class Example
	{
		[Key]
		public Guid ExampleId { get; set; }
		public string ExampleCharacteristic { get; set; }
		public bool IsActive { get; set; }
		public Guid ModifiedBy { get; set; }//TODO, next time, make this NULLABLE
		public DateTime ModifiedDate { get; set; }//TODO, next time, make this NULLABLE
		public Guid CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }

		//public ICollection<UserExample> UserExamples { get; set; }
	}
}
