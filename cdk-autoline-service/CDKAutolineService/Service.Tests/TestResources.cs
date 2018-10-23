using Service.Contract.DbModels;
using Service.Contract.Models;
using Service.Contract.Response;
using System;
using System.Collections.Generic;

namespace Service.Tests
{
    public class TestResources
    {
        public static CustomerVerifyRequest CustomerVerifyRequest => new CustomerVerifyRequest()
        {
            CommunityId = "EBBETDEV",
            CustomerNo = 10,
            RoofTopId = "EBBVW11DEV"
        };

        public static string CdkAutolineUrl = "http://xyz/api";
        
        public static CustomerVehicleRegisterRequest CustomerVehicleRegisterModel => new CustomerVehicleRegisterRequest
        {
            AppToken = "e694dc32-e8d9-4378-87b6-cd96e320b453",
            CommunityId = "EBBETDEV",
            CustomerNo = 10,
            EmailAddress = "test@email.com",
            FirstName = "test",
            RoofTopId = "testRoofTop",
            Surname = "test"
        };

        public static CdkCustomer CdkCustomer => new CdkCustomer
        {
            Id = 10,
            CommunityId = "EBBETDEV",
            CustomerNo = 10,
            CustomerLoginId = "test@email.com",
            Password = "test1234",
            Token = new Guid("e0779055-5b94-4291-8acf-4874210469b8")
        };

        public static AppToken AppToken => new AppToken
        {
            CommunityId = "EBBVW11DEV",
            Id = 1,
            Token = new Guid("e0779055-5b94-4291-8acf-4874210469b8")
        };

        public static TokenResponse TokenResponse => new TokenResponse()
        {
            Token = "e694dc32-e8d9-4378-87b6-cd96e320b453",
            Result = new ErrorResponse()
            {
                ErrorCode = "0"
            }
        };

        public static string TestAppToken => "e694dc32-e8d9-4378-87b6-cd96e320b453";

        public static CustomerResponse CustomerResponse => new CustomerResponse()
        {
            ErrorCode = "0"
        };

        public static GetRecommendedServicesRequest GetRecommendedServicesRequest => new GetRecommendedServicesRequest
        {
            CommunityId = "EBBETDEV",
            RooftopId = "testRoofTop",
            MakeCode = "testMakeCode",
            ModelCode = "testModel",
            EstOdometer = "10000",
            EstVehicleAgeMonths = "36"
        };

        public static CDKRecommendedServiceResponse CdkRecommendedServiceResponse => new CDKRecommendedServiceResponse
        {
            Results = new GetRecommendedServicesResponse
            {
                PriceListData = new List<VehicleService>
                {
                    new VehicleService
                    {
                        JobCode = "123",
                        JobDescription = "Service"
                    }
                }
            }
        };

        public static CreateServiceBookingRequest CreateServiceBookingRequest => new CreateServiceBookingRequest()
        {
            CommunityId = "EBBETDEV",
            CustomerNo = 10,
            EmailAddress = "test@email.com",
            FirstName = "test",
            SurName = "test",
            VehMakeCode = "testMakeCode",
            VehModelCode = "testModel",
            VehicleRegistrationNo = "testRegistration",
            RooftopId = "testRoofTop",
            VehVariantCode = "testVariantCode",
            JobDate = DateTime.Now,
            Jobs = new List<JobData>()
            {
                new JobData()
                {
                    JobCode = "71"
                },
                new JobData()
                {
                    JobCode = "72"
                }
            },
            ActualMileage = 1000,
            MobileTelNo = "1234567890",
            AdvisorId = "JSM",
            AdvisorDropOffTimeCode = "07:30 - 07:45",
            TransportMethod = "testTransportMethod"

        };

        public static CreateServiceBookingResponse CreateServiceBookingResponse => new CreateServiceBookingResponse()
        {
            AppointmentId = 1,
            WipNo = 1,
            Result = new ErrorResponse()
            {
                ErrorCode = "0"
            }
        };

        public static DealerCDKConfiguration DealerCDKConfiguration => new DealerCDKConfiguration()
        {
            Id = 1,
            CommunityId = "EBBETDEV",
            RoofTopId = "testRoofTop",
            PartnerId = "EBBETDEVPK",
            PartnerKey = "testPartnerKey",
            PartnerVersion = "1"
        };

        public static GetServiceAdvisorsRequest GetServiceAdvisorsRequest => new GetServiceAdvisorsRequest()
        {
            CommunityId = "EBBETDEV",
            CustomerNo = 10,
            RooftopId = "testroofTop",
            AppointmentDate = DateTime.Now,
            CustomerId = "testcustomerId",
            DropOffTime = "testDropTime",
            TransportMethod = "testTransportMethod"

        };

        public static GetServiceAdvisorsResponse GetServiceAdvisorsResponse => new GetServiceAdvisorsResponse()
        {
            PreferredSA = "testPrefferedSA",
            PreferredSAAvailToday = true,
            PreferredSAName = "testPreferedSAName",
            Result = new ErrorResponse
            {
                ErrorCode = "0"
            },
            Results = new Results
            {
                AdvisorData = new List<AdvisorData>()
                {
                    new AdvisorData
                    {
                        AdvisorID = "1",
                        AdvisorName = "testAdvisorName"
                    },
                    new AdvisorData
                    {
                        AdvisorID = "2",
                        AdvisorName = "testAdvisorName1"
                    }
                }
            }

        };

        public static GetAppointmentSlotsRequest GetAppointmentSlotsRequest => new GetAppointmentSlotsRequest()
        {
            CommunityId = "EBBETDEV",
            CustomerNo = 10,
            RooftopId = "testroofTop",
            InitialDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30),
            JobCode = new List<string> { "70" }
        };

        public static GetAppointmentSlotsResponse GetAppointmentSlotsResponse => new GetAppointmentSlotsResponse()
        {
            NonWorkingDates = new List<string> { "2018-10-20", "2018-10-21", "2018-10-22" },
            WorksDiary = new List<string> { "2018-10-18", "2018-10-19", "2018-10-23", "2018-10-24" },
            WorksDiaryDetails = new List<WorksDiaryDetail>
            {
                new WorksDiaryDetail { Date = "2018-10-18", CapacityFree = 96.29, WorkshopCapacity = 96.29},
                new WorksDiaryDetail { Date = "2018-10-18", CapacityFree = 96.29, WorkshopCapacity = 96.29}
            },
            Option = new List<AppointmentOption>
            {
                new AppointmentOption
                {
                    OptionID = "APP01",
                    OptionDisplayName = "Appointment booking",
                    OptionDescription = "Service Advisor booking time for a quick check around your vehicle to discuss what work is required on the vehicle",
                    OptionPrice = 0,
                    OptionAdvisor = "Manual",
                    InitialAppFieldName = "Arrival",
                    InitialAppTimeRequired = true,
                    SecondAppFieldName = "",
                    SecondAppTimeRequired = false,
                    RequestAddress = false,
                    RequestPostCode = false,
                    FirstAddressLabel = "",
                    RequestSecondAddress = false,
                    RequestSecondPostCode = false,
                    SecondAddressLabel = "",
                    AdvisorsPhotos = false,
                    Slots = new List<AppointmentSlot> {
                            new AppointmentSlot{
                                Date = "2018-10-18",
                                Slots = new List<string> {
                                    "07:30-07:45",
                                    "07:45-08:00"
                                }
                            }
                    },
                    Resource = new AppointmentResource()
                    {
                        Id = "1",
                        Type = 1
                    }
                }
            },
            Result = new ErrorResponse
            {
                ErrorCode = "0"
            }
        };

    }
}