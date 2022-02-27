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
	}
}
