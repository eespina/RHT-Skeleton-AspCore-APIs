using System.Collections.Generic;
using System.Threading.Tasks;
using ExampleApi.ViewModels;

namespace ExampleApi.Services.Interfaces
{
	public interface IExampleService
	{
		Task<IEnumerable<ExampleViewModel>> GetExamples();
		Task<ExampleViewModel> GetExample(string exampleName);
		Task<ExampleViewModel> UpdateExample(ExampleViewModel exampleViewModel);
		Task<ExampleViewModel> CreateExample(ExampleViewModel exampleViewModel);
		Task<bool> DeleteExample(string exampleId);
	}
}
