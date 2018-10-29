using Microsoft.WindowsAzure.Storage.Blob;
using Processor.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Processor.Implementation
{
    public class AzureFileProcessor : IAzureFileProcessor
    {
        private IAzureServices _azureServices;

        public AzureFileProcessor(IAzureServices azureServices)
        {
            _azureServices = azureServices ?? throw new ArgumentNullException(nameof(azureServices));
        }

        public async Task<List<FileContent>> ProcessAzureFilesContent()
        {
            try
            {
                return await _azureServices.GetCloudContent();
            }
            catch (Exception ex)
            {
                throw new ContentException(ex.Message, ex.StackTrace);
            }
        }
    }
}
