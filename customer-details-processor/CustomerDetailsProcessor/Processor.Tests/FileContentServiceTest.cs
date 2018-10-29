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
    public class FileContentServiceTest
    {
        private FileContentService _underTest;
        private Mock<IAzureFileProcessor> _azureFileProcessor;
        private Mock<IEmailProcessor> _emailProcessor;

        private List<string> lstDealerCsvSources = new List<string>()
        {
            "email",
            "azureFile"
        };

        private List<FileContent> lstFileContent = new List<FileContent>()
        {
            new FileContent()
            {
                Content = new byte[100],
                FileName = "xyz"
            },
            new FileContent()
            {
                Content = new byte[150],
                FileName = "xyz1"
            }
        };

        [SetUp]
        public void Setup()
        {
            _azureFileProcessor = new Mock<IAzureFileProcessor>();
            _emailProcessor = new Mock<IEmailProcessor>();
            _underTest = new FileContentService(_emailProcessor.Object, _azureFileProcessor.Object);
        }

        [Test]
        public void FileContentService_Constructor_NullException()
        {
            Assert.Throws<ArgumentNullException>(() => new FileContentService(null, _azureFileProcessor.Object));
            Assert.Throws<ArgumentNullException>(() => new FileContentService(_emailProcessor.Object, null));
        }

        [Test]
        public async Task FileContentService_Test()
        {
            _emailProcessor.Setup(mock => mock.ProcessEmailContent()).Returns(Task.FromResult(lstFileContent));
            _azureFileProcessor.Setup(mock => mock.ProcessAzureFilesContent()).Returns(Task.FromResult(lstFileContent));
            var fileContents = await _underTest.GetFileContent(lstDealerCsvSources);
            Assert.IsNotNull(fileContents);
            Assert.AreEqual(4, fileContents.Count);
        }
    }
}
