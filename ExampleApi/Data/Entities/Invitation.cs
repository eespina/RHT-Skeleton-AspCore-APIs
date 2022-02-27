using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExampleApi.Data.Entities
{
	public class Invitation
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public Guid InvitationId { get; set; }
		public Guid UserId { get; set; }
		public string InvitationTemporaryCredentials { get; set; }
		public bool InvitationSent { get; set; }
		public DateTime? InvitationSentDate { get; set; }
		public bool InvitationReceived { get; set; }
		public DateTime? InvitationReceivedDate { get; set; }
		public bool InvitationCompleted { get; set; }
		public DateTime? InvitationCompletedDate { get; set; }
		public DateTime ExpirationDate { get; set; }
		public Guid? InvitationEmailBatchRunId { get; set; } //TODO - create 'InvitationEmailBatchRun' Table
		public bool ManuallyCompleted { get; set; }
		public Guid ModifiedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		public Guid CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
