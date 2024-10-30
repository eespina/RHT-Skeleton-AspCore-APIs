using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Data.Entities;
using System;

namespace UserApi.Data
{
	/// <summary>
	/// This SHOULD be moved into it's own MicroService API including everything alongside it.
	/// </summary>
	public class ExampleDbRepository : IExampleDbRepository
	{
		private readonly UserDbContext ctx;
		private readonly ILogger<ExampleDbRepository> logger;

		public ExampleDbRepository(UserDbContext ctx, ILogger<ExampleDbRepository> logger)
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

		public async Task<OwnerUser> GetExampleUserOwnerByOwnerUserId(string ownerUserId)
		{
			try
			{
				var ownerUser = await ctx.OwnerUser.FirstOrDefaultAsync(u => u.OwnerUserId.ToString() == ownerUserId);
				return ownerUser;
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside ExampleDbRepository.GetExampleUserOwnerByUserName - " + ex);
				return null;
			}
		}

		public async Task<OwnerUser> GetExampleUserOwnerByUserName(string userName)
        {
            try
            {
                var allOwnerUsers = await ctx.OwnerUser.Where(u => u.IsActive && u.UserName == userName).ToListAsync();
                return allOwnerUsers.FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.LogError("ERROR inside ExampleDbRepository.GetExampleUserOwnerByUserName - " + ex);
                return null;
            }
		}

		public async Task<OwnerUser> GetExampleUserOwnerByEmail(string email)
		{
			try
			{
				var allOwnerUsers = await ctx.OwnerUser.Where(u => u.IsActive && u.Email == email).ToListAsync();
				return allOwnerUsers.FirstOrDefault();
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside ExampleDbRepository.GetExampleUserOwnerByEmail - " + ex);
				return null;
			}
		}

		public async Task<bool> SaveAllAsync()
		{
			return await ctx.SaveChangesAsync() > 0;
		}

		public async Task AddEntity(object model)
		{
			await ctx.AddAsync(model);
		}

		public async Task UpdateEntity(object model)
		{
			ctx.Update(model);
		}

		public async Task DeleteEntityAsync(object model)
        {
			ctx.Remove(model);
        }
    }
}
