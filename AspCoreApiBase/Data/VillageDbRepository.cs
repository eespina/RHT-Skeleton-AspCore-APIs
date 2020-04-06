using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspCoreBase.Data.Entities;
using System;

namespace AspCoreBase.Data
{
	public class VillageDbRepository : IVillageDbRepository
	{
		private readonly VillageDbContext ctx;
		private readonly ILogger<VillageDbRepository> logger;

		public VillageDbRepository(VillageDbContext ctx, ILogger<VillageDbRepository> logger)
		{
			this.ctx = ctx;
			this.logger = logger;
		}

		public async Task<List<OwnerUser>> GetVillageUserOwners()
		{
			try
			{
				var allOwnerUsers = await ctx.OwnerUser.Where(u => u.IsActive).ToListAsync();
				return allOwnerUsers;
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside VillageDbRepository.GetVillageUserOwners - " + ex);
				return null;
			}
        }

        public async Task<OwnerUser> GetVillageUserOwner(string userName)
        {
            try
            {
                var allOwnerUsers = await ctx.OwnerUser.Where(u => u.IsActive && u.UserName == userName).ToListAsync();
                return allOwnerUsers.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR inside VillageDbRepository.GetVillageUserOwner - " + ex);
                return null;
            }
        }

        public async Task<List<Property>> GetProperties()
		{
			try
			{
				var allProperties = await ctx.Property.Where(u => u.IsActive).ToListAsync();
				return allProperties;
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside VillageDbRepository.GetProperties - " + ex);
				return null;
			}
		}

		public async Task<Property> GetProperty(string propertyName)
		{
			try
			{
				var property = await ctx.Property.FirstAsync(p => p.BuildingName == propertyName);
				return property;
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside VillageDbRepository.GetProperty - " + ex);
				return null;
			}
		}

		public async Task<bool> SaveAllAsync()
		{
			return await ctx.SaveChangesAsync() > 0;
		}

		public void AddEntity(object model)
		{
			ctx.Add(model);
		}
	}
}
