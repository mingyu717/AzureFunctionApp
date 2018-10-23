using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Service.Contract;
using Service.Contract.Response;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture]
    public class CustomerRegisterationServiceTests
    {
        private Mock<IRestfulClient> _restfullClientMock;
        private CustomerRegistrationService _underTest;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _restfullClientMock = new Mock<IRestfulClient>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SaveCustomerVehicleRequest, Customer>()
                    .ForMember(dest => dest.CustomerNo, opt => opt.ResolveUsing(src => src.CustomerNo));
                cfg.CreateMap<SaveCustomerVehicleRequest, CustomerVehicle>();
                cfg.CreateMap<Customer, RegisterCustomerRequest>()
                    .ForMember(dest => dest.RoofTopId, opt => opt.ResolveUsing(src => src.RooftopId))
                    .ForMember(dest => dest.EmailAddress, opt => opt.ResolveUsing(src => src.CustomerEmail));
                cfg.CreateMap<CustomerVehicle, RegisterCustomerRequest>()
                    .ForMember(dest => dest.RegistrationNumber, opt => opt.ResolveUsing(src => src.RegistrationNo))
                    .ForMember(dest => dest.VehicleId, opt => opt.ResolveUsing(src => src.VehicleNo));
                cfg.CreateMap<Customer, VerifyCustomerRequest>()
                    .ForMember(dest => dest.RoofTopId, opt => opt.ResolveUsing(src => src.RooftopId));
            });
            _mapper = config.CreateMapper();

            _underTest =
                new CustomerRegistrationService(_mapper, _restfullClientMock.Object, null);
        }

        [Test]
        public void CustomerRegistrationService_TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerRegistrationService(null, _restfullClientMock.Object, null));
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerRegistrationService(_mapper, null, null));
        }

        [Test]
        public void Register_Test_RequestNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.Register(null, TestResources.CustomerVehicle));
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.Register(TestResources.Customer, null));
        }

        [Test]
        public async Task Register_Test_ValidRequest()
        {
            await _underTest.Register(TestResources.Customer, TestResources.CustomerVehicle);
            _restfullClientMock.Verify(mock => mock.PostAsync("registercustomer", It.Is<RegisterCustomerRequest>(
                request => request.CustomerNo == TestResources.Customer.CustomerNo
                           && request.RoofTopId == TestResources.Customer.RooftopId
                           && request.FirstName == TestResources.Customer.FirstName
                           && request.Surname == TestResources.Customer.Surname
                           && request.EmailAddress == TestResources.Customer.CustomerEmail
                           && request.CommunityId == TestResources.Customer.CommunityId
                           && request.VehicleId == TestResources.CustomerVehicle.VehicleNo.ToString()
                           && request.MakeCode == TestResources.CustomerVehicle.MakeCode
                           && request.ModelCode == TestResources.CustomerVehicle.ModelCode
                           && request.VariantCode == TestResources.CustomerVehicle.VariantCode
                           && request.RegistrationNumber == TestResources.CustomerVehicle.RegistrationNo
            )), Times.Once);
        }

        [Test]
        public void Verify_Test_RequestNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.Verify(null));
        }

        [Test]
        public async Task Verify_Test_ValidRequest()
        {
            const string expected = "CDKAutolineToken";

            _restfullClientMock.Setup(mock => mock.PostAsync<VerifyCustomerRequest, VerifyCustomerResponse>(
                    "verifycustomer",
                    It.IsAny<VerifyCustomerRequest>()))
                .Returns(Task.FromResult(new VerifyCustomerResponse {CDKAutolineToken = expected}));

            var result = await _underTest.Verify(TestResources.Customer);

            _restfullClientMock.Verify(
                mock => mock.PostAsync<VerifyCustomerRequest, VerifyCustomerResponse>("verifycustomer",
                    It.IsAny<VerifyCustomerRequest>()),
                Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.CDKAutolineToken);
        }

        [Test]
        public void GetRegisterCustomerRequest_Test()
        {
            var registerCustomerRequest = _underTest.GetRegisterCustomerRequest(TestResources.Customer, TestResources.CustomerVehicle);

            Assert.IsNotNull(registerCustomerRequest);
            Assert.AreEqual(TestResources.Customer.CustomerNo, registerCustomerRequest.CustomerNo);
            Assert.AreEqual(TestResources.Customer.RooftopId, registerCustomerRequest.RoofTopId);
            Assert.AreEqual(TestResources.Customer.FirstName, registerCustomerRequest.FirstName);
            Assert.AreEqual(TestResources.Customer.Surname, registerCustomerRequest.Surname);
            Assert.AreEqual(TestResources.Customer.CommunityId, registerCustomerRequest.CommunityId);
            Assert.AreEqual(TestResources.CustomerVehicle.VehicleNo, int.Parse(registerCustomerRequest.VehicleId));
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.MakeCode, registerCustomerRequest.MakeCode);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.ModelCode, registerCustomerRequest.ModelCode);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.VariantCode, registerCustomerRequest.VariantCode);
            Assert.AreEqual(TestResources.SaveCustomerVehicleRequest.RegistrationNo,
                registerCustomerRequest.RegistrationNumber);
            Assert.AreEqual(TestResources.Customer.PhoneNumber, registerCustomerRequest.PhoneNumber);
        }

        [Test]
        public void GetVerifyCustomerRequest_Test()
        {
            var verifyCustomerRequest = _underTest.GetVerifyCustomerRequest(TestResources.Customer);

            Assert.IsNotNull(verifyCustomerRequest);
            Assert.AreEqual(TestResources.Customer.CustomerNo, verifyCustomerRequest.CustomerNo);
            Assert.AreEqual(TestResources.Customer.RooftopId, verifyCustomerRequest.RoofTopId);
            Assert.AreEqual(TestResources.Customer.CommunityId, verifyCustomerRequest.CommunityId);
        }
    }
}