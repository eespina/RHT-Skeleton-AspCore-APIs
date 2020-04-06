using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreBase.Data.Entities;

namespace AspCoreBase.Data
{
	public interface IVillageDbRepository
	{
		Task<List<OwnerUser>> GetVillageUserOwners();
        Task<OwnerUser> GetVillageUserOwner(string userName);
        Task<List<Property>> GetProperties();
		Task<Property> GetProperty(string propertyName);
		Task<bool> SaveAllAsync();
		void AddEntity(object model);
	}
}
