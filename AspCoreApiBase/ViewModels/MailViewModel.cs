namespace AspCoreBase.ViewModels
{
	public class MailViewModel
    {
		public string Body { get; set; }
		public string Subject { get; set; }
		public System.Text.Encoding SubjectEncoding { get; set; }
		public bool IsBodyHtml { get; set; }//Text vs HTML
		public System.Net.Mail.MailAddress ReplyToList { get; set; }
	}
}
