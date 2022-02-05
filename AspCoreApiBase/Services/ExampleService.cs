using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreBase.Data;
using AspCoreBase.Data.Entities;
using AspCoreBase.Services.Interfaces;
using AspCoreBase.ViewModels;
using System;

namespace AspCoreBase.Services
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
					if (examples.Count > 0)
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

		public async Task<ExampleViewModel> GetExamples(string exampleName)
		{
			try
			{
				var example = await exampleDbRepository.GetExample(exampleName);
				if (example != null)
				{
					var firstOne = example;
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

		public async Task<bool> CreateExampleUserConnection(OwnerViewModel user)
		{
			//get the example with the existing 'u' UserViewModel
			var example = await GetExamples(user.ExampleName);

			if (example != null)
			{
				//save the userExample with the ExampleId
				exampleDbRepository.AddEntity(new UserExample
				{
					UserExampleId = new Guid(),
					UserId = user.UserId,
					UserTypeId = user.UserType.Id,
					ExampleId = example.ExampleId,
					ModifiedBy = user.CurrentAdministeringUser,
					ModifiedDate = DateTime.Now,
					CreatedBy = user.CurrentAdministeringUser,
					CreatedDate = DateTime.Now
				});

				return await exampleDbRepository.SaveAllAsync();
			}

			logger.LogWarning("ERROR inside ExampleService.CreateExampleUserConnection - Could Not find Example to associate to new User " + user.UserName + "!");
			return false;
		}
	}
}
