
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace EducationalFundingCo.Utilities
{
    public class AzureBlobStorage
    {
        string accessKey = string.Empty;
        public IConfiguration GetConfiguration { get; set; }
        public BlobContainerClient GetBlobContainerClient { get; set; }
        public string ContainerName { get; set; }
        public string FileName { get; set; }
        public string SourcePath { get; set; }

        public async Task UploadFileToBlob()
        {
            accessKey = GetConfiguration["BlobConnections:AccessKey"];
            GetBlobContainerClient = new BlobContainerClient(accessKey, ContainerName);

            await GetBlobContainerClient.CreateIfNotExistsAsync();
            await GetBlobContainerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

            BlobClient blob = GetBlobContainerClient.GetBlobClient(FileName);

            using (FileStream fileStream = File.OpenRead(SourcePath))
            {
                if(!await blob.ExistsAsync())
                    await blob.UploadAsync(fileStream);
            }
        }

        public AsyncPageable<BlobItem> GetBlobItems()
        {
            accessKey = GetConfiguration["BlobConnections:AccessKey"];
            GetBlobContainerClient = new BlobContainerClient(accessKey, ContainerName);
            if (!GetBlobContainerClient.Exists())
                return null;

            var blobList = GetBlobContainerClient.GetBlobsAsync();
            return blobList;
        }

    }
}
