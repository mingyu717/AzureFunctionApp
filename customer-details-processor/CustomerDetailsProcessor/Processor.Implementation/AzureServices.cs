using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Processor.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Processor.Implementation
{
    public class AzureServices : IAzureServices
    {
        private readonly CloudFileShare _cloudFileShare;
        private readonly string _connectionString;
        private readonly string _shareName;
        private readonly string _azureFileProcessedFolderName;
        private readonly string _appendDateFormatInFileName;

        public AzureServices(string connectionString, string shareName, string azureFileProcessedFolderName, string appendDateFormatInFileName)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _shareName = shareName ?? throw new ArgumentNullException(nameof(shareName));
            _azureFileProcessedFolderName = azureFileProcessedFolderName ?? throw new ArgumentNullException(nameof(azureFileProcessedFolderName));
            _appendDateFormatInFileName = appendDateFormatInFileName ?? throw new ArgumentNullException(nameof(appendDateFormatInFileName));
            _cloudFileShare = Connect();
        }

        public async Task<List<FileContent>> GetCloudContent()
        {
            List<FileContent> lstFileContent = new List<FileContent>();

            if (_cloudFileShare == null) throw new NullReferenceException(nameof(_cloudFileShare));

            if (_cloudFileShare.Exists())
            {
                CloudFileDirectory cloudRootDirectory = _cloudFileShare.GetRootDirectoryReference();
                if (cloudRootDirectory.Exists())
                {
                    var lstFilesAndDirectories = cloudRootDirectory.ListFilesAndDirectories();
                    foreach (var file in lstFilesAndDirectories)
                    {
                        if (file is CloudFile)
                        {
                            CloudFile cloudFile = file as CloudFile;
                            var fileContent = await ParseFileContent(cloudFile);
                            lstFileContent.Add(fileContent);
                            MoveFileToProcessedFolder(cloudFile);
                        }
                    }
                }
            }
            return lstFileContent;

        }

        private CloudFileShare Connect()
        {
            CloudStorageAccount cloudStorageAccount;
            if (CloudStorageAccount.TryParse(_connectionString, out cloudStorageAccount))
            {
                var cloudFileClient = cloudStorageAccount.CreateCloudFileClient();
                return cloudFileClient.GetShareReference(_shareName);
            }
            throw new FormatException(nameof(_connectionString));
        }

        private async Task<FileContent> ParseFileContent(CloudFile cloudFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await cloudFile.DownloadToStreamAsync(memoryStream);
                return new FileContent
                {
                    FileName = cloudFile.Name,
                    Content = memoryStream.ToArray()
                };
            }
        }

        private async void MoveFileToProcessedFolder(CloudFile cloudFile)
        {
            CloudFileDirectory rootDirectory = _cloudFileShare.GetRootDirectoryReference();
            if (rootDirectory.Exists())
            {
                CloudFileDirectory customDirectory = rootDirectory.GetDirectoryReference(_azureFileProcessedFolderName);
                customDirectory.CreateIfNotExists();
                var fileName = $"{Path.GetFileNameWithoutExtension(cloudFile.Name)}-{DateTime.Now.ToString(_appendDateFormatInFileName)}{Path.GetExtension(cloudFile.Name)}";
                CloudFile file = customDirectory.GetFileReference(fileName);
                Uri fileUrl = new Uri(cloudFile.StorageUri.PrimaryUri.ToString());
                await file.StartCopyAsync(fileUrl);
                await cloudFile.DeleteAsync();
            }
        }
    }
}
