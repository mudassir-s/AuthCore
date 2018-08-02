using AuthCore.Models.Entities;
using AutoMapper;

namespace AuthCore.ViewModels.Mappings
{
    public class ViewModelToEntityMappingProfile : Profile
    {
        public ViewModelToEntityMappingProfile()
        {
            CreateMap<RegistrationViewModel, User>().ForMember(
                au => au.UserName,
                map => map.MapFrom(vm => vm.Email)
            );
        }
    }
}