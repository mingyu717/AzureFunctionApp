using Moq;
using NUnit.Framework;
using Service.Contract.DbModels;
using Service.Implementation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Service.Tests
{
    [TestFixture]
    public class AppTokenDALTest
    {
        private AppTokenDAL _underTest;
        private Mock<CDKAutolineContext> _cdkAutolineContextMock;

        private readonly IQueryable<AppToken> _appTokens = new List<AppToken>
        {
            TestResources.AppToken
        }.AsQueryable();

        [SetUp]
        public void SetUp()
        {
            var appTokenDbSetMock = new Mock<IDbSet<AppToken>>();
            appTokenDbSetMock.Setup(m => m.Provider).Returns(_appTokens.Provider);
            appTokenDbSetMock.Setup(m => m.Expression).Returns(_appTokens.Expression);
            appTokenDbSetMock.Setup(m => m.ElementType).Returns(_appTokens.ElementType);
            appTokenDbSetMock.Setup(m => m.GetEnumerator()).Returns(_appTokens.GetEnumerator());
            _cdkAutolineContextMock = new Mock<CDKAutolineContext>();
            _cdkAutolineContextMock.Setup(mock => mock.AppTokens).Returns(appTokenDbSetMock.Object);
            _cdkAutolineContextMock.Setup(mock => mock.SaveChangesAsync());
            _underTest = new AppTokenDAL(_cdkAutolineContextMock.Object);
        }

        [Test]
        public void AppTokenDAL_Constructor_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new AppTokenDAL(null));
        }

        [Test]
        public void GetAppToken_NotFound_ReturnNull_Test()
        {
            var result = _underTest.GetAppToken("test");
            Assert.IsNull(result);
        }

        [Test]
        public void GetAppToken_Found_ReturnToken_Test()
        {
            var result = _underTest.GetAppToken(TestResources.AppToken.CommunityId);
            Assert.AreEqual(TestResources.AppToken.Id, result.Id);
            Assert.AreEqual(TestResources.AppToken.CommunityId, result.CommunityId);
            Assert.AreEqual(TestResources.AppToken.Token, result.Token);
        }

        [Test]
        public void SaveAppToken_NotExist_ShouldAddNew()
        {
            var token = Guid.NewGuid();
            var result = _underTest.SaveAppToken(new AppToken
            {
                CommunityId = "Test",
                Token = token
            });
            Assert.IsNotNull(result.Status);
            _cdkAutolineContextMock.Verify(m => m.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void SaveAppToken_AlreadyExist_ShouldAddNew()
        {
            var token = Guid.NewGuid();
            var result = _underTest.SaveAppToken(new AppToken
            {
                CommunityId = TestResources.AppToken.CommunityId,
                Token = token
            });
            Assert.IsNotNull(result.Status);
            _cdkAutolineContextMock.Verify(m => m.SaveChangesAsync(), Times.Once);
        }
    }
}