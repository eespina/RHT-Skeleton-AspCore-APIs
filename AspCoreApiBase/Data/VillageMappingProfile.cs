using AutoMapper;
using AspCoreBase.Data.Entities;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Data
{
	public class VillageProfile : Profile
	{
		public VillageProfile()
		{
			CreateMap<OwnerUser, OwnerViewModel>()
				.ForMember(o => o.Email, ex => ex.MapFrom(o => o.Email))
				.ReverseMap();
			CreateMap<AdminUser, OwnerViewModel>()
				.ForMember(o => o.Email, ex => ex.MapFrom(o => o.Email))
				.ReverseMap();
		}
	}
}
