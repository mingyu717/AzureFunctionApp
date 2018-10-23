using Moq;
using NUnit.Framework;
using Service.Contract.DBModel;
using Service.Implementation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Tests
{
    public class AppointmentDALTest
    {
        private AppointmentDAL _underTest;
        private Mock<AppointmentContext> _appointmentContextMock;

        private readonly IQueryable<Appointment> _appointment = new List<Appointment>
        {
            TestResources.Appointment
        }.AsQueryable();

        private readonly IQueryable<AppointmentJob> _appointmentJob = new List<AppointmentJob>
        {
            TestResources.AppointmentJob
        }.AsQueryable();

        [SetUp]
        public void SetUp()
        {
            var appointmentDbSetMock = GetMockDbSet(_appointment);

            var appointmentJobsDbSetMock = GetMockDbSet(_appointmentJob);
            appointmentJobsDbSetMock.Setup(m => m.Include("Appointments")).Returns(appointmentJobsDbSetMock.Object);

            _appointmentContextMock = new Mock<AppointmentContext>();
            _appointmentContextMock.Setup(mock => mock.Appointments).Returns(appointmentDbSetMock.Object);
            _appointmentContextMock.Setup(mock => mock.AppointmentJobs).Returns(appointmentJobsDbSetMock.Object);
            _appointmentContextMock.Setup(mock => mock.SaveChangesAsync()).Returns(Task.FromResult(0));
            _underTest = new AppointmentDAL(_appointmentContextMock.Object);
        }

        internal Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> entities) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());
            return mockSet;
        }

        [Test]
        public async Task SaveAppointment_Test()
        {
            var result = await _underTest.SaveAppointment(new Appointment()
            {
                DealerId = 1,
                CustomerFirstName = "Manu",
                CustomerSurName = "Chawla",
                EmailAddress = "manu.chawla@daffodilsw.com",
                Mobile = "1234567890",
                VehicleRegistrationNumber = "FA129",
                JobDate = DateTime.Now,
                TransportMethod = "App01",
                DropOffTime = "07:45-08:00"
            });
            _appointmentContextMock.Verify(mock => mock.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task SaveAppointmentJob_Test()
        {
            await _underTest.SaveAppointmentJob(new AppointmentJob()
            {
                AppointmentId = 1,
                JobId = 71
            });
            _appointmentContextMock.Verify(mock => mock.SaveChangesAsync(), Times.Once);
        }
    }
}