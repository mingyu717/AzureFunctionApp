using System;
using System.Configuration;
using AutoMapper;
using Processor.Contract;
using Processor.Implementation;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace CustomerDetailsProcessor
{
    public static class UnityContainer
    {
        private static IUnityContainer _unityContainer;
        public static IUnityContainer Instance => _unityContainer ?? (_unityContainer = InitialiseUnityContainer());

        private static readonly string CustomerVehicleServiceKey = ConfigurationManager.AppSettings["customerVehicleAppKey"];
        private static readonly string CustomerVehicleServiceUrl = ConfigurationManager.AppSettings["customerVehicleAppUrl"];
        private static readonly string DealerConfigurationServiceUrl = ConfigurationManager.AppSettings["dealerConfigurationServiceUrl"];
        private static readonly string DealerConfigurationServiceKey = ConfigurationManager.AppSettings["dealerConfigurationServiceKey"];

        private static readonly int ExchangeVersion = Convert.ToInt32(ConfigurationManager.AppSettings["exchangeVersion"]);
        private static readonly string ExchangeUserName = ConfigurationManager.AppSettings["exchangeUserName"];
        private static readonly string ExchangePassword = ConfigurationManager.AppSettings["exchangePassword"];
        private static readonly string ExchangeUrl = ConfigurationManager.AppSettings["exchangeUrl"];
        private static readonly string AzureFileConnectionString = ConfigurationManager.AppSettings["azureFileConnectionString"];
        private static readonly string AzureFileShareName = ConfigurationManager.AppSettings["azureFileShareName"];
        private static readonly string AzureFileProcessedFolderName = ConfigurationManager.AppSettings["azureFileProcessedFolderName"];
        private static readonly string AppendDateFormatInFileName = ConfigurationManager.AppSettings["appendDateFormatInFileName"];

        private static IUnityContainer InitialiseUnityContainer()
        {
            var container = new Unity.UnityContainer();

            RegisterAutoMapper(container);
            RegisterServiceClients(container);

            container.RegisterType<ICustomerVehicleClient, CustomerVehicleClient>(
                new InjectionConstructor(container.Resolve<IRestfulClient>(Constants.CustomerVehicleAPI),
                container.Resolve<IMapper>()));

            container.RegisterType<IDealerConfigurationClient, DealerConfigurationClient>(
                new InjectionConstructor(container.Resolve<IRestfulClient>(Constants.DealerConfigurationAPI)));

            container.RegisterType<IEmailProcessor, EmailProcessor>();

            container.RegisterType<IExchangeServices, ExchangeServices>("ExchangeServices",
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    ExchangeVersion,
                    ExchangeUserName,
                    ExchangePassword,
                    ExchangeUrl
                ));

            container.RegisterType<IMailService, MailService>(
                new InjectionConstructor(container.Resolve<IExchangeServices>("ExchangeServices")));

            container.RegisterType<ICsvProcessor, CsvProcess>(new InjectionConstructor(GetCsvColumnNames()));

            container.RegisterType<IAzureServices, AzureServices>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                     AzureFileConnectionString,
                     AzureFileShareName,
                     AzureFileProcessedFolderName,
                     AppendDateFormatInFileName
                ));

            container.RegisterType<IAzureFileProcessor, AzureFileProcessor>(
                new InjectionConstructor(container.Resolve<IAzureServices>()));

            container.RegisterType<IFileContentService, FileContentService>(
                new InjectionConstructor(container.Resolve<IEmailProcessor>(), container.Resolve<IAzureFileProcessor>()));

            return container;
        }

        private static void RegisterServiceClients(Unity.UnityContainer container)
        {
            var customerVehicleClientConfiguration = new ClientConfiguration
            {
                ServiceUrl = CustomerVehicleServiceUrl,
                AccessKey = CustomerVehicleServiceKey
            };
            container.RegisterType<IRestfulClient, RestfulClient>(Constants.CustomerVehicleAPI,
                new InjectionConstructor(customerVehicleClientConfiguration));

            var dealerClientConfiguration = new ClientConfiguration
            {
                ServiceUrl = DealerConfigurationServiceUrl,
                AccessKey = DealerConfigurationServiceKey
            };
            container.RegisterType<IRestfulClient, RestfulClient>(Constants.DealerConfigurationAPI,
                new InjectionConstructor(dealerClientConfiguration));
        }

        private static void RegisterAutoMapper(Unity.UnityContainer container)
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<CustomerVehicle, SaveCustomerVehicleRequest>(); });

            var mapper = config.CreateMapper();

            container.RegisterInstance(mapper);
        }

        private static CsvColumnNames GetCsvColumnNames()
        {
            return new CsvColumnNames
            {
                CustomerNo = ConfigurationManager.AppSettings["customerNoColumn"],
                CustomerEmail = ConfigurationManager.AppSettings["customerEmailColumn"],
                FirstName = ConfigurationManager.AppSettings["firstNameColumn"],
                Surname = ConfigurationManager.AppSettings["surNameColumn"],
                PhoneNumber = ConfigurationManager.AppSettings["phoneNumberColumn"],
                RooftopId = ConfigurationManager.AppSettings["roofTopIdColumn"],
                CommunityId = ConfigurationManager.AppSettings["communityIdColumn"],
                VehicleNo = ConfigurationManager.AppSettings["vehicleNoColumn"],
                RegistrationNo = ConfigurationManager.AppSettings["registrationNoColumn"],
                VinNumber = ConfigurationManager.AppSettings["vinNumberColumn"],
                MakeCode = ConfigurationManager.AppSettings["makeCodeColumn"],
                ModelCode = ConfigurationManager.AppSettings["modelCodeColumn"],
                ModelYear = ConfigurationManager.AppSettings["modelYearColumn"],
                ModelDescription = ConfigurationManager.AppSettings["modelDescriptionColumn"],
                VariantCode = ConfigurationManager.AppSettings["variantCodeColumn"],
                NextServiceMileage = ConfigurationManager.AppSettings["nextServiceMileageColumn"],
            };
        }
    }
}