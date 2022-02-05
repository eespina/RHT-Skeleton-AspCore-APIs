using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspCoreBase.Data.Entities;
using System;

namespace AspCoreBase.Data
{
	public class ExampleDbRepository : IExampleDbRepository
	{
		private readonly ExampleDbContext ctx;
		private readonly ILogger<ExampleDbRepository> logger;

		public ExampleDbRepository(ExampleDbContext ctx, ILogger<ExampleDbRepository> logger)
		{
			this.ctx = ctx;
			this.logger = logger;
		}

		public async Task<List<OwnerUser>> GetExampleUserOwners()
		{
			try
			{
				var allOwnerUsers = await ctx.OwnerUser.Where(u => u.IsActive).ToListAsync();
				return allOwnerUsers;
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside ExampleDbRepository.GetExampleUserOwners - " + ex);
				return null;
			}
        }

        public async Task<OwnerUser> GetExampleUserOwner(string userName)
        {
            try
            {
                var allOwnerUsers = await ctx.OwnerUser.Where(u => u.IsActive && u.UserName == userName).ToListAsync();
                return allOwnerUsers.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR inside ExampleDbRepository.GetExampleUserOwner - " + ex);
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
				logger.LogError("ERROR inside ExampleDbRepository.GetProperties - " + ex);
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
				logger.LogError("ERROR inside ExampleDbRepository.GetProperty - " + ex);
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
