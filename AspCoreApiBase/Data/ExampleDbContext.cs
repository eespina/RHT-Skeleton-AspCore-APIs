using Microsoft.EntityFrameworkCore;
using AspCoreBase.Data.Entities;

namespace AspCoreBase.Data
{
    public class ExampleDbContext : DbContext
    {
        public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options) { }
        public DbSet<AdminUser> AdminUser { get; set; }
        public DbSet<OwnerUser> OwnerUser { get; set; }
        public DbSet<Example> Example { get; set; }
        public DbSet<UserExample> UserExample { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*	May NOT Need these below as there may NOT be a need to have a bridge table to map/match User and Example
			 *	
			modelBuilder.Entity<OwnerUser>()
				.HasOne(p => p.AdminUser)
				.WithMany(b => b.OwnerUsers)
				.HasForeignKey(p => p.CreatedBy)
				.HasConstraintName("ForeignKey_OwnerUser_AdminUser_CreatedBy");
			modelBuilder.Entity<OwnerUser>()
				.HasOne(p => p.AdminUser)
				.WithMany(b => b.OwnerUsers)
				.HasForeignKey(p => p.ModifiedBy)
				.HasConstraintName("ForeignKey_OwnerUser_AdminUser_ModifiedBy");

			modelBuilder.Entity<UserExample>()
				.HasOne(p => p.OwnerUser)
				.WithMany(b => b.UserExamples)
				.HasForeignKey(p => p.OwnerUserId)
				.HasConstraintName("ForeignKey_UserExample_OwnerUser");
			modelBuilder.Entity<UserExample>()
				.HasOne(p => p.Example)
				.WithMany(b => b.UserExamples)
				.HasForeignKey(p => p.Example)
				.HasConstraintName("ForeignKey_UserExample_Example");

				END OF - May NOT Need these below as there may NOT be a need to have a bridge table to map/match User and Example */

            /*
				//SEED data option - usefull if we just want to use test data instead of live data
                modelBuilder.Entity<OwnerUser>().HasData(
                new OwnerUser
                {
                    FirstName = "Test Owner 1",
                    Email = "test_1@email.com"
                },
                new OwnerUser
                {
                    FirstName = "Test Owner 2",
                    Email = "test_2@email.com"
                }
                );
            */
        }

    }
}
