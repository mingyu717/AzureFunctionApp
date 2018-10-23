using Service.Contract;
using Service.Contract.Response;
using System;
using System.Collections.Generic;

namespace Service.Tests
{
    public static class TestResources
    {
        public static CustomerVehicle CustomerVehicle => new CustomerVehicle()
        {
            CustomerId = 1,
            VehicleNo = 12345678,
            RegistrationNo = "HBC123",
            VinNumber = "HBC234",
            MakeCode = "B",
            ModelCode = "BMW112",
            ModelYear = "2017",
            ModelDescription = "BMW 112",
            LastServiceDate = "10/03/2017",
            NextServiceDate = "8/06/2018",
            LastKnownMileage = 16248,
            NextServiceMileage = 89388,
            VariantCode = "VariantCode",
            InvitationTime = DateTime.Now
        };

        public static Customer Customer => new Customer()
        {
            Id = 5,
            CustomerNo = 123456,
            CustomerEmail = "test@mail.com",
            RooftopId = "ABC123",
            CommunityId = "abcd1234",
            FirstName = "First Name",
            Surname = "Last Name",
            PhoneNumber = "021123456"
        };

        public static SaveCustomerVehicleRequest SaveCustomerVehicleRequest => new SaveCustomerVehicleRequest
        {
            CustomerNo = 123456,
            CustomerEmail = "test@mail.com",
            FirstName = "First Name",
            Surname = "Last Name",
            PhoneNumber = "021123456",
            CommunityId = "abcd1234",
            RooftopId = "ABC123",
            VehicleNo = 12345678,
            RegistrationNo = "HBC123",
            VinNumber = "HBC234",
            MakeCode = "B",
            ModelCode = "BMW112",
            ModelYear = "2017",
            ModelDescription = "BMW 112",
            LastServiceDate = "10/03/2017",
            NextServiceDate = "8/06/2018",
            LastKnownMileage = 16248,
            NextServiceMileage = 89388,
            VariantCode = "VariantCode",
        };

        public static SaveCustomerVehicleRequest SaveCustomerVehicleRequestWithoutPhoneNumber => new SaveCustomerVehicleRequest
        {
            CustomerNo = 123456,
            CustomerEmail = "test@mail.com",
            FirstName = "First Name",
            Surname = "Last Name",
            CommunityId = "abcd1234",
            RooftopId = "ABC123",
            VehicleNo = 12345678,
            RegistrationNo = "HBC123",
            VinNumber = "HBC234",
            MakeCode = "B",
            ModelCode = "BMW112",
            ModelYear = "2017",
            ModelDescription = "BMW 112",
            LastServiceDate = "10/03/2017",
            NextServiceDate = "8/06/2018",
            LastKnownMileage = 16248,
            NextServiceMileage = 89388,
            VariantCode = "VariantCode",
        };


        public static Invitation Invitation => new Invitation()
        {
            CustomerId = 1,
            ContactDetail = "021123456",
            DealerId = 1,
            Method = 0,
            Timestamp = DateTime.Now
        };

        public static DealerConfigurationResponse DealerConfigurationResponse => new DealerConfigurationResponse()
        {
            DealerId = 1,
            DealerName = "abc",
            RooftopId = "EBBVW11Dev",
            CommunityId = "EBBETDev",
            Address = "xyz",
            CommunicationMethod = 0,
            PhoneNumber = "1234567890",
            Latitude = 28.459497,
            Longitude = 77.026638,
            AppThemeName = "SampleTheme1",
            HasDropOff = true,
            HasCourtesyCar = true,
            EmailAddress = "testDealer@mail.com"
        };

        public static DealerInvitationContentResponse DealerInvitationResponse => new DealerInvitationContentResponse()
        {
            DealerId = 1,
            EmailContent = "Test email content",
            EmailSubject = "Test email subject",
            SmsContent = "Test sms content"
        };

        public static DismissVehicleOwnershipRequest DismissVehicleOwnershipRequest =>
            new DismissVehicleOwnershipRequest()
            {
                CustomerNo = 12,
                DealerId = 12,
                VehicleNo = 12
            };

        public static UpdateCustomerContactRequest UpdateCustomerContactRequest => new UpdateCustomerContactRequest()
        {
            DealerId = 1,
            CustomerNo = 123,
            FirstName = "xyz",
            SurName = "abc",
            CustomerEmail = "test@email.com",
            PhoneNumber = "1234567890",
            AdditionalComments = "Test additional Comments"
        };


        public static CreateServiceBookingRequest CreateServiceBookingRequestVersion_InvalidDealerId => new CreateServiceBookingRequest()
        {
            DealerId = 0
        };

        public static CreateServiceBookingRequest CreateServiceBookingRequestVersion_InvalidCustomerNo => new CreateServiceBookingRequest()
        {
            CustomerNo = -1
        };

        public static CreateServiceBookingResponse CreateServiceBookingResponse => new CreateServiceBookingResponse()
        {
            AppointmentId = 120,
            Result = new ErrorResponse()
            {
                ErrorCode = "0"
            },
            WipNo = 124
        };

        public static CreateServiceBookingRequest CreateServiceBookingRequest => new CreateServiceBookingRequest()
        {
            CustomerNo = 123,
            DealerId = 12,
            VehicleNo = 134,
            ActualMileage = 1000,
            AdvisorDropOffTimeCode = "7:30 - 7:45",
            AdvisorID = "JSM",
            JobDate = new DateTime(2018, 10, 20),
            Jobs = new List<JobData>()
            {
                new JobData() {JobCode = "71"}
            },
            TransportMethod = "testTransportMethod"
        };

        public static CDKCreateServiceBookingRequest CDKCreateServiceBookingRequest => new CDKCreateServiceBookingRequest()
        {
            EmailAddress = "test@email.com",
            RooftopId = "testRoofTopId",
            CommunityId = "testCommunityId",
            CustomerNo = 123,
            FirstName = "test",
            VehicleNo = 134,
            ActualMileage = 1000,
            AdvisorDropOffTimeCode = "7:30 - 7:45",
            AdvisorId = "JSM",
            JobDate = new DateTime(2018, 10, 20),
            Jobs = new List<JobData>()
            {
                new JobData() {JobCode = "71"}
            },
            MobileTelNo = "1234567890",
            SurName = "test",
            TransportMethod = "testTransportMethod",
            VehMakeCode = "testMakeCode",
            VehModelCode = "testModelCode",
            VehVariantCode = "testVariantCode",
            VehicleRegistrationNo = "testRegistrationNo"
        };

        public static ServiceBookings ServiceBookings => new ServiceBookings()
        {
            CustomerNo = 123,
            BookingReference = "124",
            DealerId = 12,
            VehicleNo = 134,
            CreateTime = new DateTime(2018, 10, 20)
        };
    }
}