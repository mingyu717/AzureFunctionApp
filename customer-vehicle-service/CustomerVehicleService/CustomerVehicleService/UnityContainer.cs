using System;
using System.Configuration;
using AutoMapper;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using SendGrid;
using Service.Contract;
using Service.Contract.Response;
using Service.Implementation;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace CustomerVehicleService
{
    public class UnityContainer
    {
        private static IUnityContainer _unityContainer;

        private static readonly string ServiceBookingAppUrl = ConfigurationManager.AppSettings["serviceBookingAppUrl"];
        private static readonly string PilvoAuthId = ConfigurationManager.AppSettings["pilvoAuthId"];
        private static readonly string PilvoAuthToken = ConfigurationManager.AppSettings["pilvoAuthToken"];

        private static readonly string DatabaseConnectionString =
            ConfigurationManager.ConnectionStrings["sql_connection"].ConnectionString;

        private static readonly string InvitationFromPhoneNumber =
            ConfigurationManager.AppSettings["invitationFromPhoneNumber"];

        private static readonly string InvitationExpiredDays =
            ConfigurationManager.AppSettings["invitationExpiredDays"];

        private static readonly string ServiceBookingExpiredDays =
            ConfigurationManager.AppSettings["serviceBookingExpiredDays"];

        private static readonly string CdkAutolineApiKey = ConfigurationManager.AppSettings["CDKAutolineServiceKey"];
        private static readonly string CdkAutolineApiUrl = ConfigurationManager.AppSettings["CDKAutolineServiceUrl"];

        private static readonly string DealerConfigurationApiKey = ConfigurationManager.AppSettings["DealerConfigurationServiceKey"];
        private static readonly string DealerConfigurationApiUrl = ConfigurationManager.AppSettings["DealerConfigurationServiceUrl"];

        private static readonly string Key = TelemetryConfiguration.Active.InstrumentationKey =
            ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];

        private static readonly string SendGridAPIKey = ConfigurationManager.AppSettings["sendGridApiKey"];

        private static readonly string ServiceBookingUrlPlaceHolder = ConfigurationManager.AppSettings["serviceBookingUrlPlaceHolder"];
        private static readonly string RegistrationNoPlaceHolder = "{REGISTRATION-NUMBER}";
        private static readonly string ServiceBookingEmail = ConfigurationManager.AppSettings["serviceBookingEmail"];

        public static IUnityContainer Instance => _unityContainer ?? (_unityContainer = InitialiseUnityContainer());

        private static IUnityContainer InitialiseUnityContainer()
        {
            var container = new Unity.UnityContainer();

            RegisterTelemetryClient(container);
            RegisterServiceClients(container);
            RegisterAutoMapper(container);
            RegisterSendGridClient(container);

            container.RegisterType<ICustomerVehicleDAL, CustomerVehicleDAL>(
                new InjectionConstructor(DatabaseConnectionString));

            container.RegisterType<ICustomerServiceBooking, CustomerServiceBooking>(
                new InjectionConstructor(container.Resolve<IRestfulClient>(Constants.CDKAutolineAPI),
                    container.Resolve<TelemetryClient>()));

            container.RegisterType<ISMSGatewayClient, PlivoGatewayClient>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(PilvoAuthId, PilvoAuthToken));

            container.RegisterType<IEmailGatewayClient, EmailGatewayClient>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(container.Resolve<ISendGridClient>()));

            container.RegisterType<IDealerConfigurationService, DealerConfigurationService>(
                new InjectionConstructor(container.Resolve<IRestfulClient>(Constants.DealerConfigurationAPI)));

            container.RegisterType<ICustomerInvitationService, CustomerInvitationService>(
                new InjectionConstructor(ServiceBookingAppUrl, Convert.ToInt32(InvitationExpiredDays),
                    InvitationFromPhoneNumber, container.Resolve<ISMSGatewayClient>(),
                    container.Resolve<IEmailGatewayClient>(), container.Resolve<TelemetryClient>(),
                    container.Resolve<IDealerConfigurationService>(), ServiceBookingUrlPlaceHolder,
                    RegistrationNoPlaceHolder
                ));

            container.RegisterType<IEmailService, EmailService>(
                new InjectionConstructor(container.Resolve<IEmailGatewayClient>(),
                    ServiceBookingEmail));

            container.RegisterType<ICustomerRegistrationService, CustomerRegistrationService>(
                new InjectionConstructor(container.Resolve<IMapper>(), container.Resolve<IRestfulClient>(Constants.CDKAutolineAPI),
                    container.Resolve<TelemetryClient>()));

            container.RegisterType<IValidateRequest, ValidateRequest>();

            container.RegisterType<ICustomerVehicleService, Service.Implementation.CustomerVehicleService>(
                new InjectionConstructor(container.Resolve<ICustomerVehicleDAL>(), container.Resolve<IMapper>(),
                    container.Resolve<ICustomerRegistrationService>(), container.Resolve<ICustomerInvitationService>(),
                    container.Resolve<IEmailService>(), container.Resolve<ICustomerServiceBooking>(),
                    Convert.ToInt32(ServiceBookingExpiredDays)));

            return container;
        }

        private static void RegisterTelemetryClient(Unity.UnityContainer container)
        {
            var telemetryClient = new TelemetryClient {InstrumentationKey = Key};

            container.RegisterInstance(telemetryClient, new ContainerControlledLifetimeManager());
        }

        private static void RegisterSendGridClient(Unity.UnityContainer container)
        {
            var sendGridClient = new SendGridClient(SendGridAPIKey);
            container.RegisterInstance<ISendGridClient>(sendGridClient, new ContainerControlledLifetimeManager());
        }

        private static void RegisterServiceClients(Unity.UnityContainer container)
        {
            var cdkClientConfiguration =
                new ClientConfiguration {ServiceUrl = CdkAutolineApiUrl, AccessKey = CdkAutolineApiKey};

            container.RegisterType<IRestfulClient, RestfulClient>(Constants.CDKAutolineAPI, new InjectionConstructor(cdkClientConfiguration));

            var dealerClientConfiguration =
                new ClientConfiguration {ServiceUrl = DealerConfigurationApiUrl, AccessKey = DealerConfigurationApiKey};
            container.RegisterType<IRestfulClient, RestfulClient>(Constants.DealerConfigurationAPI, new InjectionConstructor(dealerClientConfiguration));
        }

        private static void RegisterAutoMapper(Unity.UnityContainer container)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SaveCustomerVehicleRequest, Customer>();
                cfg.CreateMap<SaveCustomerVehicleRequest, CustomerVehicle>();
                cfg.CreateMap<Customer, RegisterCustomerRequest>()
                    .ForMember(dest => dest.RoofTopId, opt => opt.ResolveUsing(src => src.RooftopId))
                    .ForMember(dest => dest.EmailAddress, opt => opt.ResolveUsing(src => src.CustomerEmail));
                cfg.CreateMap<CustomerVehicle, RegisterCustomerRequest>()
                    .ForMember(dest => dest.RegistrationNumber, opt => opt.ResolveUsing(src => src.RegistrationNo))
                    .ForMember(dest => dest.VehicleId, opt => opt.ResolveUsing(src => src.VehicleNo));
                cfg.CreateMap<Customer, GetCustomerVehicleResponse>();
                cfg.CreateMap<CustomerVehicle, GetCustomerVehicleResponse>();
                cfg.CreateMap<Customer, VerifyCustomerRequest>()
                    .ForMember(dest => dest.RoofTopId, opt => opt.ResolveUsing(src => src.RooftopId));
                cfg.CreateMap<SaveCustomerVehicleRequest, Invitation>()
                    .ForMember(dest => dest.ContactDetail, opt => opt.ResolveUsing(src => src.PhoneNumber));
                cfg.CreateMap<CreateServiceBookingRequest, ServiceBookings>();
                cfg.CreateMap<CreateServiceBookingResponse, ServiceBookings>()
                    .ForMember(dest => dest.BookingReference, opt => opt.ResolveUsing(src => src.WipNo.ToString()));
                cfg.CreateMap<CreateServiceBookingRequest, CDKCreateServiceBookingRequest>();
                cfg.CreateMap<Customer, CDKCreateServiceBookingRequest>()
                    .ForMember(dest => dest.MobileTelNo, opt => opt.ResolveUsing(src => src.PhoneNumber))
                    .ForMember(dest => dest.EmailAddress, opt => opt.ResolveUsing(src => src.CustomerEmail));
                cfg.CreateMap<CustomerVehicle, CDKCreateServiceBookingRequest>()
                    .ForMember(dest => dest.VehicleRegistrationNo, opt => opt.ResolveUsing(src => src.RegistrationNo))
                    .ForMember(dest => dest.VehMakeCode, opt => opt.ResolveUsing(src => src.MakeCode))
                    .ForMember(dest => dest.VehModelCode, opt => opt.ResolveUsing(src => src.ModelCode))
                    .ForMember(dest => dest.VehVariantCode, opt => opt.ResolveUsing(src => src.VariantCode));
            });

            var mapper = config.CreateMapper();

            container.RegisterInstance(mapper);
        }
    }
}