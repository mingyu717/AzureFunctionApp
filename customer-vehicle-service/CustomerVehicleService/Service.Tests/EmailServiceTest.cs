using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture]
    public class EmailServiceTest
    {
        private Mock<IEmailGatewayClient> _emailGatewayClientMock;
        private EmailService _underTest;
        private readonly string _serviceBookingEmail = "test@email.com";

        [SetUp]
        public void Setup()
        {
            _emailGatewayClientMock = new Mock<IEmailGatewayClient>();
            _underTest = new EmailService(_emailGatewayClientMock.Object,_serviceBookingEmail);
        }

        [Test]
        public void EmailService_Construtor_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailService(null, _serviceBookingEmail));
            Assert.Throws<ArgumentNullException>(() => new EmailService(_emailGatewayClientMock.Object, null));
        }

        [Test]
        public async Task DismissVehicleOwnerShip_Test()
        {
            await _underTest.SendDismissVehicleOwnerShipEmail(TestResources.Customer,
                TestResources.DismissVehicleOwnershipRequest,
                TestResources.DealerConfigurationResponse.EmailAddress, TestResources.CustomerVehicle.RegistrationNo);

            _emailGatewayClientMock.Verify(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public async Task SendUpdateCustomerContactEmail_Test()
        {
            await _underTest.SendUpdateCustomerContactEmail(TestResources.UpdateCustomerContactRequest,
                TestResources.Customer.CustomerNo, TestResources.DealerConfigurationResponse.EmailAddress);
            _emailGatewayClientMock.Verify(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }
    }
}
