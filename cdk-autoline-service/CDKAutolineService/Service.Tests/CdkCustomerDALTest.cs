using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Service.Contract.DbModels;
using Service.Implementation;

namespace Service.Tests
{
    [TestFixture]
    public class CdkCustomerDALTest
    {
        private CdkCustomerDAL _underTest;
        private Mock<CDKAutolineContext> _cdkAutolineContextMock;

        private readonly IQueryable<CdkCustomer> _cdkCustomers = new List<CdkCustomer>
        {
            TestResources.CdkCustomer
        }.AsQueryable();

        [SetUp]
        public void SetUp()
        {
            var cdkCustomerDbSetMock = new Mock<IDbSet<CdkCustomer>>();
            cdkCustomerDbSetMock.Setup(m => m.Provider).Returns(_cdkCustomers.Provider);
            cdkCustomerDbSetMock.Setup(m => m.Expression).Returns(_cdkCustomers.Expression);
            cdkCustomerDbSetMock.Setup(m => m.ElementType).Returns(_cdkCustomers.ElementType);
            cdkCustomerDbSetMock.Setup(m => m.GetEnumerator()).Returns(_cdkCustomers.GetEnumerator());
            _cdkAutolineContextMock = new Mock<CDKAutolineContext>();
            _cdkAutolineContextMock.Setup(mock => mock.CdkCustomers).Returns(cdkCustomerDbSetMock.Object);
            _cdkAutolineContextMock.Setup(mock => mock.SaveChangesAsync()).Returns(Task.FromResult(0));
            _underTest = new CdkCustomerDAL(_cdkAutolineContextMock.Object);
        }

        [Test]
        public void CdkCustomerDAL_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new CdkCustomerDAL(null));
        }

        [Test]
        public void GetCdkCustomer_Null_Parameter_Exception_Test()
        {
            Assert.Throws<ArgumentNullException>(() => _underTest.GetCdkCustomer(null, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => _underTest.GetCdkCustomer("test", -1));
        }

        [Test]
        public void GetCdkCustomer_NotFound_ReturnNull_Test()
        {
            var result = _underTest.GetCdkCustomer("test", 2);
            Assert.IsNull(result);
        }

        [Test]
        public void GetCdkCustomer_Test()
        {
            var result = _underTest.GetCdkCustomer(TestResources.CdkCustomer.CommunityId,
                            TestResources.CdkCustomer.CustomerNo);
            Assert.AreEqual(TestResources.CdkCustomer.CommunityId, result.CommunityId);
            Assert.AreEqual(TestResources.CdkCustomer.CustomerLoginId, result.CustomerLoginId);
            Assert.AreEqual(TestResources.CdkCustomer.CustomerNo, result.CustomerNo);
            Assert.AreEqual(TestResources.CdkCustomer.Id, result.Id);
            Assert.AreEqual(TestResources.CdkCustomer.Password, result.Password);
            Assert.AreEqual(TestResources.CdkCustomer.Token, result.Token);
        }

        [Test]
        public void AddCustomer_NullParameterException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.AddCustomer(null));
            Assert.ThrowsAsync<ArgumentNullException>(() => _underTest.AddCustomer(new CdkCustomer()
            {
                CommunityId = null
            }));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _underTest.AddCustomer(new CdkCustomer()
            {
                CommunityId = "testCommunity",
                CustomerNo = -1
            }));
        }

        [Test]
        public async Task AddCustomer_Test()
        {
            await _underTest.AddCustomer(TestResources.CdkCustomer);
            _cdkAutolineContextMock.Verify(mock=> mock.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateCustomerToken_InvalidParameter_Test()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _underTest.UpdateCustomerToken(-1, new Guid()));
        }

        [Test]
        public void UpdateCustomerToken_NotFound_Test()
        {
            Assert.ThrowsAsync<Exception>(() => _underTest.UpdateCustomerToken(2, new Guid()));
        }

        [Test]
        public async Task UpdateCustomeToken_Test()
        {
            await _underTest.UpdateCustomerToken(10, new Guid());
            _cdkAutolineContextMock.Verify(mock => mock.SaveChangesAsync(), Times.Once);
        }
    }
}
