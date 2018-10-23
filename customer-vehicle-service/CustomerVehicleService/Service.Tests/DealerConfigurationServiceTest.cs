using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.Response;
using Service.Implementation;
using System;
using System.Threading.Tasks;

namespace Service.Tests
{
    [TestFixture]
    public class DealerConfigurationServiceTest
    {
        private DealerConfigurationService _underTest;
        private Mock<IRestfulClient> _restfullClientMock;

        [SetUp]
        public void Setup()
        {
            _restfullClientMock = new Mock<IRestfulClient>();
            _underTest = new DealerConfigurationService(_restfullClientMock.Object);
        }

        [Test]
        public void DealerConfigurationService_TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DealerConfigurationService(null));
        }

        [Test]
        public async Task GetDealerConfiguration_ByRoofTopAndCommunityId_Test()
        {
            string roofTopId = TestResources.DealerConfigurationResponse.RooftopId;
            string communityId = TestResources.DealerConfigurationResponse.CommunityId;
            var url = $"dealers/?roofTopId={roofTopId}&communityid={communityId}";

            _restfullClientMock.Setup(mock => mock.GetAsync<DealerConfigurationResponse>(url))
                               .Returns(Task.FromResult(TestResources.DealerConfigurationResponse));

            var dealerConfigResponse = await _underTest.GetDealerConfiguration(TestResources.DealerConfigurationResponse.RooftopId,
                                       TestResources.DealerConfigurationResponse.CommunityId);

            Assert.IsNotNull(dealerConfigResponse);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.DealerId, dealerConfigResponse.DealerId);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.DealerName, dealerConfigResponse.DealerName);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.CommunityId, dealerConfigResponse.CommunityId);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.Address, dealerConfigResponse.Address);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.RooftopId, dealerConfigResponse.RooftopId);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.Latitude, dealerConfigResponse.Latitude);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.Longitude, dealerConfigResponse.Longitude);
            Assert.IsTrue(dealerConfigResponse.HasCourtesyCar);
            Assert.IsTrue(dealerConfigResponse.HasDropOff);

        }

        [Test]
        public async Task GetDealerConfiguration_ByRoofTopAndCommunityId_Null_Test()
        {
            var dealerConfigResponse = await _underTest.GetDealerConfiguration(string.Empty, string.Empty);
            Assert.IsNull(dealerConfigResponse);
        }

        [Test]
        public async Task GetDealerConfiramation_ByDealerId_Test()
        {
            var dealerId = TestResources.DealerConfigurationResponse.DealerId;
            var url = $"dealers/{dealerId}";

            _restfullClientMock.Setup(mock => mock.GetAsync<DealerConfigurationResponse>(url))
                               .Returns(Task.FromResult(TestResources.DealerConfigurationResponse));
            var dealerConfigResponse = await _underTest.GetDealerConfiguration(dealerId);
            Assert.IsNotNull(dealerConfigResponse);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.DealerId, dealerConfigResponse.DealerId);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.DealerName, dealerConfigResponse.DealerName);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.CommunityId, dealerConfigResponse.CommunityId);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.Address, dealerConfigResponse.Address);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.RooftopId, dealerConfigResponse.RooftopId);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.Latitude, dealerConfigResponse.Latitude);
            Assert.AreEqual(TestResources.DealerConfigurationResponse.Longitude, dealerConfigResponse.Longitude);
            Assert.IsTrue(dealerConfigResponse.HasCourtesyCar);
            Assert.IsTrue(dealerConfigResponse.HasDropOff);
        }

        [Test]
        public async Task GetDealerConfirmation_ByDealerId_Null_Test()
        {
            var dealerConfigResponse = await _underTest.GetDealerConfiguration(1);
            Assert.IsNull(dealerConfigResponse);
        }

        [Test]
        public async Task GetDealerInvitationContent_Null_Test()
        {
            var dealerConfigResponse = await _underTest.GetDealerInvitationContent(TestResources.DealerConfigurationResponse.RooftopId,
                TestResources.DealerConfigurationResponse.CommunityId);
            Assert.IsNull(dealerConfigResponse);
        }

        [Test]
        public async Task GetDealerInvitationContent_Test()
        {
            var roofTopId = TestResources.DealerConfigurationResponse.RooftopId;
            var communityId = TestResources.DealerConfigurationResponse.CommunityId;

            _restfullClientMock.Setup(mock => mock.GetAsync<DealerInvitationContentResponse>(It.IsAny<string>()))
                .Returns(Task.FromResult(TestResources.DealerInvitationResponse));
            var dealerConfigResponse = await _underTest.GetDealerInvitationContent(roofTopId, communityId);
            Assert.IsNotNull(dealerConfigResponse);
            Assert.AreEqual(TestResources.DealerInvitationResponse.DealerId, dealerConfigResponse.DealerId);
            Assert.AreEqual(TestResources.DealerInvitationResponse.EmailContent, dealerConfigResponse.EmailContent);
            Assert.AreEqual(TestResources.DealerInvitationResponse.EmailSubject, dealerConfigResponse.EmailSubject);
            Assert.AreEqual(TestResources.DealerInvitationResponse.SmsContent, dealerConfigResponse.SmsContent);
        }
    }
}
