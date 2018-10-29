using System;
using System.IO;
using System.Reflection;

namespace Processor.Tests
{
    public class TestBase
    {
        /// <summary>
        /// Get file bytes from respected path
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected byte[] GetEmbeddedFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = fileName;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}