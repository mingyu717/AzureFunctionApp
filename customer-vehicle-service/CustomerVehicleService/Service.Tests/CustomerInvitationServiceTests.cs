using Moq;
using NUnit.Framework;
using PhoneNumbers;
using Service.Contract;
using Service.Implementation;
using System;
using System.Threading.Tasks;
using Service.Contract.Exceptions;
using Service.Contract.Response;

namespace Service.Tests
{
    [TestFixture]
    public class CustomerInvitationServiceTests
    {
        private CustomerInvitationService _underTest;
        private Mock<ISMSGatewayClient> _smsGatewayClientMock;
        private Mock<IEmailGatewayClient> _emailGateWayClientMock;
        private Mock<IDealerConfigurationService> _dealerConfigurationServiceMock;
        private static readonly string serviceBookingAppUrl = "https://service.bookingapp.com";
        private static readonly string seriveBookingUrlPlaceHolder = "SERVICE-BOOKING-INVITATION-URL";
        private static readonly string registrationNoPlaceholder = "REGISTRATION-NUMBER";

        [SetUp]
        public void SetUp()
        {
            _smsGatewayClientMock = new Mock<ISMSGatewayClient>();
            _emailGateWayClientMock = new Mock<IEmailGatewayClient>();
            _dealerConfigurationServiceMock = new Mock<IDealerConfigurationService>();
            _underTest =
                new CustomerInvitationService(serviceBookingAppUrl, 14, "+649123456", _smsGatewayClientMock.Object,
                    _emailGateWayClientMock.Object, null, _dealerConfigurationServiceMock.Object,
                    seriveBookingUrlPlaceHolder, registrationNoPlaceholder);
        }

        [Test]
        public void CustomerRegistrationService_TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerInvitationService(null, 14, "+649123456", _smsGatewayClientMock.Object,
                    _emailGateWayClientMock.Object, null,
                    _dealerConfigurationServiceMock.Object, seriveBookingUrlPlaceHolder, registrationNoPlaceholder));

            Assert.Throws<ArgumentNullException>(() =>
                new CustomerInvitationService(serviceBookingAppUrl, 14, "+649123456", null,
                    _emailGateWayClientMock.Object, null,
                    _dealerConfigurationServiceMock.Object, seriveBookingUrlPlaceHolder, registrationNoPlaceholder));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new CustomerInvitationService(serviceBookingAppUrl, 0, "+649123456", _smsGatewayClientMock.Object,
                    _emailGateWayClientMock.Object, null,
                    _dealerConfigurationServiceMock.Object, seriveBookingUrlPlaceHolder, registrationNoPlaceholder));

            Assert.Throws<ArgumentNullException>(() =>
                new CustomerInvitationService(serviceBookingAppUrl, 14, null, _smsGatewayClientMock.Object,
                    _emailGateWayClientMock.Object, null,
                    _dealerConfigurationServiceMock.Object, seriveBookingUrlPlaceHolder, registrationNoPlaceholder));

            Assert.Throws<ArgumentNullException>(() =>
                new CustomerInvitationService(serviceBookingAppUrl, 14, "+649123456",
                    _smsGatewayClientMock.Object, null, null,
                    _dealerConfigurationServiceMock.Object, seriveBookingUrlPlaceHolder, registrationNoPlaceholder));
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerInvitationService(serviceBookingAppUrl, 14, "+649123456",
                    _smsGatewayClientMock.Object, _emailGateWayClientMock.Object, null,
                    null, seriveBookingUrlPlaceHolder, registrationNoPlaceholder));
            Assert.Throws<ArgumentNullException>(() =>
                new CustomerInvitationService(serviceBookingAppUrl, 14, "+649123456",
                    _smsGatewayClientMock.Object, _emailGateWayClientMock.Object, null,
                    _dealerConfigurationServiceMock.Object, null, null));
        }

        [TestCase(CommunicationMethod.Sms, ExpectedResult = CommunicationMethod.Sms)]
        [TestCase(CommunicationMethod.SmsOrEmail, ExpectedResult = CommunicationMethod.Sms)]
        [TestCase(CommunicationMethod.EmailOrSms, ExpectedResult = CommunicationMethod.Email)]
        [TestCase(CommunicationMethod.Email, ExpectedResult = CommunicationMethod.Email)]
        public async Task<CommunicationMethod> Invite_Test(CommunicationMethod method)
        {
            _smsGatewayClientMock
                .Setup(mock => mock.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((true, null));
            _emailGateWayClientMock
                .Setup(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((true, null));
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestResources.DealerInvitationResponse);
            return await _underTest.Invite(TestResources.SaveCustomerVehicleRequest, TestResources.DealerConfigurationResponse, method);
        }

        [TestCase(CommunicationMethod.Sms)]
        [TestCase(CommunicationMethod.Email)]
        public void Invite_Exception_Test(CommunicationMethod method)
        {
            _smsGatewayClientMock
                .Setup(mock => mock.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((false, new Exception()));
            _emailGateWayClientMock
                .Setup(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((false, new Exception()));
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestResources.DealerInvitationResponse);

            Assert.ThrowsAsync<Exception>(() => _underTest.Invite(TestResources.SaveCustomerVehicleRequest, TestResources.DealerConfigurationResponse, method));

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                _underTest.Invite(null, TestResources.DealerConfigurationResponse, method));
        }

        [Test]
        public void SendInvite_NullDealerInvitationResponse()
        {
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((DealerInvitationContentResponse) null);

            Assert.ThrowsAsync<DealerInvitationContentException>(() =>
                _underTest.Invite(TestResources.SaveCustomerVehicleRequest, TestResources.DealerConfigurationResponse, CommunicationMethod.Sms));
        }

        [Test]
        public async Task SendInvite_DefaultCommunicationMethodNone()
        {
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestResources.DealerInvitationResponse);
            var result = await _underTest.Invite(TestResources.SaveCustomerVehicleRequestWithoutPhoneNumber,
                TestResources.DealerConfigurationResponse, (CommunicationMethod) 5);

            Assert.AreEqual(CommunicationMethod.None, result);
        }

        [Test]
        public async Task Invite_PhoneNo_NotAvailable_Send_Html_Email_Test()
        {
            _emailGateWayClientMock
                .Setup(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((true, null));
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestResources.DealerInvitationResponse);
            var actualResult = await _underTest.Invite(TestResources.SaveCustomerVehicleRequestWithoutPhoneNumber,
                TestResources.DealerConfigurationResponse, CommunicationMethod.SmsOrEmail);
            Assert.AreEqual(CommunicationMethod.Email, actualResult);
        }

        [Test]
        public async Task Invite_ExceptionInSms_SendHtmlEmail_Test()
        {
            _smsGatewayClientMock
                .Setup(mock => mock.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((false, new Exception()));
            _emailGateWayClientMock
                .Setup(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((true, null));
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestResources.DealerInvitationResponse);
            var actualResult = await _underTest.Invite(TestResources.SaveCustomerVehicleRequest,
                TestResources.DealerConfigurationResponse, CommunicationMethod.EmailOrSms);
            Assert.AreEqual(CommunicationMethod.Email, actualResult);
        }

        [Test]
        public async Task Invite_ExceptionInEmail_SendSms_Test()
        {
            _smsGatewayClientMock
                .Setup(mock => mock.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((true, null));
            _emailGateWayClientMock
                .Setup(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((false, new Exception()));
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestResources.DealerInvitationResponse);
            var actualResult = await _underTest.Invite(TestResources.SaveCustomerVehicleRequest,
                TestResources.DealerConfigurationResponse, CommunicationMethod.SmsOrEmail);
            Assert.AreEqual(CommunicationMethod.Sms, actualResult);
        }


        [Test]
        public async Task SendEmailWithOtherOption_sendSmsAsSecondOption_CantSendEmail_SendSms()
        {
            _smsGatewayClientMock
                .Setup(mock => mock.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((true, null));
            _emailGateWayClientMock
                .Setup(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((false, null));
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestResources.DealerInvitationResponse);
            var actualResult = await _underTest.SendEmailWithOtherOption(TestResources.SaveCustomerVehicleRequest,
                TestResources.DealerConfigurationResponse, TestResources.DealerInvitationResponse, true);
            Assert.AreEqual(CommunicationMethod.Sms, actualResult);
        }

        [Test]
        public async Task SendSmsWithOtherOption_sendEmailAsSecondOption_CantSendSms_SendEmail()
        {
            _smsGatewayClientMock
                .Setup(mock => mock.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((false, null));
            _emailGateWayClientMock
                .Setup(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((true, null));
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestResources.DealerInvitationResponse);
            var actualResult = await _underTest.SendSmsWithOtherOption(TestResources.SaveCustomerVehicleRequest,
                TestResources.DealerConfigurationResponse, TestResources.DealerInvitationResponse, true);
            Assert.AreEqual(CommunicationMethod.Email, actualResult);
        }

        [TestCase(CommunicationMethod.EmailOrSms)]
        [TestCase(CommunicationMethod.SmsOrEmail)]
        public void Invite_ExceptionIn_EmailOrSms_CommunicationMethod_Test(CommunicationMethod method)
        {
            _smsGatewayClientMock
                .Setup(mock => mock.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((false, new Exception()));
            _dealerConfigurationServiceMock
                .Setup(mock => mock.GetDealerInvitationContent(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestResources.DealerInvitationResponse);
            _emailGateWayClientMock
                .Setup(mock => mock.SendHtmlEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((false, new Exception()));
            Assert.ThrowsAsync<Exception>(() => _underTest.Invite(TestResources.SaveCustomerVehicleRequest,
                TestResources.DealerConfigurationResponse, method));
        }

        [Test]
        public void ValidateLink_Test()
        {
            Assert.IsTrue(_underTest.ValidateLink(TestResources.CustomerVehicle));
        }

        [TestCase("0212101111", "+64 21 210 1111")]
        [TestCase("212101111", "+64 21 210 1111")]
        [TestCase("+64212101111", "+64 21 210 1111")]
        public void FormatInternationalMobileNumberTest(string phoneNumber, string expectedInternationalPhoneNumber)
        {
            var result = _underTest.FormatInternationalMobileNumber(phoneNumber);
            Assert.AreEqual(expectedInternationalPhoneNumber, result);
        }

        [TestCase("abcdedf")]
        [TestCase("")]
        public void FormatInternationalMobileNumber_InvalidNumberTest(string phoneNumber)
        {
            Assert.Throws<NumberParseException>(() => _underTest.FormatInternationalMobileNumber(phoneNumber));
        }

        [Test]
        public void GetInvitationText_Tests()
        {
            const int dealerId = 1;
            const int customerNo = 101;
            const int vehicleNo = 101;
            const string registrationNo = "REGO123";
            string content = $"You car {registrationNoPlaceholder} is due for service, here is the link {seriveBookingUrlPlaceHolder}";
            const string expected = "You car REGO123 is due for service, here is the link https://service.bookingapp.com?d=1&cno=101&vno=101";

            var result = _underTest.GetInvitationText(dealerId, customerNo, vehicleNo, registrationNo, content);
            Assert.AreEqual(expected, result);
        }
    }
}