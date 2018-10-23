using AutoMapper;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.Exceptions;
using Service.Contract.Request;
using Service.Implementation;
using System;
using System.Threading.Tasks;
using Service.Contract.Response;
using System.Collections.Generic;

namespace Service.Test
{
    [TestFixture]
    public class DealerConfigurationServiceTest
    {
        private DealerConfigurationService _dealerConfigurationService;
        private Mock<IDealerConfigurationDAL> _dealerConfigurationDal;
        private IMapper _mapper;

        private readonly DealerConfigurationCreateRequest _dealerConfigurationCreateRequest =
            new DealerConfigurationCreateRequest
            {
                DealerName = "xyz",
                RooftopId = "EBBVW11DEV1",
                CommunityId = "EBBETDEV1",
                Address = "Gurgaon",
                PhoneNumber = "1234567890",
                EmailAddress = "test@email.com",
                Latitude = 28.459497,
                Longitude = 77.026638,
                AppThemeName = "SampleTheme1",
                ShowTransportations = true,
                ShowAdvisors = true,
                ShowPrice = true,
                CommunicationMethodName = "SMS",
                SmsContent = "Test sms content",
                EmailContent = "Test email content",
                EmailSubject = "Test email subject",
                MinimumFreeCapacity = 20,
                CsvSourceName = "azurefile"
            };

        private readonly DealerConfigurationUpdateRequest _dealerConfigurationUpdateRequest =
            new DealerConfigurationUpdateRequest
            {
                DealerName = "xyz",
                RooftopId = "EBBVW11Pro",
                CommunityId = "EBBETPro",
                Address = "Gurgaon",
                PhoneNumber = "1234567890",
                EmailAddress = "test@email.com",
                Latitude = 28.459497,
                Longitude = 77.026638,
                AppThemeName = "SampleTheme1",
                ShowTransportations = true,
                ShowAdvisors = true,
                ShowPrice = true,
                CommunicationMethodName = "SMS",
                SmsContent = "Test sms content",
                EmailContent = "Test email content",
                EmailSubject = "Test email subject",
                CsvSourceName = "email",
                MinimumFreeCapacity = 20
            };

        private readonly List<int> _lstDealerCsvSources = new List<int>()
        {
            0,
            1
        };

        private readonly DealerConfiguration _dealerConfiguration = new DealerConfiguration()
        {
            DealerId = 1,
            DealerName = "xyz",
            RooftopId = "EBBVW11DEV1",
            CommunityId = "EBBETDEV1",
            Address = "Gurgaon",
            PhoneNumber = "1234567890",
            Latitude = 28.459497,
            Longitude = 77.026638,
            AppThemeName = "SampleTheme1",
            EmailAddress = "test@email.com",
            ShowTransportations = true,
            ShowAdvisors = true,
            ShowPrice = true,
            CommunicationMethod = 1,
            SmsContent = "Test sms content",
            EmailContent = "Test email content",
            EmailSubject = "Test email subject",
            CsvSource = 1,
            MinimumFreeCapacity = 20
        };

        private readonly DealerInvitationContentResponse _dealerInvitationContentResponse = new DealerInvitationContentResponse()
        {
            DealerId = 1,
            SmsContent = "Test sms content",
            EmailContent = "Test email content",
            EmailSubject = "Test email subject"
        };

        [SetUp]
        public void Setup()
        {
            _dealerConfigurationDal = new Mock<IDealerConfigurationDAL>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DealerConfigurationCreateRequest, DealerConfiguration>();
                cfg.CreateMap<DealerConfigurationUpdateRequest, DealerConfiguration>();
                cfg.CreateMap<DealerConfiguration, DealerConfigurationResponse>();
                cfg.CreateMap<DealerConfiguration, DealerInvitationContentResponse>();
            });

            _mapper = config.CreateMapper();
            _dealerConfigurationService = new DealerConfigurationService(_dealerConfigurationDal.Object, _mapper);
        }

        /// <summary>
        /// Test scenario, to check if constructor is null
        /// </summary>
        [Test]
        public void DealerConfigurationService_TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new DealerConfigurationService(null, _mapper));
            Assert.Throws<ArgumentNullException>(() =>
                new DealerConfigurationService(_dealerConfigurationDal.Object, null));
        }

        /// <summary>
        /// Test scenario, to check null value.
        /// </summary>
        [Test]
        public void AddDealerConfiguration_Null_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _dealerConfigurationService.AddDealerConfiguration(null));
        }

        /// <summary>
        /// Test scenario, when give dealercofiguration request object then expected result should be success.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AddDealerConfiguration_Test()
        {
            await _dealerConfigurationService.AddDealerConfiguration(_dealerConfigurationCreateRequest);
            _dealerConfigurationDal.Verify(mock => mock.AddDealerConfiguration(It.IsAny<DealerConfiguration>()),
                Times.Once);
        }

        [Test]
        public void AddDealerConfiguration_CommunicationMethodException_Test()
        {
            Assert.ThrowsAsync<CommunicationMethodException>(() => _dealerConfigurationService.AddDealerConfiguration(
                new DealerConfigurationCreateRequest()
                {
                    CommunicationMethodName = "noSMS"
                }));
        }

        /// <summary>
        /// Test scenario, when delete dealerconfiguration by dealer id 1 then expected will be success.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeleteDealerConfiguration_Test()
        {
            await _dealerConfigurationService.DeleteDealerConfiguration(1);
            _dealerConfigurationDal.Verify(mock => mock.DeleteDealerConfiguration(1), Times.Once);
        }

        /// <summary>
        /// Test scenario, when dealerconfiguration request is null then expexted result will be argument null excepiton
        /// </summary>
        [Test]
        public void EditDealerConfiguration_Null_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _dealerConfigurationService.EditDealerConfiguration(null, 1));
        }

        [Test]
        public void EditDealerConfiguration_InvalidCommunicationMethodException_Test()
        {
            Assert.ThrowsAsync<CommunicationMethodException>(() => _dealerConfigurationService.EditDealerConfiguration(
                new DealerConfigurationUpdateRequest()
                {
                    CommunicationMethodName = "noSms"
                }, 1));
        }

        /// <summary>
        /// Test scenario  during edit dealer configuration by dealer id,  then expected result should be success.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task EditDealerConfiguration_ValidRequest_Test()
        {
            _dealerConfigurationDal.Setup(m => m.GetDealerConfigurationById(1)).Returns(_dealerConfiguration);

            _dealerConfigurationDal
                .Setup(m => m.GetDealerConfigurationByRoofTopIdAndCommunityId(
                    _dealerConfigurationUpdateRequest.RooftopId, _dealerConfigurationUpdateRequest.CommunityId))
                .Returns(_dealerConfiguration);

            await _dealerConfigurationService.EditDealerConfiguration(_dealerConfigurationUpdateRequest, 1);
            _dealerConfigurationDal.Verify(mock => mock.EditDealerConfiguration(It.IsAny<DealerConfiguration>(), 1),
                Times.Once);
        }

        /// <summary>
        /// Test scenario, given to edit dealer detail when passsing dealerid
        /// then that dealer id is not present it will return not found response.
        /// </summary>
        [Test]
        public void EditDealer_NotFoundById()
        {
            _dealerConfigurationDal.Setup(m => m.GetDealerConfigurationById(11)).Returns((DealerConfiguration)null);
            Assert.ThrowsAsync<DealerNotExistException>(() =>
                _dealerConfigurationService.EditDealerConfiguration(_dealerConfigurationUpdateRequest, 11));
        }

        /// <summary>
        /// Test scenario, given to edit dealer detail when passing communityid and rooftopid
        /// then that community id and rooftopid is used in other dealer throw exception
        /// dealer already exist.
        /// </summary>
        [Test]
        public void EditDealer_CommunityIdAndRoofTopIdAlreadyExist_AndDifferentId()
        {
            _dealerConfigurationDal.Setup(m => m.GetDealerConfigurationById(11)).Returns(_dealerConfiguration);

            _dealerConfigurationDal
                .Setup(m => m.GetDealerConfigurationByRoofTopIdAndCommunityId(
                    _dealerConfigurationUpdateRequest.RooftopId, _dealerConfigurationUpdateRequest.CommunityId))
                .Returns(_dealerConfiguration);
            Assert.ThrowsAsync<DealerAlreadyExistException>(() =>
                _dealerConfigurationService.EditDealerConfiguration(_dealerConfigurationUpdateRequest, 11));
        }

        /// <summary>
        /// Test scenario when existing dealerid is passed, then it will return the dealer configuration.
        /// </summary>
        [Test]
        public void GetDealerConfigById_Test()
        {
            _dealerConfigurationDal.Setup(mock => mock.GetDealerConfigurationById(1))
                .Returns(_dealerConfiguration);
            var dealerConfigurationResponse = _dealerConfigurationService.GetDealerConfigurationById(1);
            Assert.AreEqual(_dealerConfigurationCreateRequest.Address, dealerConfigurationResponse.Address);
            Assert.AreEqual(_dealerConfigurationCreateRequest.AppThemeName, dealerConfigurationResponse.AppThemeName);
            Assert.AreEqual(_dealerConfigurationCreateRequest.CommunityId, dealerConfigurationResponse.CommunityId);
            Assert.AreEqual(_dealerConfigurationCreateRequest.DealerName, dealerConfigurationResponse.DealerName);
            Assert.AreEqual(_dealerConfigurationCreateRequest.Latitude, dealerConfigurationResponse.Latitude);
            Assert.AreEqual(_dealerConfigurationCreateRequest.Longitude, dealerConfigurationResponse.Longitude);
            Assert.AreEqual(_dealerConfigurationCreateRequest.ShowAdvisors, dealerConfigurationResponse.ShowAdvisors);
            Assert.AreEqual(_dealerConfigurationCreateRequest.ShowPrice, dealerConfigurationResponse.ShowPrice);
            Assert.AreEqual(_dealerConfigurationCreateRequest.ShowTransportations, dealerConfigurationResponse.ShowTransportations);
            Assert.AreEqual(_dealerConfigurationCreateRequest.PhoneNumber, dealerConfigurationResponse.PhoneNumber);
            Assert.AreEqual(_dealerConfigurationCreateRequest.EmailAddress, dealerConfigurationResponse.EmailAddress);
            Assert.AreEqual(_dealerConfigurationCreateRequest.RooftopId, dealerConfigurationResponse.RooftopId);
            Assert.AreEqual(_dealerConfigurationCreateRequest.RooftopId, dealerConfigurationResponse.RooftopId);
            Assert.AreEqual((int)(CommunicationMethod)Enum.Parse(typeof(CommunicationMethod),
                _dealerConfigurationCreateRequest.CommunicationMethodName, true), dealerConfigurationResponse.CommunicationMethod);
        }

        /// <summary>
        /// Test scenario, when dealer id is not present then it will return the null.
        /// </summary>
        [Test]
        public void DealerId_NotPresent_Test()
        {
            _dealerConfigurationDal.Setup(mock => mock.GetDealerConfigurationById(1))
                .Returns(_mapper.Map<DealerConfiguration>(null));
            var dealerConfiguration = _dealerConfigurationService.GetDealerConfigurationById(1);
            Assert.IsNull(dealerConfiguration);
        }

        /// <summary>
        /// Test scenario, when dealer rooftopId and community id exist in system then expected dealer configuration return.
        /// </summary>
        [Test]
        public void GetDealerConfigByRoofTopIdAndCommunityId()
        {
            _dealerConfigurationDal.Setup(mock =>
                    mock.GetDealerConfigurationByRoofTopIdAndCommunityId(_dealerConfigurationCreateRequest.RooftopId,
                        _dealerConfigurationCreateRequest.CommunityId))
                .Returns(_dealerConfiguration);
            var dealerConfigurationResponse = _dealerConfigurationService.GetDealerConfigurationByRoofTopIdAndCommunityId(
                _dealerConfigurationCreateRequest.RooftopId, _dealerConfigurationCreateRequest.CommunityId);
            Assert.AreEqual(_dealerConfigurationCreateRequest.Address, dealerConfigurationResponse.Address);
            Assert.AreEqual(_dealerConfigurationCreateRequest.AppThemeName, dealerConfigurationResponse.AppThemeName);
            Assert.AreEqual(_dealerConfigurationCreateRequest.CommunityId, dealerConfigurationResponse.CommunityId);
            Assert.AreEqual(_dealerConfigurationCreateRequest.DealerName, dealerConfigurationResponse.DealerName);
            Assert.AreEqual(_dealerConfigurationCreateRequest.Latitude, dealerConfigurationResponse.Latitude);
            Assert.AreEqual(_dealerConfigurationCreateRequest.Longitude, dealerConfigurationResponse.Longitude);
            Assert.AreEqual(_dealerConfigurationCreateRequest.ShowAdvisors, dealerConfigurationResponse.ShowAdvisors);
            Assert.AreEqual(_dealerConfigurationCreateRequest.ShowPrice, dealerConfigurationResponse.ShowPrice);
            Assert.AreEqual(_dealerConfigurationCreateRequest.ShowTransportations, dealerConfigurationResponse.ShowTransportations);
            Assert.AreEqual(_dealerConfigurationCreateRequest.PhoneNumber, dealerConfigurationResponse.PhoneNumber);
            Assert.AreEqual(_dealerConfigurationCreateRequest.EmailAddress, dealerConfigurationResponse.EmailAddress);
            Assert.AreEqual(_dealerConfigurationCreateRequest.RooftopId, dealerConfigurationResponse.RooftopId);
            Assert.AreEqual(_dealerConfiguration.MinimumFreeCapacity, dealerConfigurationResponse.MinimumFreeCapacity);
            Assert.AreEqual((int)(CommunicationMethod)Enum.Parse(typeof(CommunicationMethod),
                _dealerConfigurationCreateRequest.CommunicationMethodName, true), dealerConfigurationResponse.CommunicationMethod);
        }

        /// <summary>
        /// Test scenario, when dealer rooftopId and community id exist in system then expected dealer configuration return.
        /// </summary>
        [Test]
        public void RoofTopId_OR_CommunityId_Not_ExistInSystem_Test()
        {
            _dealerConfigurationDal.Setup(mock =>
                    mock.GetDealerConfigurationByRoofTopIdAndCommunityId(_dealerConfigurationCreateRequest.RooftopId,
                        _dealerConfigurationCreateRequest.CommunityId))
                .Returns(_mapper.Map<DealerConfiguration>(null));
            var dealerConfiguration = _dealerConfigurationService.GetDealerConfigurationByRoofTopIdAndCommunityId(
                _dealerConfigurationCreateRequest.RooftopId, _dealerConfigurationCreateRequest.CommunityId);
            Assert.IsNull(dealerConfiguration);
        }

        /// <summary>
        /// Test scenario when send expected dealerid, rooftopid and communityid then return should be expected true result.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="dealerId"></param>
        /// <param name="roofTopId"></param>
        /// <param name="communityId"></param>
        [TestCase(DealerSearchCriteria.SearchByRooftopAndCommunityId, default(int), "EBBVW11DEV1", "EBBETDEV1",
            ExpectedResult = true)]
        [TestCase(DealerSearchCriteria.SearchByDealerId, 1, "", "", ExpectedResult = true)]
        [TestCase(DealerSearchCriteria.SearchByAll, 1, "EBBVW11DEV1", "EBBETDEV1", ExpectedResult = true)]
        public bool DealerExists_Test(DealerSearchCriteria searchCriteria, int dealerId, string roofTopId,
            string communityId)
        {
            _dealerConfigurationDal.Setup(mock => mock.GetDealerConfigurationById(dealerId))
                .Returns(_dealerConfiguration);
            _dealerConfigurationDal.Setup(mock =>
                    mock.GetDealerConfigurationByRoofTopIdAndCommunityId(roofTopId, communityId))
                .Returns(_dealerConfiguration);
            return _dealerConfigurationService.CheckDealerExist(searchCriteria, dealerId, roofTopId, communityId);
        }

        /// <summary>
        /// Test scenario when send expected dealerid, rooftopid and communityid then return should be expected false result.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="dealerId"></param>
        /// <param name="roofTopId"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        [TestCase(DealerSearchCriteria.SearchByAll, default(int), "", "", ExpectedResult = false)]
        [TestCase(DealerSearchCriteria.SearchByDealerId, default(int), "", "", ExpectedResult = false)]
        [TestCase(DealerSearchCriteria.SearchByRooftopAndCommunityId, default(int), "", "", ExpectedResult = false)]
        [TestCase(DealerSearchCriteria.SearchByRooftopAndCommunityId, default(int), "EBBVW11DEV1", "EBBETDEV1",
            ExpectedResult = false)]
        [TestCase(DealerSearchCriteria.SearchByDealerId, 1, "", "", ExpectedResult = false)]
        [TestCase(DealerSearchCriteria.SearchByAll, 1, "EBBVW11DEV1", "EBBETDEV1", ExpectedResult = false)]
        [TestCase(4, default(int), "", "", ExpectedResult = false)]
        public bool DealerNotExists_Test(DealerSearchCriteria searchCriteria, int dealerId, string roofTopId,
            string communityId)
        {
            _dealerConfigurationDal.Setup(mock => mock.GetDealerConfigurationById(dealerId))
                .Returns(_mapper.Map<DealerConfiguration>(null));
            _dealerConfigurationDal.Setup(mock =>
                    mock.GetDealerConfigurationByRoofTopIdAndCommunityId(roofTopId, communityId))
                .Returns(_mapper.Map<DealerConfiguration>(null));
            return _dealerConfigurationService.CheckDealerExist(searchCriteria, dealerId, roofTopId, communityId);
        }

        /// <summary>
        /// Test scenario, given to validate communication method
        /// when giving communication name
        /// then that communication name should be valid communication name.
        /// </summary>
        /// <param name="communicationMethodName"></param>
        /// <returns></returns>
        [TestCase("NonSMS", ExpectedResult = false)]
        [TestCase("SMS", ExpectedResult = true)]
        public bool ValidateCommunitcationMethod_Test(string communicationMethodName)
        {
            return _dealerConfigurationService.CheckCommunicationMethod(communicationMethodName);
        }

        [Test]
        public void GetInvitationContent_Test()
        {
            _dealerConfigurationDal.Setup(mock => mock.GetDealerConfigurationByRoofTopIdAndCommunityId(
                _dealerConfigurationCreateRequest.RooftopId,
                _dealerConfigurationCreateRequest.CommunityId)).Returns(_dealerConfiguration);
            var dealerInvitationContent = _dealerConfigurationService.GetInvitationContent(
                _dealerConfiguration.RooftopId,
                _dealerConfiguration.CommunityId);
            Assert.AreEqual(_dealerInvitationContentResponse.DealerId, dealerInvitationContent.DealerId);
            Assert.AreEqual(_dealerInvitationContentResponse.EmailContent, dealerInvitationContent.EmailContent);
            Assert.AreEqual(_dealerInvitationContentResponse.EmailSubject, dealerInvitationContent.EmailSubject);
            Assert.AreEqual(_dealerInvitationContentResponse.SmsContent, dealerInvitationContent.SmsContent);
        }

        [Test]
        public void GetInvitationContent_Null_Test()
        {
            _dealerConfigurationDal.Setup(mock => mock.GetDealerConfigurationByRoofTopIdAndCommunityId(
                _dealerConfigurationCreateRequest.RooftopId,
                _dealerConfigurationCreateRequest.CommunityId)).Returns(_mapper.Map<DealerConfiguration>(null));
            var dealerInvitationContent = _dealerConfigurationService.GetInvitationContent(
                _dealerConfiguration.RooftopId,
                _dealerConfiguration.CommunityId);
            Assert.IsNull(dealerInvitationContent);
        }

        [Test]
        public void GetDealersCsvSources_Test()
        {
            _dealerConfigurationDal.Setup(mock => mock.GetDealersCsvSources()).Returns(_lstDealerCsvSources);
            var dealerConfigurationResponse = _dealerConfigurationService.GetDealersCsvSources();
            Assert.AreEqual(2, dealerConfigurationResponse.Count);

        }

        [Test]
        public void GetDealersCsvSources_Null_Test()
        {
            _dealerConfigurationDal.Setup(mock => mock.GetDealersCsvSources()).Returns((List<int>)null);
            var dealerConfigurationResponse = _dealerConfigurationService.GetDealersCsvSources();
            Assert.AreEqual(0, dealerConfigurationResponse.Count);
        }

        [TearDown]
        public void TearDown()
        {
            _dealerConfigurationService = null;
            _dealerConfigurationDal = null;
            _mapper = null;
        }
    }
}