using System;
using System.ComponentModel.DataAnnotations;

namespace AspCoreBase.Data.Entities
{
    public class Example
	{
		[Key]
		public Guid ExampleId { get; set; }
		public string ExampleCharacteristic { get; set; }
		public bool IsActive { get; set; }
		public Guid ModifiedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		public Guid CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }

		//public ICollection<UserExample> UserExamples { get; set; }
	}
}
