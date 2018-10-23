using AutoMapper;
using Service.Contract;
using Service.Contract.Request;
using Service.Contract.Response;
using Service.Implementation;
using Unity;
using Unity.Injection;

namespace ConfigurationManagerService
{
    public class UnityContainer
    {
        private static IUnityContainer _unityContainer;

        private static readonly string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["sql_connection"].ConnectionString;

        public static IUnityContainer Instance => _unityContainer ?? (_unityContainer = InitialiseUnityContainer());

        private static IUnityContainer InitialiseUnityContainer()
        {
            var container = new Unity.UnityContainer();

            container.RegisterType<IValidateRequest, ValidateRequest>();
            container.RegisterType<IDealerConfigurationDAL, DealerConfigurationDAL>(new InjectionConstructor(ConnectionString));
            container.RegisterType<IDealerConfigurationService, DealerConfigurationService>();

            RegisterAutoMapper(container);
            return container;
        }

        private static void RegisterAutoMapper(Unity.UnityContainer container)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DealerConfigurationCreateRequest, DealerConfiguration>();
                cfg.CreateMap<DealerConfigurationUpdateRequest, DealerConfiguration>();
                cfg.CreateMap<DealerConfiguration, DealerConfigurationResponse>();
                cfg.CreateMap<DealerConfiguration, DealerInvitationContentResponse>();
            });
            var mapper = config.CreateMapper();
            container.RegisterInstance(mapper);
        }
    }
}
