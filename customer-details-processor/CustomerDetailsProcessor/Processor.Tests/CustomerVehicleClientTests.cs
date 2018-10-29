using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using Processor.Contract;
using Processor.Implementation;

namespace Processor.Tests
{
    [TestFixture]
    public class CustomerVehicleClientTests
    {
        private CustomerVehicleClient _underTest;
        private Mock<IRestfulClient> _restfulClientMock;
        private IMapper _mapper;

        private readonly CustomerVehicle _customerVehicle = new CustomerVehicle()
        {
            CustomerNo = 123456,
            CustomerEmail = "test@mail.com",
            RooftopId = "ABC123",
            VehicleNo = 12345678,
            RegistrationNo = "HBC123",
            CommunityId = "abcd1234",
            FirstName = "First Name",
            Surname = "Last Name",
            VinNumber = "HBC234",
            PhoneNumber = "0212104571"
        };

        [SetUp]
        public void SetUp()
        {
            _restfulClientMock = new Mock<IRestfulClient>();
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<CustomerVehicle, SaveCustomerVehicleRequest>(); });
            _mapper = config.CreateMapper();
            _underTest = new CustomerVehicleClient(_restfulClientMock.Object, _mapper);
        }

        [Test]
        public void CustomerVehicleClient_TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomerVehicleClient(null, _mapper));
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerVehicleClient(_restfulClientMock.Object, null));
        }

        [Test]
        public void CustomerVehicleClient_SaveCustomerVehicle_NullRequest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.SaveCustomerVehicle(null));
        }

        [Test]
        public async Task CustomerVehicleClient_SaveCustomerVehicle()
        {
            _restfulClientMock.Setup(m => m.PostAsync("SaveCustomerVehicle", It.IsAny<SaveCustomerVehicleRequest>()))
                .Returns(Task.FromResult(new HttpResponseMessage {StatusCode = HttpStatusCode.OK}));
            await _underTest.SaveCustomerVehicle(_customerVehicle);
            _restfulClientMock.Verify(m => m.PostAsync("SaveCustomerVehicle", It.IsAny<SaveCustomerVehicleRequest>()),
                Times.Once);
        }

        [Test]
        public void GetSaveCustomerVehicleRequest_Test()
        {
            var result = _underTest.GetSaveCustomerVehicleRequest(_customerVehicle);
            Assert.AreEqual(_customerVehicle.CustomerNo, result.CustomerNo);
            Assert.AreEqual(_customerVehicle.CustomerEmail, result.CustomerEmail);
            Assert.AreEqual(_customerVehicle.FirstName, result.FirstName);
            Assert.AreEqual(_customerVehicle.Surname, result.Surname);
            Assert.AreEqual(_customerVehicle.PhoneNumber, result.PhoneNumber);
            Assert.AreEqual(_customerVehicle.RooftopId, result.RooftopId);
            Assert.AreEqual(_customerVehicle.CommunityId, result.CommunityId);
            Assert.AreEqual(_customerVehicle.VehicleNo, result.VehicleNo);
            Assert.AreEqual(_customerVehicle.RegistrationNo, result.RegistrationNo);
            Assert.AreEqual(_customerVehicle.VinNumber, result.VinNumber);
            Assert.AreEqual(_customerVehicle.MakeCode, result.MakeCode);
            Assert.AreEqual(_customerVehicle.ModelCode, result.ModelCode);
            Assert.AreEqual(_customerVehicle.ModelYear, result.ModelYear);
            Assert.AreEqual(_customerVehicle.ModelDescription, result.ModelDescription);
            Assert.AreEqual(_customerVehicle.VariantCode, result.VariantCode);
            Assert.AreEqual(_customerVehicle.NextServiceMileage, result.NextServiceMileage);
        }
    }
}