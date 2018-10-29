using Processor.Contract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Processor.Implementation
{
    public class FileContentService : IFileContentService
    {
        private readonly IEmailProcessor _emailProcessor;
        private readonly IAzureFileProcessor _azureFileProcessor;
        public FileContentService(IEmailProcessor emailProcessor, IAzureFileProcessor azureFileProcessor)
        {
            _emailProcessor = emailProcessor ?? throw new ArgumentNullException(nameof(emailProcessor));
            _azureFileProcessor = azureFileProcessor ?? throw new ArgumentNullException(nameof(azureFileProcessor));
        }

        public async Task<List<FileContent>> GetFileContent(List<string> dealersCsvSource)
        {
            List<FileContent> lstFileContents = new List<FileContent>();
            foreach (var csvSource in dealersCsvSource)
            {
                var source = (CsvSource)Enum.Parse(typeof(CsvSource), csvSource.ToString(), true);
                switch (source)
                {
                    case CsvSource.Email:
                        {
                            // Process Email content
                            var emailcontent = await _emailProcessor.ProcessEmailContent();
                            lstFileContents.AddRange(emailcontent);
                            break;

                        }
                    case CsvSource.AzureFile:
                        {
                            // Process azure file content.
                            lstFileContents.AddRange(await _azureFileProcessor.ProcessAzureFilesContent());
                            break;
                        }
                }
            }
            return lstFileContents;
        }
    }
}
