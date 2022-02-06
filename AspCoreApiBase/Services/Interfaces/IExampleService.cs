using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Services.Interfaces
{
	public interface IExampleService
	{
		Task<IEnumerable<ExampleViewModel>> GetExamples();
		Task<ExampleViewModel> GetExample(string exampleName);
		Task<bool> CreateExampleUserConnection(OwnerViewModel u);
	}
}
