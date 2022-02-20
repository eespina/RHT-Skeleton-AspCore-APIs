using Microsoft.Extensions.Logging;

namespace AspCoreApiTemplate.Data
{
	public class AuthorityDbRepository : IAuthorityDbRepository
	{
		private readonly AuthorityDbContext ctx;
		private readonly ILogger<AuthorityDbRepository> logger;

		public AuthorityDbRepository(AuthorityDbContext ctx, ILogger<AuthorityDbRepository> logger)
		{
			this.ctx = ctx;
			this.logger = logger;
		}

		public bool SaveAll()
		{
			return ctx.SaveChanges() > 0;
		}

		public void AddEntity(object model)
		{
			ctx.Add(model);
		}
	}
}
