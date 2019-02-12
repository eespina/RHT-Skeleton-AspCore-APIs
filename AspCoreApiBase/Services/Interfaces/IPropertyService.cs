using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Services.Interfaces
{
	public interface IPropertyService
	{
		Task<IEnumerable<PropertyViewModel>> GetProperties();
		Task<bool> CreatePropertyUserConnection(OwnerViewModel u);
	}
}
