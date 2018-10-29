using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using NUnit.Framework;
using Processor.Contract;
using Processor.Implementation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Tests
{
    [TestFixture]
    public class AzureFileProcessorTests : TestBase
    {
        private Mock<IAzureServices> _azureServicesMock;
        private AzureFileProcessor _unitTest;

        [SetUp]
        public void Setup()
        {
            _azureServicesMock = new Mock<IAzureServices>();
            _unitTest = new AzureFileProcessor(_azureServicesMock.Object);
        }

        [Test]
        public void AzureBlobProcessor_Constructor_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new AzureFileProcessor(null));
        }

        [TestCase("Processor.Tests.MockCustomerDetails.CustomerMockFile.csv")]
        public async Task ProcessAzureFilesContent_Test(string fileName)
        {
            byte[] contentBytes = GetEmbeddedFile(fileName);
            _azureServicesMock.Setup(mock => mock.GetCloudContent()).Returns(Task.FromResult(GetBlobList(contentBytes)));
            var fileContent = await _unitTest.ProcessAzureFilesContent();
            Assert.AreEqual(1, fileContent.Count);
            Assert.AreEqual("Mock.csv", fileContent[0].FileName);
        }

        [Test]
        public void ProcessAzureFiles_Exception_Test()
        {
            const string exceptionMessage = "This is a new exception";
            _azureServicesMock.Setup(mock=> mock.GetCloudContent()).Throws(new Exception(exceptionMessage));
            var exception = Assert.ThrowsAsync<ContentException>(()=>_unitTest.ProcessAzureFilesContent());
            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        private List<FileContent> GetBlobList(byte[] content)
        {
            return new List<FileContent>
            {
                new FileContent
                {
                    FileName = "Mock.csv",
                    Content = content
                }
            };
        }

    }
}
