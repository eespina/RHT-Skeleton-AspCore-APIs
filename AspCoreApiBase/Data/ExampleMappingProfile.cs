using AutoMapper;
using AspCoreBase.Data.Entities;
using AspCoreBase.ViewModels;

namespace AspCoreBase.Data
{
	public class ExampleProfile : Profile
	{
		public ExampleProfile()
		{
			CreateMap<OwnerUser, OwnerViewModel>()
				.ForMember(o => o.Email, ex => ex.MapFrom(o => o.Email))
				.ReverseMap();
			CreateMap<AdminUser, OwnerViewModel>()
				.ForMember(o => o.Email, ex => ex.MapFrom(o => o.Email))
				.ReverseMap();
			CreateMap<Example, ExampleViewModel>()
				.ForMember(e => e.ExampleId, ex => ex.MapFrom(e => e.ExampleId))
				.ReverseMap();
		}
	}
}
