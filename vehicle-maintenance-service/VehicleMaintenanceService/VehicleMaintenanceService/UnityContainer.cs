using System.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Service.Contract;
using Service.Contract.Models;
using Service.Implemention;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace VehicleMaintenanceService
{
    public class UnityContainer
    {
        private static readonly string CdkAutolineApiKey = ConfigurationManager.AppSettings["CDKAutolineServiceKey"];
        private static readonly string CdkAutolineApiUrl = ConfigurationManager.AppSettings["CDKAutolineServiceUrl"];

        private static readonly string DealerConfigurationApiKey = ConfigurationManager.AppSettings["DealerConfigurationServiceKey"];
        private static readonly string DealerConfigurationApiUrl = ConfigurationManager.AppSettings["DealerConfigurationServiceUrl"];

        private const string CdkAutolineApiClient = "CDKAutolineApiClient";
        private const string DealerConfigurationApiClient = "DealerConfigurationAPIClient";

        private static readonly string Key = TelemetryConfiguration.Active.InstrumentationKey =
            ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];

        private static IUnityContainer _unityContainer;
        public static IUnityContainer Instance => _unityContainer ?? (_unityContainer = InitialiseUnityContainer());
        private static IUnityContainer InitialiseUnityContainer()
        {
            var container = new Unity.UnityContainer();

            RegisterServiceClients(container);
            RegisterTelemetryClient(container);
            container.RegisterType<IRecommendedService, RecommendedService>();
            container.RegisterType<IDealerService, DealerService>(
                new InjectionConstructor(container.Resolve<IRestfulClient>(DealerConfigurationApiClient)));
            container.RegisterType<ICDKAutolineService, CDKAutolineService>(
                new InjectionConstructor(container.Resolve<IRestfulClient>(CdkAutolineApiClient), container.Resolve<TelemetryClient>()));

            return container;
        }
        private static void RegisterServiceClients(Unity.UnityContainer container)
        {
            var cdkClientConfiguration =
                new ClientConfiguration { ServiceUrl = CdkAutolineApiUrl, AccessKey = CdkAutolineApiKey };

            container.RegisterType<IRestfulClient, RestfulClient>(CdkAutolineApiClient, new InjectionConstructor(cdkClientConfiguration));

            var dealerClientConfiguration =
                new ClientConfiguration { ServiceUrl = DealerConfigurationApiUrl, AccessKey = DealerConfigurationApiKey };
            container.RegisterType<IRestfulClient, RestfulClient>(DealerConfigurationApiClient, new InjectionConstructor(dealerClientConfiguration));
        }
        private static void RegisterTelemetryClient(Unity.UnityContainer container)
        {
            var telemetryClient = new TelemetryClient { InstrumentationKey = Key };

            container.RegisterInstance(telemetryClient, new ContainerControlledLifetimeManager());
        }
    }
}
