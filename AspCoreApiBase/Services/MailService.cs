using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AspCoreBase.Data;
using AspCoreBase.Data.Entities;
using AspCoreBase.Data.Entities.Authority;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Services
{
	public class MailService : IMailService
	{
		private readonly ILogger<MailService> logger;
		private readonly IExampleDbRepository exampleDbRepository;
		private readonly UserManager<AuthorityUser> authorityUser;

		public MailService(UserManager<AuthorityUser> AuthorityUser, ILogger<MailService> logger, IExampleDbRepository repository)
		{
			this.authorityUser = AuthorityUser;
			this.logger = logger;
			this.exampleDbRepository = repository;
		}

		public async Task<bool> SaveEmailComposition(OwnerViewModel user, System.Security.Claims.ClaimsPrincipal currentUser)
		{
			var adminUserId = currentUser.Identities.First().Claims.First(i => i.Type.Split('/').Last() == "nameidentifier").Value;

			if (await authorityUser.FindByIdAsync(adminUserId) != null)
			{
				var invitation = new Invitation
				{
					UserId = user.UserId,
					InvitationTemporaryCredentials = user.TemporaryCredentials,
					InvitationSent = false,
					InvitationSentDate = null,
					InvitationReceived = false,
					InvitationReceivedDate = null,
					InvitationCompleted = false,
					InvitationCompletedDate = null,
					ExpirationDate = DateTime.Now.AddHours(48).Date,
					InvitationEmailBatchRunId = null,
					ManuallyCompleted = false,
					ModifiedBy = new Guid(adminUserId),
					ModifiedDate = DateTime.Now,
					CreatedBy = new Guid(adminUserId),
					CreatedDate = DateTime.Now
				};

				exampleDbRepository.AddEntity(invitation);

				return await exampleDbRepository.SaveAllAsync();
			}

			logger.LogWarning("ERROR inside MailService.SaveEmailComposition - Admin " + user.UserName + " Not created");
			return false;
		}

		//NEEDS AN ACTUAL SMTP SERVER address, credentials, etc..
		public async Task<bool> SendMessage(MailViewModel mailViewModel)
		{
			try
			{
				SmtpClient mySmtpClient = new SmtpClient("smtp.gmail.com", 587)   //google's info, i guess
				{
					// set smtp-client with basicAuthentication
					UseDefaultCredentials = false,
					Credentials = new System.Net.NetworkCredential("username", "password"), //TODO - WHEREVER THIS ENDS UP, use a config file
					EnableSsl = true    //Google uses https
				};

				// add from,to mailaddresses
				MailMessage myMail = new MailMessage(new MailAddress("test@example.com", "TestFromName"), new MailAddress("nebod410s@hotmail.com", "TestToName"))
				{
					Subject = mailViewModel.Subject,
					SubjectEncoding = mailViewModel.SubjectEncoding,
					Body = mailViewModel.Body,
					IsBodyHtml = true
				};

				myMail.ReplyToList.Add(mailViewModel.ReplyToList);

				await mySmtpClient.SendMailAsync(myMail);

				return true;
			}
			catch (SmtpException ex)
			{
				logger.LogError("SmtpException ERROR - " + ex);
				return false;
			}
			catch (Exception ex)
			{
				logger.LogError("MailService.SendMessage ERROR - " + ex);
				return false;
			}
		}
	}
}
