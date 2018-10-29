using NUnit.Framework;
using Processor.Implementation;
using System;

namespace Processor.Tests
{
    [TestFixture]
    public class MailServiceTest
    {       
        [Test]
        public void MailService_Constructor_NullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MailService(null));
        }
    }
}
