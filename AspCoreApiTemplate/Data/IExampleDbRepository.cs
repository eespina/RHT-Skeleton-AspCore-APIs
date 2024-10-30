using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreApiTemplate.Data.Entities;

namespace AspCoreApiTemplate.Data
{
	/// <summary>
	/// This SHOULD be moved into it's own MicroService API including everything alongside it.
	/// </summary>
	public interface IExampleDbRepository
	{
		//Task<List<OwnerUser>> GetExampleUserOwners();
		//Task<OwnerUser> GetExampleUserOwnerByOwnerUserId(string ownerUserId);
		//Task<OwnerUser> GetExampleUserOwnerByUserName(string userName);
		//Task<OwnerUser> GetExampleUserOwnerByEmail(string email);
		Task<List<Example>> GetExamples();
		Task<Example> GetExample(string exampleName);
		Task<bool> SaveAllAsync();
		Task AddEntity(object model);
        Task UpdateEntity(object model);
        Task DeleteEntityAsync(object mode);
	}
}
