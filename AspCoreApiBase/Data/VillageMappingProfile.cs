using AutoMapper;
using AspCoreBase.Data.Entities;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Data
{
	public class VillageProfile : Profile
	{
		public VillageProfile()
		{
			CreateMap<OwnerUser, UserViewModel>()
				.ForMember(o => o.Email, ex => ex.MapFrom(o => o.Email))
				.ReverseMap();
			CreateMap<AdminUser, UserViewModel>()
				.ForMember(o => o.Email, ex => ex.MapFrom(o => o.Email))
				.ReverseMap();
		}
	}
}
