using AutoMapper;
using DataConnectorLibraryProject.Models.ModelsDto;
using DataConnectorLibraryProject.Models.ServerSideModels;

namespace DataConnectorLibraryProject.MappingData
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {

            CreateMap<Customer, CustomerDto>();

            CreateMap<CustomerDto, Customer>()
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.Position, opt => opt.Ignore())
                .ForMember(dest => dest.Employees, opt => opt.Ignore())
                .ForMember(dest => dest.Vehicles, opt => opt.Ignore());

            CreateMap<CustomerInputDto, Customer>();
        }
    }
}