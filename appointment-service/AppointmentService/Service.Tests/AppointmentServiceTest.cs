using AutoMapper;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.DBModel;
using Service.Contract.RequestModel;
using Service.Implementation;
using System;
using System.Threading.Tasks;

namespace Service.Tests
{
    [TestFixture]
    public class AppointmentServiceTest
    {
        private Mock<IAppointmentDAL> _appointmentDAL;
        private IMapper _mapper;

        private AppointmentService _underTest;

        [SetUp]
        public void Setup()
        {
            _appointmentDAL = new Mock<IAppointmentDAL>();

            _appointmentDAL.Setup(mock => mock.SaveAppointment(It.IsAny<Appointment>())).Returns(Task.FromResult(12));
            _appointmentDAL.Setup(mock => mock.SaveAppointmentJob(It.IsAny<AppointmentJob>())).Returns(Task.FromResult(12));
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateAppointmentRequest, Appointment>();
                cfg.CreateMap<CreateAppointmentRequest, AppointmentJob>();
            });
            _mapper = config.CreateMapper();
            _underTest = new AppointmentService(_appointmentDAL.Object, _mapper);
        }

        [Test]
        public void AppointmentService_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new AppointmentService(null, _mapper));
            Assert.Throws<ArgumentNullException>(() => new AppointmentService(_appointmentDAL.Object, null));
        }


        [Test]
        public void CreateAppointment_NullRequest_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.CreateAppointment(null));
        }

        [Test]
        public async Task CreateAppointment_Test()
        {
            await _underTest.CreateAppointment(TestResources.CreateAppointmentRequest);
        }
    }
}
