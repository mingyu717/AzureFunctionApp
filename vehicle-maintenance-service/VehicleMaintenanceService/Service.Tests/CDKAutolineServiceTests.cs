using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NUnit.Framework;
using Service.Contract;
using Service.Contract.Models;
using Service.Implemention;

namespace Service.Tests
{
    [TestFixture]
    public class CDKAutolineServiceTests
    {
        private CDKAutolineService _underTest;
        private Mock<IRestfulClient> _restfulClientMock;
        private IFixture _fixture;
        private GetRecommendedServicesRequest _getRecommendedServicesRequest;
        private DealerConfigurationResponse _dealerConfigurationResponse;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _restfulClientMock = _fixture.Freeze<Mock<IRestfulClient>>();
            _getRecommendedServicesRequest = _fixture.Create<GetRecommendedServicesRequest>();
            _getRecommendedServicesRequest.ModelYear = "2017";
            _dealerConfigurationResponse = _fixture.Create<DealerConfigurationResponse>();
            _underTest = _fixture.Create<CDKAutolineService>();
        }

        [Test]
        public void Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new CDKAutolineService(null, null));
        }

        [Test]
        public void GetRecommendedServices_ArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.GetRecommendedServices(_getRecommendedServicesRequest, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.GetRecommendedServices(null, _dealerConfigurationResponse));
        }

        [Test]
        public async Task GetRecommendedServices_CDKRecommendedServicesResponseIsNull_ShouldReturnNull()
        {
            _restfulClientMock.Setup(m => m.PostAsync<GetCdkRecommendedServiceRequest, CdkRecommendedServicesResponse>("GetRecommendedServices",
                It.Is<GetCdkRecommendedServiceRequest>(r =>
                    r.CommunityId == _dealerConfigurationResponse.CommunityId &&
                    r.RooftopId == _dealerConfigurationResponse.RooftopId &&
                    r.EstOdometer == _getRecommendedServicesRequest.EstOdometer &&
                    r.EstVehicleAgeMonths == "12" &&
                    r.MakeCode == _getRecommendedServicesRequest.MakeCode &&
                    r.ModelCode == _getRecommendedServicesRequest.ModelCode))).Returns(Task.FromResult((CdkRecommendedServicesResponse) null));

            var result = await _underTest.GetRecommendedServices(_getRecommendedServicesRequest, _dealerConfigurationResponse);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetRecommendedServices_CDKRecommendedServicesResponseCdkVehicleServicesIsNull_ShouldReturnNull()
        {
            var threeYearsService = _fixture.Create<CdkVehicleService>();
            threeYearsService.JobDescription = "45km/3yr service";
            var fourYearsService = _fixture.Create<CdkVehicleService>();
            fourYearsService.JobDescription = "60km/4yr service";
            var fiveYearsService = _fixture.Create<CdkVehicleService>();
            fiveYearsService.JobDescription = "75k/5yr service";
            var sixYearsService = _fixture.Create<CdkVehicleService>();
            sixYearsService.JobDescription = "90k/6yr service";
            var genericVwService = _fixture.Create<CdkVehicleService>();
            genericVwService.JobDescription = "Annual Service";
            var airFilterReplacement = _fixture.Create<CdkVehicleService>();
            airFilterReplacement.JobDescription = "Air filter replacement";
            var cdkRecommendedServicesResponse = new CdkRecommendedServicesResponse
            {
                PriceListData = new List<CdkVehicleService>
                {
                    threeYearsService,
                    fourYearsService,
                    fiveYearsService,
                    sixYearsService,
                    genericVwService,
                    airFilterReplacement
                }
            };

            _restfulClientMock.Setup(m => m.PostAsync<GetCdkRecommendedServiceRequest, CdkRecommendedServicesResponse>("GetRecommendedServices",
                It.Is<GetCdkRecommendedServiceRequest>(r =>
                    r.CommunityId == _dealerConfigurationResponse.CommunityId &&
                    r.RooftopId == _dealerConfigurationResponse.RooftopId &&
                    r.EstOdometer == _getRecommendedServicesRequest.EstOdometer &&
                    r.EstVehicleAgeMonths == "12" &&
                    r.MakeCode == _getRecommendedServicesRequest.MakeCode &&
                    r.ModelCode == _getRecommendedServicesRequest.ModelCode))).
                Returns(Task.FromResult(cdkRecommendedServicesResponse));

            var result = await _underTest.GetRecommendedServices(_getRecommendedServicesRequest, _dealerConfigurationResponse);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.DistanceBasedServices.Count);
            Assert.AreEqual(threeYearsService.JobExtDescription, result.DistanceBasedServices[0].Description);
            Assert.AreEqual(threeYearsService.JobDescription, result.DistanceBasedServices[0].Name);
            Assert.AreEqual(threeYearsService.JobCode, result.DistanceBasedServices[0].ServiceCode);
            Assert.AreEqual(threeYearsService.ProductCode, result.DistanceBasedServices[0].ProductCode);
            Assert.AreEqual(threeYearsService.JobTime, result.DistanceBasedServices[0].ServiceTime);
            Assert.AreEqual(threeYearsService.JobPrice, result.DistanceBasedServices[0].Price);
            Assert.AreEqual(1, result.AdditionalServices.Count);
            Assert.AreEqual(airFilterReplacement.JobExtDescription, result.AdditionalServices[0].Description);
            Assert.AreEqual(airFilterReplacement.JobDescription, result.AdditionalServices[0].Name);
            Assert.AreEqual(airFilterReplacement.ProductCode, result.AdditionalServices[0].ProductCode);
            Assert.AreEqual(airFilterReplacement.JobCode, result.AdditionalServices[0].ServiceCode);
            Assert.AreEqual(airFilterReplacement.JobTime, result.AdditionalServices[0].ServiceTime);
            Assert.AreEqual(airFilterReplacement.JobPrice, result.AdditionalServices[0].Price);
        }

        [Test]
        public async Task GetRecommendedServices_CDKRecommendedServicesResponse_Test()
        {
            _restfulClientMock.Setup(m => m.PostAsync<GetCdkRecommendedServiceRequest, CdkRecommendedServicesResponse>("GetRecommendedServices",
                    It.Is<GetCdkRecommendedServiceRequest>(r =>
                        r.CommunityId == _dealerConfigurationResponse.CommunityId &&
                        r.RooftopId == _dealerConfigurationResponse.RooftopId &&
                        r.EstOdometer == _getRecommendedServicesRequest.EstOdometer &&
                        r.EstVehicleAgeMonths == "12" &&
                        r.MakeCode == _getRecommendedServicesRequest.MakeCode &&
                        r.ModelCode == _getRecommendedServicesRequest.ModelCode))).
                Returns(Task.FromResult(new CdkRecommendedServicesResponse { PriceListData = null }));
            var result = await _underTest.GetRecommendedServices(_getRecommendedServicesRequest, _dealerConfigurationResponse);

            Assert.IsNull(result);
        }

        [TestCase("2017", ExpectedResult = "12")]
        [TestCase("2018", ExpectedResult = "6")]
        [TestCase("203", ExpectedResult = "")]
        [TestCase("", ExpectedResult = "")]
        [TestCase(null, ExpectedResult = "")]
        [TestCase("InvalidModelYear", ExpectedResult = "")]
        public string GetEstVehicleAgeMonths_Tests(string modelYearString)
        {
            return _underTest.GetEstVehicleAgeMonths(modelYearString);
        }

        [TestCase("Annual Service", ExpectedResult = true, Description = "Annual service description test1")]
        [TestCase("Annual VW Service", ExpectedResult = true, Description = "Annual service description test2")]
        [TestCase("Annual 1 Service", ExpectedResult = true, Description = "Annual service description test3")]
        [TestCase("Annual 1B Service", ExpectedResult = true, Description = "Annual service description test4")]
        [TestCase("Annual 1wew% Service", ExpectedResult = false, Description = "Annual service description test5")]
        [TestCase("Annual % Service", ExpectedResult = false, Description = "Annual service description test6")]
        [TestCase("AnnualService", ExpectedResult = false, Description = "Annual service description test7")]
        [TestCase("Annual 1Service", ExpectedResult = false, Description = "Annual service description test8")]
        [TestCase("AnnualVService", ExpectedResult = false, Description = "Annual service description test9")]
        [TestCase("AnnualV Service", ExpectedResult = false, Description = "Annual service description test10")]
        [TestCase("Generic Service", ExpectedResult = true, Description = "Annual service description test11")]
        [TestCase("Generic VW Service", ExpectedResult = true, Description = "Annual service description test12")]
        [TestCase("Generic 1 Service", ExpectedResult = true, Description = "Annual service description test13")]
        [TestCase("Generic 1B Service", ExpectedResult = true, Description = "Annual service description test14")]
        [TestCase("Generic 1wew% Service", ExpectedResult = false, Description = "Annual service description test15")]
        [TestCase("Generic % Service", ExpectedResult = false, Description = "Annual service description test16")]
        [TestCase("GenericService", ExpectedResult = false, Description = "Annual service description test17")]
        [TestCase("Generic 1Service", ExpectedResult = false, Description = "Annual service description test18")]
        [TestCase("GenericVService", ExpectedResult = false, Description = "Annual service description test19")]
        [TestCase("GenericV Service", ExpectedResult = false, Description = "Annual service description test20")]
        [TestCase("60km/4yr service", ExpectedResult = true, Description = "Distance based service description test")]
        [TestCase("Fuel filter replacement", ExpectedResult = false, Description = "Additional service description test")]
        [TestCase("40k/2yr service", ExpectedResult = true, Description = "Distance based service description test")]
        [TestCase(null, ExpectedResult = false, Description = "Null test")]
        [TestCase("", ExpectedResult = false, Description = "Empty string test")]
        public bool IsDistanceBasedServices(string jobDescription)
        {
            return _underTest.IsDistanceBasedServices(jobDescription);
        }
    }
}
