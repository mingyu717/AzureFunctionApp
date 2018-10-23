using AutoMapper;
using Service.Contract.Models;

namespace Service.Implemention
{
    public static class AutoMapper
    {
        private static IMapper _mapper;
        public static IMapper Instance => _mapper ?? (_mapper = InitialiseAutoMapper());

        private static IMapper InitialiseAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GetRecommendedServicesRequest, GetCdkRecommendedServiceRequest>();
                cfg.CreateMap<DealerConfigurationResponse, GetCdkRecommendedServiceRequest>();
                cfg.CreateMap<CdkVehicleService, ServiceResponse>()
                    .ForMember(dest => dest.Name, opt => opt.ResolveUsing(src => src.JobDescription))
                    .ForMember(dest => dest.ServiceCode, opt => opt.ResolveUsing(src => src.JobCode))
                    .ForMember(dest => dest.Description, opt => opt.ResolveUsing(src => src.JobExtDescription))
                    .ForMember(dest => dest.ServiceTime, opt => opt.ResolveUsing(src => src.JobTime))
                    .ForMember(dest => dest.Price, opt => opt.ResolveUsing(src => src.JobPrice));
            });

            return config.CreateMapper();
        }
    }
}
