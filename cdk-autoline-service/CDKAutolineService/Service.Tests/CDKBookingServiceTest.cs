using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.DbModels;
using Service.Contract.Exceptions;
using Service.Contract.Models;
using Service.Contract.Response;
using Service.Implementation;
using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace Service.Tests
{
    [TestFixture]
    public class CDKBookingServiceTest
    {
        private Mock<IRestApiClient> _restApiClientMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<ICdkCustomerDAL> _cdkCustomerDALMock;
        private CDKBookingService _underTest;
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _restApiClientMock = new Mock<IRestApiClient>();
            _tokenServiceMock = new Mock<ITokenService>();
            _cdkCustomerDALMock = new Mock<ICdkCustomerDAL>();
            _underTest = new CDKBookingService(_restApiClientMock.Object, _tokenServiceMock.Object, _cdkCustomerDALMock.Object,
                null, TestResources.CdkAutolineUrl);
            _cdkCustomerDALMock.Setup(mock => mock.GetCdkCustomer(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(TestResources.CdkCustomer);
            _restApiClientMock.Setup(mock => mock.Invoke<CreateServiceBookingResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = true, Result = TestResources.CreateServiceBookingResponse }));

            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [Test]
        public void CDKAutolineService_Constructore_Exception_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new CDKBookingService(null,
                _tokenServiceMock.Object, _cdkCustomerDALMock.Object, null, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKBookingService(_restApiClientMock.Object,
                null, _cdkCustomerDALMock.Object, null, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKBookingService(_restApiClientMock.Object,
                _tokenServiceMock.Object, null, null, TestResources.CdkAutolineUrl));

            Assert.Throws<ArgumentNullException>(() => new CDKBookingService(_restApiClientMock.Object,
                _tokenServiceMock.Object, _cdkCustomerDALMock.Object, null, null));
        }

        [Test]
        public void CreateServiceBooking_InvalidCustomer_Exception_Test()
        {
            _cdkCustomerDALMock.Setup(mock => mock.GetCdkCustomer(It.IsAny<string>(), It.IsAny<int>()))
                .Returns((CdkCustomer)null);
            Assert.ThrowsAsync<InvalidCustomerException>(() => _underTest.CreateServiceBooking(TestResources.CreateServiceBookingRequest));
        }

        [Test]
        public void CreateServiceBooking_NullRequest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() =>
                _underTest.CreateServiceBooking(It.IsAny<CreateServiceBookingRequest>()));
        }

        [Test]
        public async Task CreateServiceBooking_Test()
        {
            var createServiceBookingResponse = await _underTest.CreateServiceBooking(TestResources.CreateServiceBookingRequest);

            Assert.AreEqual(TestResources.CreateServiceBookingResponse.AppointmentId,
                createServiceBookingResponse.AppointmentId);
            Assert.AreEqual(TestResources.CreateServiceBookingResponse.WipNo, createServiceBookingResponse.WipNo);
            Assert.AreEqual(TestResources.CreateServiceBookingResponse.Result.ErrorCode, createServiceBookingResponse.Result.ErrorCode);

            _cdkCustomerDALMock.Verify(mock => mock.GetCdkCustomer(It.IsAny<string>(), It.IsAny<int>()));
            _restApiClientMock.Verify(mock => mock.Invoke<CreateServiceBookingResponse>(It.IsAny<ApiRequest>()));
        }

        [Test]
        public void CreateServiceBooking_CreateBookingFailed_Test()
        {
            _restApiClientMock.Setup(mock => mock.Invoke<CreateServiceBookingResponse>(It.IsAny<ApiRequest>()))
                .Returns(Task.FromResult(new ApiResponse() { Success = false }));

            Assert.ThrowsAsync<CDKAutolineException>(() => _underTest.CreateServiceBooking(TestResources.CreateServiceBookingRequest));
        }

        [Test]
        public void MapToCdkCreateServiceBookingRequest_Test()
        {
            var createServiceBookingRequest = _fixture.Create<CreateServiceBookingRequest>();
            var cdkCustomer = _fixture.Create<CdkCustomer>();

            var result = _underTest.MapToCdkCreateServiceBookingRequest(createServiceBookingRequest, cdkCustomer);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.RooftopId, createServiceBookingRequest.RooftopId);
            Assert.AreEqual(result.CustomerId, cdkCustomer.CustomerLoginId);
            Assert.AreEqual(result.EmailAddress, createServiceBookingRequest.EmailAddress);
            Assert.AreEqual(result.MobileTelNo, createServiceBookingRequest.MobileTelNo);
            Assert.AreEqual(result.FirstName, createServiceBookingRequest.FirstName);
            Assert.AreEqual(result.SurName, createServiceBookingRequest.SurName);
            Assert.AreEqual(result.VehicleRegistrationNo, createServiceBookingRequest.VehicleRegistrationNo);
            Assert.AreEqual(result.VehMakeCode, createServiceBookingRequest.VehMakeCode);
            Assert.AreEqual(result.VehModelCode, createServiceBookingRequest.VehModelCode);
            Assert.AreEqual(result.VehVariantCode, createServiceBookingRequest.VehVariantCode);
            Assert.AreEqual(result.Jobs, createServiceBookingRequest.Jobs);
            Assert.AreEqual(result.JobDate, createServiceBookingRequest.JobDate);
            Assert.AreEqual(result.TransportMethod, createServiceBookingRequest.TransportMethod);
            Assert.AreEqual(result.AdvisorID, createServiceBookingRequest.AdvisorId);
            Assert.AreEqual(result.AdvisorDropOffTimeCode, createServiceBookingRequest.AdvisorDropOffTimeCode);
            Assert.AreEqual(result.SendConfirmationMail, true);
        }
    }
}
