using Service.Contract.DBModel;
using Service.Contract.RequestModel;
using System;

namespace Service.Tests
{
    public class TestResources
    {
        public static Appointment Appointment => new Appointment()
        {
            Id = 1,
            DealerId = 1,
            CustomerFirstName = "Manu",
            CustomerSurName = "Chawla",
            EmailAddress = "manu.chawla@daffodilsw.com",
            Mobile = "1234567890",
            VehicleRegistrationNumber = "FA129",
            JobDate = DateTime.Now,
            TransportMethod = "App01",
            DropOffTime = "07:45-08:00"
        };

        public static AppointmentJob AppointmentJob => new AppointmentJob()
        {
            Id = 1,
            AppointmentId = 1,
            JobId = 71
        };

        public static CreateAppointmentRequest CreateAppointmentRequest => new CreateAppointmentRequest()
        {
            DealerId = 1,
            CustomerFirstName = "Manu",
            CustomerSurName = "Chawla",
            EmailAddress = "manu.chawla@daffodilsw.com",
            Mobile = "1234567890",
            VehicleRegistrationNumber = "FA129",
            JobDate = DateTime.Now.ToString(),
            TransportMethod = "App01",
            DropOffTime = "07:45-08:00",
            JobId = 1
        };

        public static CreateAppointmentRequest CreateAppointmentRequest_InvalidDealerId => new CreateAppointmentRequest()
        {
            DealerId = -1
        };
    }
}
