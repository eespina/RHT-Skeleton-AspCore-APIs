using System;

namespace AspCoreBase.ViewModels
{
	public class ExampleViewModel
	{
		public System.Guid ExampleId { get; set; }
		public string ExampleCharacteristic { get; set; }
		public bool IsActive { get; set; }
		public Guid ModifiedBy { get; set; }//TODO, next time, make this NULLABLE?
		public DateTime ModifiedDate { get; set; }//TODO, next time, make this NULLABLE?
		public Guid CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
