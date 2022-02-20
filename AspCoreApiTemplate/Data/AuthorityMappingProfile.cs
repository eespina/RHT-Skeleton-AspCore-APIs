using AutoMapper;
using AspCoreApiTemplate.Data.Entities;
using AspCoreApiTemplate.ViewModels;

namespace AspCoreApiTemplate.Data
{
	public class AuthorityProfile : Profile
	{
		public AuthorityProfile()
		{
			CreateMap<OwnerUser, OwnerViewModel>()
				.ReverseMap();
		}
	}
}
