using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreBase.Data.Entities;

namespace AspCoreBase.Data
{
	public interface IExampleDbRepository
	{
		Task<List<OwnerUser>> GetExampleUserOwners();
        Task<OwnerUser> GetExampleUserOwner(string userName);
        Task<List<Property>> GetProperties();
		Task<Property> GetProperty(string propertyName);
		Task<bool> SaveAllAsync();
		void AddEntity(object model);
	}
}
