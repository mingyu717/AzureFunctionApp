using Service.Contract.DbModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Service.Implementation;
using NUnit.Framework;

namespace Service.Tests
{
    [TestFixture]
    public class DealerCDKConfigurationDALTest
    {
        private DealerCDKConfigurationsDAL _underTest;
        private Mock<CDKAutolineContext> _cdkAutolineContextMock;

        private readonly IQueryable<DealerCDKConfiguration> _dealerCdkConfigurations = new List<DealerCDKConfiguration>
        {
            TestResources.DealerCDKConfiguration
        }.AsQueryable();


        [SetUp]
        public void SetUp()
        {
            var dealerCDKConfigurationDbSetMock = new Mock<IDbSet<DealerCDKConfiguration>>();
            dealerCDKConfigurationDbSetMock.Setup(m => m.Provider).Returns(_dealerCdkConfigurations.Provider);
            dealerCDKConfigurationDbSetMock.Setup(m => m.Expression).Returns(_dealerCdkConfigurations.Expression);
            dealerCDKConfigurationDbSetMock.Setup(m => m.ElementType).Returns(_dealerCdkConfigurations.ElementType);
            dealerCDKConfigurationDbSetMock.Setup(m => m.GetEnumerator()).Returns(_dealerCdkConfigurations.GetEnumerator());
            _cdkAutolineContextMock = new Mock<CDKAutolineContext>();
            _cdkAutolineContextMock.Setup(mock => mock.DealerCDKConfigurations).Returns(dealerCDKConfigurationDbSetMock.Object);
            _underTest = new DealerCDKConfigurationsDAL(_cdkAutolineContextMock.Object);
        }

        [Test]
        public void DealerCDKConfigurationsDAL_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new DealerCDKConfigurationsDAL(null));
        }

        [Test]
        public void GetDealerCDKConfiguration_NotFound_ReturnNull_Test()
        {
            var result = _underTest.GetDealerCDKConfigurations("test", "test");
            Assert.IsNull(result);
        }

        [Test]
        public void GetDealerCDKConfiguration_Found_ReturnConfiguration_Test()
        {
            var result = _underTest.GetDealerCDKConfigurations(TestResources.DealerCDKConfiguration.RoofTopId, TestResources.DealerCDKConfiguration.CommunityId);
            Assert.AreEqual(TestResources.DealerCDKConfiguration.CommunityId, result.CommunityId);
            Assert.AreEqual(TestResources.DealerCDKConfiguration.PartnerKey, result.PartnerKey);
            Assert.AreEqual(TestResources.DealerCDKConfiguration.PartnerId, result.PartnerId);
            Assert.AreEqual(TestResources.DealerCDKConfiguration.PartnerVersion, result.PartnerVersion);
        }
    }
}
