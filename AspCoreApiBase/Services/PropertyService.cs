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
	public class PropertyService : IPropertyService
	{
		private readonly ILogger<PropertyService> logger;
		private readonly IExampleDbRepository exampleDbRepository;
		private readonly IMapper mapper;

		public PropertyService(ILogger<PropertyService> logger, IExampleDbRepository exampleDbRepository, IMapper mapper)
		{
			this.logger = logger;
			this.exampleDbRepository = exampleDbRepository;
			this.mapper = mapper;
		}

		public async Task<IEnumerable<PropertyViewModel>> GetProperties()
		{
			try
			{
				var properties = await exampleDbRepository.GetProperties();
				if (properties != null)
				{
					if (properties.Count > 0)
					{
						var propertiesMapped = mapper.Map<IEnumerable<Property>, IEnumerable<PropertyViewModel>>(properties);
						return propertiesMapped;
					}
					else
					{
						logger.LogWarning("WARNING inside PropertyService.GetProperties - 0 Properties returned.");
					}
				}
				else
				{
					logger.LogWarning("WARNING inside PropertyService.GetProperties - Properties is NULL.");
				}

				return null;
			}
			catch (System.Exception ex)
			{
				logger.LogError("ERROR inside PropertyService.GetProperties when calling it's Service counterpart - " + ex);
				return null;
			}
		}

		public async Task<PropertyViewModel> GetProperty(string propertyName)
		{
			try
			{
				var property = await exampleDbRepository.GetProperty(propertyName);
				if (property != null)
				{
					var firstOne = property;
					var propertyMapped = mapper.Map<Property, PropertyViewModel>(property);
					return propertyMapped;
				}
				else
				{
					logger.LogWarning("WARNING inside PropertyService.GetProperties - Properties is NULL.");
				}

				return null;
			}
			catch (System.Exception ex)
			{
				logger.LogError("ERROR inside PropertyService.GetProperties when calling it's Service counterpart - " + ex);
				return null;
			}
		}

		public async Task<bool> CreatePropertyUserConnection(OwnerViewModel user)
		{
			//get the property with the existing 'u' UserViewModel
			var property = await GetProperty(user.PropertyName);

			if (property != null)
			{
				//save the userProperty with the PropertyId
				exampleDbRepository.AddEntity(new UserProperty
				{
					UserPropertyId = new Guid(),
					UserId = user.UserId,
					UserTypeId = user.UserType.Id,
					PropertyId = property.PropertyId,
					MoveInDate = DateTime.Now,
					MoveOutDate = null,
					ModifiedBy = user.CurrentAdministeringUser,
					ModifiedDate = DateTime.Now,
					CreatedBy = user.CurrentAdministeringUser,
					CreatedDate = DateTime.Now
				});

				return await exampleDbRepository.SaveAllAsync();
			}

			logger.LogWarning("ERROR inside PropertyService.CreatePropertyUserConnection - Could Not find Property to associate to new User " + user.UserName + "!");
			return false;
		}
	}
}
