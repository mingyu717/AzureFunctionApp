using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Service.Contract;
using Service.Contract.Response;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture]
    public class CustomerServiceBookingTests
    {
        private Mock<IRestfulClient> _restfullClientMock;
        private CustomerServiceBooking _underTest;
        
        [SetUp]
        public void SetUp()
        {
            _restfullClientMock = new Mock<IRestfulClient>();
            _underTest =
                new CustomerServiceBooking(_restfullClientMock.Object, null);
        }

        [Test]
        public async Task CreateServiceBooking_Test()
        {
            const string CreateServiceBookingUrl = "createservicebooking";

            _restfullClientMock.Setup(mock => mock.PostAsync<CDKCreateServiceBookingRequest, CreateServiceBookingResponse>(
                    CreateServiceBookingUrl,
                    It.IsAny<CDKCreateServiceBookingRequest>()))
                .Returns(Task.FromResult(new CreateServiceBookingResponse()));

            var result = await _underTest.CreateServiceBooking(TestResources.CDKCreateServiceBookingRequest);

            _restfullClientMock.Verify(
                mock => mock.PostAsync<CDKCreateServiceBookingRequest, CreateServiceBookingResponse>(CreateServiceBookingUrl,
                    It.IsAny<CDKCreateServiceBookingRequest>()),
                Times.Once);

            Assert.IsNotNull(result);          
        }
    }
}