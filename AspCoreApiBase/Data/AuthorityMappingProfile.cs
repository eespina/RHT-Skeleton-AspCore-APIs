using AutoMapper;
using AspCoreBase.Data.Entities;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Data
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
