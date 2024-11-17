using AutoMapper;
using DataConnectorLibraryProject.Models;
using WebApiProject.ModelsDTO;

namespace WebApiProject.MappingData
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {

            CreateMap<Customer, CustomerDTO>();
            CreateMap<CustomerDTO, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore())
                .ForMember(dest => dest.Vehicles, opt => opt.Ignore());

        }
    }
}
