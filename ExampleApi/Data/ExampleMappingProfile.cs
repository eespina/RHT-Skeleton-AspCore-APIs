using AutoMapper;
using ExampleApi.Data.Entities;
using ExampleApi.ViewModels;

namespace ExampleApi.Data
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
