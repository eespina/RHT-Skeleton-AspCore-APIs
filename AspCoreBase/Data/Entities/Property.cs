using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspCoreBase.Data.Entities
{
    public class Property
	{
		[Key]
		public Guid PropertyId { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string Address3 { get; set; }
		public string UnitNumber { get; set; }
		public string BuildingName { get; set; }
		public string City { get; set; }
		public string Country { get; set; }
		public string County { get; set; }
		public bool IsActive { get; set; }
		public string Province { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public Guid ModifiedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		public Guid CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }

		//public ICollection<UserProperty> UserProperties { get; set; }
	}
}
