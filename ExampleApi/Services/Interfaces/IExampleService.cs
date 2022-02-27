using System.Collections.Generic;
using System.Threading.Tasks;
using ExampleApi.ViewModels;

namespace ExampleApi.Services.Interfaces
{
	public interface IExampleService
	{
		Task<IEnumerable<ExampleViewModel>> GetExamples();
		Task<ExampleViewModel> UpdateExample(ExampleViewModel exampleViewModel);
	}
}
