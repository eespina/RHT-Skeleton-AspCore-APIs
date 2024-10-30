using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreApiTemplate.Data;
using AspCoreApiTemplate.Data.Entities;
using AspCoreApiTemplate.Services.Interfaces;
using AspCoreApiTemplate.ViewModels;
using System;
using System.Linq;

namespace AspCoreApiTemplate.Services
{
	/// <summary>
	/// This SHOULD be moved into it's own MicroService API including everything alongside it.
	/// </summary>
	public class ExampleService : IExampleService
	{
		private readonly ILogger<ExampleService> logger;
		private readonly IExampleDbRepository exampleDbRepository;
		private readonly IMapper mapper;
		private readonly IMockIdServerVessel<ExampleViewModel> _mockIdServerVessel;

		public ExampleService(ILogger<ExampleService> logger, IExampleDbRepository exampleDbRepository, IMapper mapper, IMockIdServerVessel<ExampleViewModel> mockIdServerVessel)
		{
			this.logger = logger;
			this.exampleDbRepository = exampleDbRepository;
			this.mapper = mapper;
			this._mockIdServerVessel = mockIdServerVessel;
		}

		public async Task<IEnumerable<ExampleViewModel>> GetExamples()
		{
			try
			{
				//MOCK the ID server behaviour and just call the Example API
				return await _mockIdServerVessel.SendMockGetRequest("https://localhost:44372/api/Example");
			}
			catch (System.Exception ex)
			{
				logger.LogError("ERROR inside ExampleService.GetExamples when calling it's Service counterpart - " + ex);
				return null;
			}
		}

		public async Task<ExampleViewModel> GetExample(string exampleId)
		{
			try
			{
				var example = await exampleDbRepository.GetExample(exampleId);
				if (example != null)
				{
					var exampleMapped = mapper.Map<Example, ExampleViewModel>(example);
					return exampleMapped;
				}
				else
				{
					logger.LogWarning("WARNING inside ExampleService.GetExamples - Examples is NULL.");
				}

				return null;
			}
			catch (System.Exception ex)
			{
				logger.LogError("ERROR inside ExampleService.GetExamples when calling it's Service counterpart - " + ex);
				return null;
			}
		}

		public async Task<ExampleViewModel> CreateExample(ExampleViewModel exampleViewModel)
		{
			try
			{
				var newExample = new Example
				{
					ExampleId = new Guid(),
					ExampleCharacteristic = exampleViewModel.ExampleCharacteristic,
					IsActive = exampleViewModel.IsActive,
					ModifiedBy = exampleViewModel.CreatedBy,//TODO, next time, make this NULLABLE
					ModifiedDate = DateTime.Now,//TODO, next time, make this NULLABLE
					CreatedBy = exampleViewModel.CreatedBy,
					CreatedDate = DateTime.Now
				};

				//save the userExample with the ExampleId
				await exampleDbRepository.AddEntity(newExample);

				await exampleDbRepository.SaveAllAsync();

				exampleViewModel.ExampleId = newExample.ExampleId;

				return exampleViewModel;
			}
			catch (Exception ex)
			{
				logger.LogError("ERROR inside ExampleService.GetExamples when calling it's Service counterpart - " + ex);
				return null;
			}
		}

		public async Task<ExampleViewModel> UpdateExample(ExampleViewModel exampleViewModel)
		{
			try
			{
				//TODO - MOCK the ID server behaviour and just call the Example API
				var mockReturnObject = await _mockIdServerVessel.SendMockPutRequest(exampleViewModel, "https://localhost:44372/api/Example/");
				return mockReturnObject;
			}
			catch (System.Exception ex)
			{
				logger.LogError("ERROR inside ExampleService.GetExamples when calling it's Service counterpart - " + ex);
				return null;
			}
		}

		public async Task<bool> DeleteExample(string exampleId)
		{
			try
			{
				var example = await exampleDbRepository.GetExample(exampleId);
				if (example != null)
				{
					await exampleDbRepository.DeleteEntityAsync(example);

					return await exampleDbRepository.SaveAllAsync();
				}
				else
				{
					logger.LogWarning("WARNING inside ExampleService.GetExamples - Examples is NULL.");
				}

				return false;
			}
			catch (System.Exception ex)
			{
				logger.LogError("ERROR inside ExampleService.GetExamples when calling it's Service counterpart - " + ex);
				return false;
			}
		}
	}
}
