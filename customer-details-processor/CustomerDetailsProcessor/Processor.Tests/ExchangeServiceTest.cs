using Moq;
using NUnit.Framework;
using Processor.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Processor.Tests
{
    [TestFixture]
    public class ExchangeServiceTest : TestBase
    {
        private Mock<IMailService> _mailService;

        [SetUp]
        public void Setup()
        {
            _mailService = new Mock<IMailService>();
        }

        /// <summary>
        /// Scenario, Test the Read email item count
        /// </summary>
        /// <returns></returns>
        [TestCase("Processor.Tests.MockCustomerDetails.CustomerMockFile.csv")]
        public void TestReadEmailItem(string fileName)
        {
            byte[] contentBytes = GetEmbeddedFile(fileName);
            _mailService.Setup(x => x.ReadEmails()).Returns(GetList(contentBytes));
            var expectedResult = (GetList(contentBytes).Result.ToList());
            var actualResult = _mailService.Object.ReadEmails().Result.ToList();
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
        }


        private async Task<IEnumerable<EmailItem>> GetList(byte[] bytes)
        {
            return new List<EmailItem>
            {
                new EmailItem()
                {
                    EmailAdditionalContents =
                        new List<EmailAdditionalContent>() {new EmailAdditionalContent() {ContentSource = bytes}}
                }
            }.AsEnumerable();
        }

        [TearDown]
        public void TearDown()
        {
            _mailService = null;
        }
    }
}