using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreBase.Data.Entities;

namespace AspCoreBase.Data
{
	public interface IExampleDbRepository
	{
		Task<List<OwnerUser>> GetExampleUserOwners();
        Task<OwnerUser> GetExampleUserOwner(string userName);
        Task<List<Example>> GetExamples();
		Task<Example> GetExample(string exampleName);
		Task<bool> SaveAllAsync();
		void AddEntity(object model);
	}
}
