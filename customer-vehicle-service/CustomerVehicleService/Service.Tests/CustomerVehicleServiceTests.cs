using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Response;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture]
    public class CustomerVehicleServiceTests
    {
        private CustomerVehicleService _underTest;
        private Mock<ICustomerVehicleDAL> _customerVehicleDalMock;
        private Mock<ICustomerRegistrationService> _customerRegistrationServiceMock;
        private Mock<ICustomerInvitationService> _customerInvitationServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ICustomerServiceBooking> _customerServiceBookingMock;
        private IMapper _mapper;
        private readonly int _serviceBookingExpiredDays;

        [SetUp]
        public void SetUp()
        {
            _customerVehicleDalMock = new Mock<ICustomerVehicleDAL>();
            _customerRegistrationServiceMock = new Mock<ICustomerRegistrationService>();
            _customerInvitationServiceMock = new Mock<ICustomerInvitationService>();
            _emailServiceMock = new Mock<IEmailService>();
            _customerServiceBookingMock = new Mock<ICustomerServiceBooking>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SaveCustomerVehicleRequest, Customer>();
                cfg.CreateMap<SaveCustomerVehicleRequest, CustomerVehicle>();
                cfg.CreateMap<Customer, GetCustomerVehicleResponse>();
                cfg.CreateMap<CustomerVehicle, GetCustomerVehicleResponse>()
                    .ForMember(dest => dest.VehicleNo, opt => opt.ResolveUsing(src => src.VehicleNo));
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
            _mapper = config.CreateMapper();

            _underTest = new CustomerVehicleService(_customerVehicleDalMock.Object, _mapper,
                _customerRegistrationServiceMock.Object, _customerInvitationServiceMock.Object, _emailServiceMock.Object,
                _customerServiceBookingMock.Object, 14);
        }

        [Test]
        public void SaveCustomerVehicle_TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomerVehicleService(null, _mapper,
                _customerRegistrationServiceMock.Object, _customerInvitationServiceMock.Object, _emailServiceMock.Object,
                _customerServiceBookingMock.Object, 14));
            Assert.Throws<ArgumentNullException>(() => new CustomerVehicleService(_customerVehicleDalMock.Object, null,
                _customerRegistrationServiceMock.Object, _customerInvitationServiceMock.Object, _emailServiceMock.Object,
                _customerServiceBookingMock.Object, 14));
            Assert.Throws<ArgumentNullException>(() => new CustomerVehicleService(_customerVehicleDalMock.Object,
                _mapper, null, _customerInvitationServiceMock.Object, _emailServiceMock.Object,
                _customerServiceBookingMock.Object, 14));
            Assert.Throws<ArgumentNullException>(() => new CustomerVehicleService(_customerVehicleDalMock.Object,
                _mapper, _customerRegistrationServiceMock.Object, null, _emailServiceMock.Object,
                _customerServiceBookingMock.Object, 14));
            Assert.Throws<ArgumentNullException>(() => new CustomerVehicleService(_customerVehicleDalMock.Object,
                _mapper, _customerRegistrationServiceMock.Object, _customerInvitationServiceMock.Object, null,
                _customerServiceBookingMock.Object, 14));
            Assert.Throws<ArgumentNullException>(() => new CustomerVehicleService(_customerVehicleDalMock.Object,
                _mapper, _customerRegistrationServiceMock.Object, _customerInvitationServiceMock.Object, _emailServiceMock.Object,
                null,14));
            Assert.Throws<ArgumentNullException>(() => new CustomerVehicleService(_customerVehicleDalMock.Object,
                _mapper, _customerRegistrationServiceMock.Object, _customerInvitationServiceMock.Object, _emailServiceMock.Object,
                null, 14));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CustomerVehicleService(_customerVehicleDalMock.Object,
                _mapper, _customerRegistrationServiceMock.Object, _customerInvitationServiceMock.Object, _emailServiceMock.Object,
                null, 0));
        }

        [Test]
        public void SaveCustomerVehicle_TestRequestNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.SaveCustomerVehicle(null, TestResources.DealerConfigurationResponse));
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.SaveCustomerVehicle(TestResources.SaveCustomerVehicleRequest, null));
        }

        [Test]
        public async Task SaveCustomerVehicle_TestNewCustomer()
        {
            _customerVehicleDalMock.Setup(m => m.GetCustomer(TestResources.SaveCustomerVehicleRequest.CustomerNo,
                TestResources.SaveCustomerVehicleRequest.RooftopId, TestResources.SaveCustomerVehicleRequest.CommunityId)).Returns((Customer)null);
            _customerVehicleDalMock.Setup(m => m.SaveCustomer(It.IsAny<Customer>())).Returns(Task.FromResult(12));

            await _underTest.SaveCustomerVehicle(TestResources.SaveCustomerVehicleRequest, TestResources.DealerConfigurationResponse);

            _customerRegistrationServiceMock.Verify(mock => mock.Register(It.IsAny<Customer>(), It.IsAny<CustomerVehicle>()), Times.Once);
            _customerVehicleDalMock.Verify(mock => mock.SaveCustomer(It.Is<Customer>(
                c => c.CustomerNo == TestResources.SaveCustomerVehicleRequest.CustomerNo
                     && c.CustomerEmail == TestResources.SaveCustomerVehicleRequest.CustomerEmail
                     && c.FirstName == TestResources.SaveCustomerVehicleRequest.FirstName
                     && c.Surname == TestResources.SaveCustomerVehicleRequest.Surname
                     && c.PhoneNumber == TestResources.SaveCustomerVehicleRequest.PhoneNumber
                     && c.RooftopId == TestResources.SaveCustomerVehicleRequest.RooftopId)), Times.Once);
            _customerVehicleDalMock.Verify(mock => mock.SaveCustomerVehicle(It.Is<CustomerVehicle>(
                c => c.CustomerId == 12
                     && c.VehicleNo == TestResources.SaveCustomerVehicleRequest.VehicleNo
                     && c.RegistrationNo == TestResources.SaveCustomerVehicleRequest.RegistrationNo
                     && c.VinNumber == TestResources.SaveCustomerVehicleRequest.VinNumber
                     && c.MakeCode == TestResources.SaveCustomerVehicleRequest.MakeCode
                     && c.ModelCode == TestResources.SaveCustomerVehicleRequest.ModelCode
                     && c.ModelYear == TestResources.SaveCustomerVehicleRequest.ModelYear
                     && c.ModelDescription == TestResources.SaveCustomerVehicleRequest.ModelDescription
                     && c.LastServiceDate == TestResources.SaveCustomerVehicleRequest.LastServiceDate
                     && c.NextServiceDate == TestResources.SaveCustomerVehicleRequest.NextServiceDate
                     && c.LastKnownMileage == TestResources.SaveCustomerVehicleRequest.LastKnownMileage
                     && c.NextServiceMileage == TestResources.SaveCustomerVehicleRequest.NextServiceMileage
                     && c.VariantCode == TestResources.SaveCustomerVehicleRequest.VariantCode
            )), Times.Once);
            _customerVehicleDalMock.Verify(mock => mock.LogInvitationDetail(It.Is<Invitation>(i => i.Method == 0)));
        }

        [Test]
        public async Task SaveCustomerVehicle_TestExistingCustomer()
        {
            _customerVehicleDalMock.Setup(m => m.GetCustomer(TestResources.SaveCustomerVehicleRequest.CustomerNo,
                TestResources.SaveCustomerVehicleRequest.RooftopId, TestResources.SaveCustomerVehicleRequest.CommunityId)).Returns(TestResources.Customer);
            _customerVehicleDalMock.Setup(m => m.SaveCustomer(It.IsAny<Customer>())).Returns(Task.FromResult(12));
            _customerInvitationServiceMock.Setup(m => m.Invite(TestResources.SaveCustomerVehicleRequest, TestResources.DealerConfigurationResponse, CommunicationMethod.Sms))
                .ReturnsAsync(CommunicationMethod.Sms);

            await _underTest.SaveCustomerVehicle(TestResources.SaveCustomerVehicleRequest, TestResources.DealerConfigurationResponse);

            _customerRegistrationServiceMock.Verify(mock => mock.Register(It.IsAny<Customer>(), It.IsAny<CustomerVehicle>()), Times.Never);
            _customerVehicleDalMock.Verify(mock => mock.SaveCustomer(It.Is<Customer>(
                c => c.CustomerNo == TestResources.SaveCustomerVehicleRequest.CustomerNo
                     && c.CustomerEmail == TestResources.SaveCustomerVehicleRequest.CustomerEmail
                     && c.FirstName == TestResources.SaveCustomerVehicleRequest.FirstName
                     && c.Surname == TestResources.SaveCustomerVehicleRequest.Surname
                     && c.PhoneNumber == TestResources.SaveCustomerVehicleRequest.PhoneNumber
                     && c.RooftopId == TestResources.SaveCustomerVehicleRequest.RooftopId)), Times.Once);
            _customerVehicleDalMock.Verify(mock => mock.SaveCustomerVehicle(It.Is<CustomerVehicle>(
                c => c.CustomerId == 12
                     && c.VehicleNo == TestResources.SaveCustomerVehicleRequest.VehicleNo
                     && c.RegistrationNo == TestResources.SaveCustomerVehicleRequest.RegistrationNo
                     && c.VinNumber == TestResources.SaveCustomerVehicleRequest.VinNumber
                     && c.MakeCode == TestResources.SaveCustomerVehicleRequest.MakeCode
                     && c.ModelCode == TestResources.SaveCustomerVehicleRequest.ModelCode
                     && c.ModelYear == TestResources.SaveCustomerVehicleRequest.ModelYear
                     && c.ModelDescription == TestResources.SaveCustomerVehicleRequest.ModelDescription
                     && c.LastServiceDate == TestResources.SaveCustomerVehicleRequest.LastServiceDate
                     && c.NextServiceDate == TestResources.SaveCustomerVehicleRequest.NextServiceDate
                     && c.LastKnownMileage == TestResources.SaveCustomerVehicleRequest.LastKnownMileage
                     && c.NextServiceMileage == TestResources.SaveCustomerVehicleRequest.NextServiceMileage
                     && c.VariantCode == TestResources.SaveCustomerVehicleRequest.VariantCode
            )), Times.Once);
            _customerVehicleDalMock.Verify(mock => mock.LogInvitationDetail(It.Is<Invitation>(i => i.Method == 0)));
        }

        [Test]
        public void GetCustomerVehicle_DealerConfigurationResponse_Is_Null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                _underTest.GetCustomerVehicle(1, 1, null));
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void GetCustomerVehicle_Test_ParametersOutOfRange(int customerId, int vehicleId)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                _underTest.GetCustomerVehicle(customerId, vehicleId, TestResources.DealerConfigurationResponse));
        }

        [Test]
        public async Task GetCustomerVehicle_Test_NotFoundCustomerVehicle()
        {
            _customerVehicleDalMock.Setup(m => m.GetCustomerVehicle(1, 1)).Returns((CustomerVehicle)null);
            _customerVehicleDalMock.Setup(m => m.GetCustomer(1, "EBBVW11Dev", "EBBETDev")).Returns((Customer)null);
            var result = await _underTest.GetCustomerVehicle(1, 1, TestResources.DealerConfigurationResponse);
            Assert.IsNull(result);
        }

        [Test]
        public void GetCustomerVehicle_Test_CustomerVehicleInvalidLink()
        {
            _customerVehicleDalMock.Setup(m => m.GetCustomerVehicle(1, 1)).Returns(new CustomerVehicle());
            _customerVehicleDalMock.Setup(m => m.GetCustomer(1, "EBBVW11Dev", "EBBETDev")).Returns(new Customer { Id = 1 });
            _customerInvitationServiceMock.Setup(m => m.ValidateLink(It.IsAny<CustomerVehicle>())).Returns(false);

            Assert.ThrowsAsync<InvitationExpiredException>(() => _underTest.GetCustomerVehicle(1, 1, TestResources.DealerConfigurationResponse));
        }

        [Test]
        public async Task GetCustomerVehicle_Test_VehicleNotFound()
        {
            _customerVehicleDalMock.Setup(m => m.GetCustomer(1, "EBBVW11Dev", "EBBETDev")).Returns(new Customer { Id = 1 });
            _customerVehicleDalMock.Setup(m => m.GetCustomerVehicle(1, 1)).Returns((CustomerVehicle)null);
            var result = await _underTest.GetCustomerVehicle(1, 1, TestResources.DealerConfigurationResponse);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetCustomerVehicle_Test_CustomerVehicleValidLink()
        {
            const string expectedCdkAutolineToken = "CdkAutolineToken";
            const string expectedCustomerLoginId = "CustomerLoginId";

            _customerVehicleDalMock.Setup(m => m.GetCustomerVehicle(5, 1))
                .Returns(TestResources.CustomerVehicle);
            _customerVehicleDalMock.Setup(m => m.GetCustomer(1, "EBBVW11Dev", "EBBETDev"))
                .Returns(TestResources.Customer);
            _customerInvitationServiceMock.Setup(m => m.ValidateLink(It.IsAny<CustomerVehicle>())).Returns(true);
            _customerRegistrationServiceMock.Setup(m => m.Verify(It.IsAny<Customer>()))
                .Returns(Task.FromResult(new VerifyCustomerResponse { CDKAutolineToken = expectedCdkAutolineToken, CustomerLoginId = expectedCustomerLoginId }));

            var result = await _underTest.GetCustomerVehicle(1, 1, TestResources.DealerConfigurationResponse);
            Assert.AreEqual(expectedCustomerLoginId, result.CustomerLoginId);
            Assert.AreEqual(TestResources.Customer.CustomerNo, result.CustomerNo);
            Assert.AreEqual(TestResources.Customer.CustomerEmail, result.CustomerEmail);
            Assert.AreEqual(expectedCdkAutolineToken, result.CdkAutolineToken);
            Assert.AreEqual(TestResources.Customer.CommunityId, result.CommunityId);
            Assert.AreEqual(TestResources.Customer.FirstName, result.FirstName);
            Assert.AreEqual(TestResources.Customer.PhoneNumber, result.PhoneNumber);
            Assert.AreEqual(TestResources.CustomerVehicle.RegistrationNo, result.RegistrationNo);
            Assert.AreEqual(TestResources.Customer.RooftopId, result.RooftopId);
            Assert.AreEqual(TestResources.Customer.Surname, result.Surname);
            Assert.AreEqual(TestResources.CustomerVehicle.VariantCode, result.VariantCode);
            Assert.AreEqual(TestResources.CustomerVehicle.VehicleNo, result.VehicleNo);
            Assert.AreEqual(TestResources.CustomerVehicle.VinNumber, result.VinNumber);
            Assert.AreEqual(TestResources.CustomerVehicle.MakeCode, result.MakeCode);
            Assert.AreEqual(TestResources.CustomerVehicle.ModelCode, result.ModelCode);
            Assert.AreEqual(TestResources.CustomerVehicle.ModelYear, result.ModelYear);
            Assert.AreEqual(TestResources.CustomerVehicle.ModelDescription, result.ModelDescription);
            Assert.AreEqual(TestResources.CustomerVehicle.LastServiceDate, result.LastServiceDate);
            Assert.AreEqual(TestResources.CustomerVehicle.NextServiceDate, result.NextServiceDate);
            Assert.AreEqual(TestResources.CustomerVehicle.LastKnownMileage, result.LastKnownMileage);
            Assert.AreEqual(TestResources.CustomerVehicle.NextServiceMileage, result.NextServiceMileage);
        }

        [Test]
        public void MapCustomer_Test()
        {
            var customer = _underTest.MapCustomer(TestResources.SaveCustomerVehicleRequest);

            Assert.IsNotNull(customer);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.CustomerNo, customer.CustomerNo);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.CustomerEmail, customer.CustomerEmail);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.RooftopId, customer.RooftopId);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.CommunityId, customer.CommunityId);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.FirstName, customer.FirstName);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.Surname, customer.Surname);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.PhoneNumber, customer.PhoneNumber);
        }

        [Test]
        public void MapCustomerVehicle_Test()
        {
            var customerVehicle = _underTest.MapCustomerVehicle(TestResources.SaveCustomerVehicleRequest, TestResources.Customer);

            Assert.IsNotNull(customerVehicle);
            Assert.AreEqual(TestResources.Customer.Id, customerVehicle.Id);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.VehicleNo, customerVehicle.VehicleNo);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.RegistrationNo, customerVehicle.RegistrationNo);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.VinNumber, customerVehicle.VinNumber);
            Assert.AreEqual(TestResources.CustomerVehicle.MakeCode, customerVehicle.MakeCode);
            Assert.AreEqual(TestResources.CustomerVehicle.ModelCode, customerVehicle.ModelCode);
            Assert.AreEqual(TestResources.CustomerVehicle.ModelYear, customerVehicle.ModelYear);
            Assert.AreEqual(TestResources.CustomerVehicle.ModelDescription, customerVehicle.ModelDescription);
            Assert.AreEqual(TestResources.CustomerVehicle.LastServiceDate, customerVehicle.LastServiceDate);
            Assert.AreEqual(TestResources.CustomerVehicle.NextServiceDate, customerVehicle.NextServiceDate);
            Assert.AreEqual(TestResources.CustomerVehicle.LastKnownMileage, customerVehicle.LastKnownMileage);
            Assert.AreEqual(TestResources.CustomerVehicle.NextServiceMileage, customerVehicle.NextServiceMileage);
            Assert.IsFalse(customerVehicle.IsProcessed);
        }

        [TestCase(CommunicationMethod.Email, 2)]
        [TestCase(CommunicationMethod.Sms, 1)]
        public void MapInvitation_Test(CommunicationMethod method, int communicationMethodId)
        {
            var invitation = _underTest.MapInvitation(TestResources.SaveCustomerVehicleRequest, TestResources.Invitation.DealerId,
                communicationMethodId, method);

            Assert.IsNotNull(invitation);
            Assert.AreEqual(
                method == CommunicationMethod.Sms
                    ? TestResources.Customer.PhoneNumber
                    : TestResources.Customer.CustomerEmail, invitation.ContactDetail);

            Assert.AreEqual(TestResources.Invitation.DealerId, invitation.DealerId);
            Assert.AreEqual((int)Enum.Parse(typeof(CommunicationMethod), method.ToString(), true), invitation.Method);
        }

        [TestCase(-1, 12, 12)]
        [TestCase(12, -1, 12)]
        [TestCase(12, 12, -1)]
        public void DismissVehicleOwnership_InvalidParam_Test(int customerId, int dealerId, int vehicleNo)
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>((() => _underTest.DismissVehicleOwnership(new DismissVehicleOwnershipRequest()
            {
                CustomerNo = customerId,
                DealerId = dealerId,
                VehicleNo = vehicleNo
            }, TestResources.DealerConfigurationResponse)));

            Assert.ThrowsAsync<ArgumentNullException>((() => _underTest.DismissVehicleOwnership(TestResources.DismissVehicleOwnershipRequest, null)));
        }

        [Test]
        public void DismissVehicleOwnerShip_InvalidCustomer()
        {
            _customerVehicleDalMock
                .Setup(mock => mock.GetCustomer(TestResources.DismissVehicleOwnershipRequest.CustomerNo,
                    TestResources.DealerConfigurationResponse.RooftopId, TestResources.DealerConfigurationResponse.CommunityId))
                .Returns((Customer)null);

            Assert.ThrowsAsync<InvalidCustomerException>(() => _underTest.DismissVehicleOwnership(
                TestResources.DismissVehicleOwnershipRequest, TestResources.DealerConfigurationResponse));
        }

        [Test]
        public void DismissVehicleOwnerShip_InvalidCustomerVehicle()
        {
            _customerVehicleDalMock
                .Setup(mock => mock.GetCustomer(TestResources.DismissVehicleOwnershipRequest.CustomerNo,
                    TestResources.DealerConfigurationResponse.RooftopId, TestResources.DealerConfigurationResponse.CommunityId))
                .Returns(TestResources.Customer);

            _customerVehicleDalMock
                .Setup(mock => mock.GetCustomerVehicle(TestResources.Customer.Id, TestResources.DismissVehicleOwnershipRequest.VehicleNo))
                .Returns((CustomerVehicle)null);

            Assert.ThrowsAsync<InvalidCustomerException>(() => _underTest.DismissVehicleOwnership(
                TestResources.DismissVehicleOwnershipRequest, TestResources.DealerConfigurationResponse));
        }

        [Test]
        public async Task DismissVehicleOwnerShip_Test()
        {
            _customerVehicleDalMock
                .Setup(mock => mock.GetCustomer(TestResources.DismissVehicleOwnershipRequest.CustomerNo,
                    TestResources.DealerConfigurationResponse.RooftopId, TestResources.DealerConfigurationResponse.CommunityId))
                .Returns(TestResources.Customer);

            _customerVehicleDalMock
                .Setup(mock => mock.GetCustomerVehicle(TestResources.Customer.Id, TestResources.DismissVehicleOwnershipRequest.VehicleNo))
                .Returns(TestResources.CustomerVehicle);

            await _underTest.DismissVehicleOwnership(
                TestResources.DismissVehicleOwnershipRequest,
                TestResources.DealerConfigurationResponse);

            _customerVehicleDalMock.Verify(mock => mock.GetCustomer(
                    TestResources.DismissVehicleOwnershipRequest.CustomerNo, TestResources.DealerConfigurationResponse.RooftopId, TestResources.DealerConfigurationResponse.CommunityId),
                Times.Once);
            _customerVehicleDalMock.Verify(mock => mock.GetCustomerVehicle(TestResources.Customer.Id, TestResources.DismissVehicleOwnershipRequest.VehicleNo), Times.Once);
            _customerVehicleDalMock.Verify(mock => mock.DeleteCustomerVehicle(TestResources.Customer.Id, TestResources.CustomerVehicle.VehicleNo), Times.Once);
            _emailServiceMock.Verify(mock => mock.SendDismissVehicleOwnerShipEmail(
                It.IsAny<Customer>(), It.IsAny<DismissVehicleOwnershipRequest>(), TestResources.DealerConfigurationResponse.EmailAddress, TestResources.CustomerVehicle.RegistrationNo));
        }

        [Test]
        public async Task UpdateCustomerContact_Test()
        {
            _customerVehicleDalMock.Setup(mock => mock.GetCustomer(TestResources.UpdateCustomerContactRequest.CustomerNo,
                    TestResources.DealerConfigurationResponse.RooftopId, TestResources.DealerConfigurationResponse.CommunityId))
                .Returns(TestResources.Customer);
            _emailServiceMock.Setup(mock => mock.SendUpdateCustomerContactEmail(TestResources.UpdateCustomerContactRequest,
                TestResources.Customer.CustomerNo, TestResources.DealerConfigurationResponse.EmailAddress));
            await _underTest.UpdateCustomerContact(TestResources.UpdateCustomerContactRequest, TestResources.DealerConfigurationResponse);
            _emailServiceMock.Verify(mock => mock.SendUpdateCustomerContactEmail(It.IsAny<UpdateCustomerContactRequest>(),
                It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void UpdateCustomerContact_InvalidCustomer_Test()
        {
            Assert.ThrowsAsync<InvalidCustomerException>(() => _underTest.UpdateCustomerContact(TestResources.UpdateCustomerContactRequest,
                TestResources.DealerConfigurationResponse));
        }

        [Test]
        public void CustomerServiceBooking_NullRoofTopId()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.CreateServiceBooking(null, new DealerConfigurationResponse {RooftopId = null}));
        }

        [Test]
        public void CustomerServiceBooking_NullCommunityId()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.CreateServiceBooking(null, new DealerConfigurationResponse {CommunityId = null}));
        }

        [Test]
        public void CustomerServiceBooking_EmptyRoofTopId()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.CreateServiceBooking(null, new DealerConfigurationResponse {RooftopId = String.Empty}));
        }

        [Test]
        public void CustomerServiceBooking_EmptyCommunityId()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.CreateServiceBooking(null, new DealerConfigurationResponse {CommunityId = String.Empty}));
        }

        [Test]
        public async Task CustomerServiceBooking_Test()
        {
            _customerServiceBookingMock
                .Setup(mock => mock.CreateServiceBooking(It.IsAny<CDKCreateServiceBookingRequest>()))
                .Returns(Task.FromResult(TestResources.CreateServiceBookingResponse));
            _customerVehicleDalMock.Setup(mock => mock.GetCustomer(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(TestResources.Customer);
            _customerVehicleDalMock.Setup(mock => mock.GetCustomerVehicle(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(TestResources.CustomerVehicle);

            var serviceBookingResponse = await _underTest.CreateServiceBooking(TestResources.CreateServiceBookingRequest, TestResources.DealerConfigurationResponse);
            Assert.NotNull(serviceBookingResponse);
            Assert.AreEqual(serviceBookingResponse.WipNo, TestResources.CreateServiceBookingResponse.WipNo);
            Assert.AreEqual(serviceBookingResponse.AppointmentId, TestResources.CreateServiceBookingResponse.AppointmentId);
            Assert.AreEqual(serviceBookingResponse.Result.ErrorCode, TestResources.CreateServiceBookingResponse.Result.ErrorCode);
            _customerVehicleDalMock.Verify(mock => mock.SaveServiceBooking(It.IsAny<ServiceBookings>()));
        }

        [Test]
        public void CustomerServiceBooking_InvalidCustomer()
        {
            _customerVehicleDalMock.Setup(mock => mock.GetCustomer(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((Customer) null);

            Assert.ThrowsAsync<InvalidCustomerException>(() =>
                _underTest.CreateServiceBooking(TestResources.CreateServiceBookingRequest, TestResources.DealerConfigurationResponse));
        }

        [Test]
        public void CustomerServiceBooking_InvalidCustomerVehicle()
        {
            _customerVehicleDalMock.Setup(mock => mock.GetCustomer(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(TestResources.Customer);
            _customerVehicleDalMock.Setup(mock => mock.GetCustomerVehicle(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((CustomerVehicle) null);

            Assert.ThrowsAsync<InvalidCustomerException>(() =>
                _underTest.CreateServiceBooking(TestResources.CreateServiceBookingRequest, TestResources.DealerConfigurationResponse));
        }

        [Test]
        public void MapServiceBooking_Test()
        {
            var serviceBooking = _underTest.MapServiceBooking(TestResources.CreateServiceBookingRequest,
                TestResources.CreateServiceBookingResponse);
            Assert.NotNull(serviceBooking);
            Assert.AreEqual(TestResources.ServiceBookings.CustomerNo, serviceBooking.CustomerNo);
            Assert.IsNotNull(serviceBooking.CreateTime);
            Assert.AreEqual(TestResources.ServiceBookings.DealerId, serviceBooking.DealerId);
            Assert.AreEqual(TestResources.ServiceBookings.VehicleNo, serviceBooking.VehicleNo);
            Assert.AreEqual(TestResources.ServiceBookings.BookingReference, serviceBooking.BookingReference);
        }

        [Test]
        public void ExistingServiceBooking_Test()
        {
            _customerVehicleDalMock
                .Setup(mock => mock.GetServiceBooking(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ServiceBookings()
                {
                    CreateTime = DateTime.Now
                });

            Assert.IsNotNull(_underTest.ExistingServiceBooking(TestResources.Customer.CustomerNo,
                TestResources.CustomerVehicle.VehicleNo,
                TestResources.DealerConfigurationResponse.DealerId));

        }

        [Test]
        public void MapCreateServiceBookingRequest_Test()
        {
            var cdkCreateServiceBookingRequest = _underTest.MapCreateServiceBookingRequest(TestResources.Customer, TestResources.CustomerVehicle, TestResources.CreateServiceBookingRequest);
            Assert.AreEqual(TestResources.Customer.RooftopId, cdkCreateServiceBookingRequest.RooftopId);
            Assert.AreEqual(TestResources.Customer.CommunityId, cdkCreateServiceBookingRequest.CommunityId);
            Assert.AreEqual(TestResources.Customer.CustomerNo, cdkCreateServiceBookingRequest.CustomerNo);
            Assert.AreEqual(TestResources.Customer.CustomerEmail, cdkCreateServiceBookingRequest.EmailAddress);
            Assert.AreEqual(TestResources.Customer.PhoneNumber, cdkCreateServiceBookingRequest.MobileTelNo);
            Assert.AreEqual(TestResources.Customer.FirstName, cdkCreateServiceBookingRequest.FirstName);
            Assert.AreEqual(TestResources.Customer.Surname, cdkCreateServiceBookingRequest.SurName);
            Assert.AreEqual(TestResources.CustomerVehicle.RegistrationNo, cdkCreateServiceBookingRequest.VehicleRegistrationNo);
            Assert.AreEqual(TestResources.CustomerVehicle.VehicleNo, cdkCreateServiceBookingRequest.VehicleNo);
            Assert.AreEqual(TestResources.CustomerVehicle.MakeCode, cdkCreateServiceBookingRequest.VehMakeCode);
            Assert.AreEqual(TestResources.CustomerVehicle.ModelCode, cdkCreateServiceBookingRequest.VehModelCode);
            Assert.AreEqual(TestResources.CustomerVehicle.VariantCode, cdkCreateServiceBookingRequest.VehVariantCode);
            Assert.AreEqual(TestResources.CreateServiceBookingRequest.ActualMileage, cdkCreateServiceBookingRequest.ActualMileage);
            Assert.AreEqual(TestResources.CreateServiceBookingRequest.JobDate, cdkCreateServiceBookingRequest.JobDate);
            Assert.AreEqual(TestResources.CreateServiceBookingRequest.TransportMethod, cdkCreateServiceBookingRequest.TransportMethod);
            Assert.AreEqual(TestResources.CreateServiceBookingRequest.AdvisorID, cdkCreateServiceBookingRequest.AdvisorId);
            Assert.AreEqual(TestResources.CreateServiceBookingRequest.AdvisorDropOffTimeCode, cdkCreateServiceBookingRequest.AdvisorDropOffTimeCode);
            Assert.AreEqual(TestResources.CreateServiceBookingRequest.Jobs.Count, cdkCreateServiceBookingRequest.Jobs.Count);
            Assert.AreEqual(TestResources.CreateServiceBookingRequest.Jobs.FirstOrDefault()?.JobCode, cdkCreateServiceBookingRequest.Jobs.FirstOrDefault()?.JobCode);
        }
    }
}