using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExampleApi.Data;
using ExampleApi.Data.Entities;
using ExampleApi.Services.Interfaces;
using ExampleApi.ViewModels;
using System;
using System.Linq;

namespace ExampleApi.Services
{
	public class ExampleService : IExampleService
	{
		private readonly ILogger<ExampleService> logger;
		private readonly IExampleDbRepository exampleDbRepository;
		private readonly IMapper mapper;

		public ExampleService(ILogger<ExampleService> logger, IExampleDbRepository exampleDbRepository, IMapper mapper)
		{
			this.logger = logger;
			this.exampleDbRepository = exampleDbRepository;
			this.mapper = mapper;
		}

		public async Task<IEnumerable<ExampleViewModel>> GetExamples()
		{
			try
			{
				var examples = await exampleDbRepository.GetExamples();
				if (examples != null)
				{
					if (examples.Any())
					{
						var examplesMapped = mapper.Map<IEnumerable<Example>, IEnumerable<ExampleViewModel>>(examples);
						return examplesMapped;
					}
					else
					{
						logger.LogWarning("WARNING inside ExampleService.GetExamples - 0 Examples returned.");
					}
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

		public async Task<ExampleViewModel> UpdateExample(ExampleViewModel exampleViewModel)
		{
			var exampleMapped = new ExampleViewModel();
			try
			{
                var example = await exampleDbRepository.GetExample(exampleViewModel.ExampleId.ToString());
                if (example != null)
                {
                    example.IsActive = exampleViewModel.IsActive;
                    example.ExampleCharacteristic = exampleViewModel.ExampleCharacteristic;
                    example.ModifiedBy = exampleViewModel.ModifiedBy;
                    example.ModifiedDate = exampleViewModel.ModifiedDate;

                    if (await exampleDbRepository.SaveAllAsync())
					{
						exampleMapped = mapper.Map<Example, ExampleViewModel>(example);
					}
				}
                else
                {
                    logger.LogWarning("WARNING inside ExampleService.GetExamples - Examples is NULL.");
                }

				return exampleMapped;
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
