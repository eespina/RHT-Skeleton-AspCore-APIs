using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreApiTemplate.ViewModels;

namespace AspCoreApiTemplate.Services.Interfaces
{
	public interface IExampleService
	{
		Task<IEnumerable<ExampleViewModel>> GetExamples();
		Task<ExampleViewModel> GetExample(string exampleName);
		Task<bool> CreateExampleUserConnection(OwnerViewModel u);
		Task<ExampleViewModel> CreateExample(ExampleViewModel exampleViewModel);
		Task<ExampleViewModel> UpdateExample(ExampleViewModel exampleViewModel);
		Task<bool> DeleteExample(string exampleId);
	}
}
