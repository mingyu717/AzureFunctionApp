using Moq;
using NUnit.Framework;
using Processor.Contract;
using Processor.Contract.Exceptions;
using Processor.Implementation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Tests
{
    [TestFixture]
    public class DealerConfigurationClientTest
    {
        private DealerConfigurationClient _underTest;
        private Mock<IRestfulClient> _restfullClientMock;

        private DealerConfigurationResponse _dealerConfigurationResponse => new DealerConfigurationResponse()
        {
            DealerId = 1,
            DealerName = "abc",
            RooftopId = "EBBVW11Dev",
            CommunityId = "EBBETDev",
            Address = "xyz",
            CommunicationMethod = 0,
            PhoneNumber = "0123456789",
            Latitude = 28.459497,
            Longitude = 77.026638,
            AppThemeName = "SampleTheme1",
            CsvSource = 0,
            EmailAddress = "test@email.com",
            MinimumFreeCapacity = 10,
            ShowAdvisors = true,
            ShowPrice = true,
            ShowTransportations = true
        };

        private readonly List<string> _lstDealerCsvSources = new List<string>()
        {
           "email",
            "azureFile"
        };

        [SetUp]
        public void Setup()
        {
            _restfullClientMock = new Mock<IRestfulClient>();
            _underTest = new DealerConfigurationClient(_restfullClientMock.Object);
        }

        [Test]
        public void DealerConfigurationService_TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DealerConfigurationClient(null));
        }

        [Test]
        public async Task GetDealerConfiguration_ByRoofTopAndCommunityId_Test()
        {
            string roofTopId = _dealerConfigurationResponse.RooftopId;
            string communityId = _dealerConfigurationResponse.CommunityId;
            var url = $"dealers/?roofTopId={roofTopId}&communityid={communityId}";

            _restfullClientMock.Setup(mock => mock.GetAsync<DealerConfigurationResponse>(url))
                               .Returns(Task.FromResult(_dealerConfigurationResponse));

            var dealerConfigResponse = await _underTest.GetDealerConfiguration(_dealerConfigurationResponse.RooftopId,
                                       _dealerConfigurationResponse.CommunityId);

            Assert.IsNotNull(dealerConfigResponse);
            Assert.AreEqual(_dealerConfigurationResponse.DealerId, dealerConfigResponse.DealerId);
            Assert.AreEqual(_dealerConfigurationResponse.DealerName, dealerConfigResponse.DealerName);
            Assert.AreEqual(_dealerConfigurationResponse.CommunityId, dealerConfigResponse.CommunityId);
            Assert.AreEqual(_dealerConfigurationResponse.Address, dealerConfigResponse.Address);
            Assert.AreEqual(_dealerConfigurationResponse.RooftopId, dealerConfigResponse.RooftopId);
            Assert.AreEqual(_dealerConfigurationResponse.Latitude, dealerConfigResponse.Latitude);
            Assert.AreEqual(_dealerConfigurationResponse.Longitude, dealerConfigResponse.Longitude);
            Assert.AreEqual(_dealerConfigurationResponse.CsvSource, dealerConfigResponse.CsvSource);
            Assert.AreEqual(_dealerConfigurationResponse.EmailAddress, dealerConfigResponse.EmailAddress);
            Assert.AreEqual(_dealerConfigurationResponse.MinimumFreeCapacity, dealerConfigResponse.MinimumFreeCapacity);
            Assert.IsTrue(dealerConfigResponse.ShowAdvisors);
            Assert.IsTrue(dealerConfigResponse.ShowPrice);
            Assert.IsTrue(dealerConfigResponse.ShowTransportations);
        }

        [Test]
        public async Task GetDealerConfiguration_ByRoofTopAndCommunityId_Null_Test()
        {
            var dealerConfigResponse = await _underTest.GetDealerConfiguration(string.Empty, string.Empty);
            Assert.IsNull(dealerConfigResponse);
        }

        [Test]
        public async Task GetDealersSources_Test()
        {
            var url = "dealers/csvsources";
            _restfullClientMock.Setup(mock => mock.GetAsync<List<string>>(url))
                .Returns(Task.FromResult(_lstDealerCsvSources));
            var dealerConfigResponse = await _underTest.GetDealerCsvSources();
            Assert.AreEqual(2, dealerConfigResponse.Count);
        }

        [TestCase(1, "", "test@email.com")]
        [TestCase(2, "0123456789", "")]
        [TestCase(3, "", "")]
        [TestCase(4, "", "")]
        public void CheckPhoneNumberPriorityByCommunicationMethod_PhoneNoNullException_Test(int method, string phoneNumber, string email)
        {
            string roofTopId = _dealerConfigurationResponse.RooftopId;
            string communityId = _dealerConfigurationResponse.CommunityId;
            var url = $"dealers/?roofTopId={roofTopId}&communityid={communityId}";

            _restfullClientMock.Setup(mock => mock.GetAsync<DealerConfigurationResponse>(url))
                .Returns(Task.FromResult(It.IsAny<DealerConfigurationResponse>()));

            Assert.Throws<PhoneOrEmailNullException>(() =>
                _underTest.ValidateEmailOrSmsByCommunicationMethod(new DealerConfigurationResponse()
                {
                    CommunicationMethod = method
                }, phoneNumber, email));
        }

        [TestCase(0, "", "", ExpectedResult = true)]
        [TestCase(1, "0123456789", "test@email.com", ExpectedResult = true)]
        [TestCase(2, "", "test@email.com", ExpectedResult = true)]
        [TestCase(3, "0123456789", "test@email.com", ExpectedResult = true)]
        [TestCase(5, "0123456789", "test@email.com", ExpectedResult = true)]
        public bool CheckPhoneNumberPriorityByCommunicationMethod_Valid_Test(int method, string phoneno, string email)
        {
            string roofTopId = _dealerConfigurationResponse.RooftopId;
            string communityId = _dealerConfigurationResponse.CommunityId;
            var url = $"dealers/?roofTopId={roofTopId}&communityid={communityId}";

            _restfullClientMock.Setup(mock => mock.GetAsync<DealerConfigurationResponse>(url))
                .Returns(Task.FromResult(_dealerConfigurationResponse));

            return _underTest.ValidateEmailOrSmsByCommunicationMethod(new DealerConfigurationResponse()
            {
                CommunicationMethod = method
            }, phoneno, email);
        }
    }
}

