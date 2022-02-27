using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AspCoreApiTemplate.Data.Entities.Authority;

namespace AspCoreApiTemplate.Data
{
	public class AuthorityDbContext : IdentityDbContext<AuthorityUser>
	{
		//Constructor needed so that, when the contexct is being added, its taking those 'options' that we specified (in the startup.cs)
		//and passing them into the context so that it know what connection string to use
		public AuthorityDbContext(DbContextOptions<AuthorityDbContext> options) : base(options) { }
		public DbSet<AuthorityUser> AuthorityUser { get; set; }
	}
}
