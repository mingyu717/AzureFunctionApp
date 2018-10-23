using AutoMapper;
using Service.Contract;
using Service.Contract.DBModel;
using Service.Contract.RequestModel;
using Service.Implementation;
using System.Configuration;
using Unity;
using Unity.Injection;

namespace AppointmentService
{
    public class UnityContainer
    {
        private static IUnityContainer _unityContainer;

        private static readonly string DatabaseConnectionString =
            ConfigurationManager.ConnectionStrings["sql_connection"].ConnectionString;

        public static IUnityContainer Instanace => _unityContainer ?? (_unityContainer = InitializeUnityContainer());

        private static IUnityContainer InitializeUnityContainer()
        {
            var container = new Unity.UnityContainer();
            RegisterAutoMapper(container);
            container.RegisterType<IValidateRequest, ValidateRequest>();
            container.RegisterType<IAppointmentDAL, AppointmentDAL>(
                new InjectionConstructor(new AppointmentContext(DatabaseConnectionString)));

            container.RegisterType<IAppointmentService,  Service.Implementation.AppointmentService>();

            return container;

        }

        private static void RegisterAutoMapper(Unity.UnityContainer container)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateAppointmentRequest, Appointment>();
                cfg.CreateMap<CreateAppointmentRequest, AppointmentJob>();
            });

            var mapper = config.CreateMapper();
            container.RegisterInstance(mapper);
        }
    }
}
