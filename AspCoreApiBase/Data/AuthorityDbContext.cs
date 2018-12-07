using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AspCoreBase.Data.Entities.Authority;

namespace AspCoreBase.Data
{
	public class AuthorityDbContext : IdentityDbContext<AuthorityUser>
	{
		//Constructor needed so that, when the contexct is being added, its taking those 'options' that we sdpecified (in the startup.cs)
		//and passing them into the context so that it know what connection string to use
		public AuthorityDbContext(DbContextOptions<AuthorityDbContext> options) : base(options) { }
		public DbSet<AuthorityUser> AuthorityUser { get; set; }
	}
}
