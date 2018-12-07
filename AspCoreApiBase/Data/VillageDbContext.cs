using Microsoft.EntityFrameworkCore;
using AspCoreBase.Data.Entities;

namespace AspCoreBase.Data
{
	public class VillageDbContext : DbContext
	{
		public VillageDbContext(DbContextOptions<VillageDbContext> options) : base(options) { }
		public DbSet<AdminUser> AdminUser { get; set; }
		public DbSet<OwnerUser> OwnerUser { get; set; }
		public DbSet<Property> Property { get; set; }
		public DbSet<UserProperty> UserProperty { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			/*	May NOT Need these below as there may NOT be a need to have a bridge table to map/match User and Property
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

			modelBuilder.Entity<UserProperty>()
				.HasOne(p => p.OwnerUser)
				.WithMany(b => b.UserProperties)
				.HasForeignKey(p => p.OwnerUserId)
				.HasConstraintName("ForeignKey_UserProperty_OwnerUser");
			modelBuilder.Entity<UserProperty>()
				.HasOne(p => p.Property)
				.WithMany(b => b.UserProperties)
				.HasForeignKey(p => p.PropertyId)
				.HasConstraintName("ForeignKey_UserProperty_Property");

				END OF - May NOT Need these below as there may NOT be a need to have a bridge table to map/match User and Property */
		}

	}
}
