using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Service.Contract;
using Service.Implementation;
using System;
using System.Configuration;
using AutoMapper;
using Service.Contract.Models;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace CDKAutoline.Service
{
    public class UnityContainer
    {
        private static IUnityContainer _unityContainer;

        private static readonly string CdkAutolineUrl = ConfigurationManager.AppSettings["cdkAutolineUrl"];
        private static readonly string UnregisteredGuid = ConfigurationManager.AppSettings["unregisteredGUID"];
        private static readonly string PasswordLength = ConfigurationManager.AppSettings["passwordLength"];
        private static readonly string PasswordCharacters = ConfigurationManager.AppSettings["passwordCharacters"];
        private static readonly string SecretKey = ConfigurationManager.AppSettings["secretKey"];
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["sql_connection"].ConnectionString;

        private static readonly string AppInsightsKey = TelemetryConfiguration.Active.InstrumentationKey =
            ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];

        public static IUnityContainer Instance => _unityContainer ?? (_unityContainer = InitialiseUnityContainer());

        private static IUnityContainer InitialiseUnityContainer()
        {
            var container = new Unity.UnityContainer();
            RegisterTelemetryClient(container);
            RegisterAutoMapper(container);

            container.RegisterType<IAppTokenDAL, AppTokenDAL>(new InjectionConstructor(new CDKAutolineContext(ConnectionString)));
            container.RegisterType<IEncryptionService, RijndaelEncryptionService>(new InjectionConstructor(SecretKey));
            container.RegisterType<IValidateRequest, ValidateRequest>();
            container.RegisterType<IRestApiClient, RestApiClient>();
            container.RegisterType<IPasswordService, PasswordService>(
                new InjectionConstructor(Convert.ToInt32(PasswordLength), PasswordCharacters));
            container.RegisterType<IEncryptedTokenCodeService, EncryptedTokenCodeService>(
                new InjectionConstructor(UnregisteredGuid, container.Resolve<IEncryptionService>()));
            container.RegisterType<ICdkCustomerDAL, CdkCustomerDAL>(
                new InjectionConstructor(new CDKAutolineContext(ConnectionString)));
            container.RegisterType<IDealerCDKConfigurationsDAL, DealerCDKConfigurationsDAL>(
                new InjectionConstructor(new CDKAutolineContext(ConnectionString)));
            container.RegisterType<ICdkCustomerService, CdkCustomerService>();
            container.RegisterType<ICustomerService, CustomerService>(
                new InjectionConstructor(container.Resolve<IRestApiClient>(), CdkAutolineUrl,
                container.Resolve<IEncryptedTokenCodeService>()));
            container.RegisterType<ITokenService, TokenService>(
                new InjectionConstructor(container.Resolve<IRestApiClient>(),
                    container.Resolve<IEncryptedTokenCodeService>(), CdkAutolineUrl,
                    UnregisteredGuid, container.Resolve<IAppTokenDAL>(), container.Resolve<TelemetryClient>(),
                    container.Resolve<ICdkCustomerService>(), container.Resolve<ICustomerService>(),
                    container.Resolve<IDealerCDKConfigurationsDAL>()));
            container.RegisterType<ICDKAutolineServices, CDKAutolineServices>();
            container.RegisterType<ICDKVehicleMaintenanceService, CDKVehicleMaintenanceService>(
                    new InjectionConstructor(container.Resolve<IRestApiClient>(), CdkAutolineUrl,
                    container.Resolve<ITokenService>(), container.Resolve<TelemetryClient>()));
            container.RegisterType<ICDKBookingService, CDKBookingService>(
                new InjectionConstructor(container.Resolve<IRestApiClient>(),
                container.Resolve<ITokenService>(), container.Resolve<ICdkCustomerDAL>(),
                container.Resolve<TelemetryClient>(), CdkAutolineUrl));
            container.RegisterType<ICDKServiceAdvisors, CDKServiceAdvisors>(
                new InjectionConstructor(container.Resolve<IRestApiClient>(), container.Resolve<ITokenService>(),
                container.Resolve<ICdkCustomerDAL>(), container.Resolve<TelemetryClient>(), CdkAutolineUrl));

            container.RegisterType<ICDKAppointmentSlotsService, CDKAppointmentSlotsService>(
                new InjectionConstructor(container.Resolve<IRestApiClient>(),
                container.Resolve<ICdkCustomerDAL>(), container.Resolve<ITokenService>(),
                container.Resolve<TelemetryClient>(), container.Resolve<IValidateRequest>(),
                container.Resolve<IMapper>(), CdkAutolineUrl));

            return container;
        }

        private static void RegisterTelemetryClient(Unity.UnityContainer container)
        {
            var telemetryClient = new TelemetryClient { InstrumentationKey = AppInsightsKey };

            container.RegisterInstance(telemetryClient, new ContainerControlledLifetimeManager());
        }

        private static void RegisterAutoMapper(Unity.UnityContainer container)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GetAppointmentSlotsRequest, CDKGetAppointmentSlotsRequest>();
            });
            var mapper = config.CreateMapper();
            container.RegisterInstance(mapper);
        }
    }
}