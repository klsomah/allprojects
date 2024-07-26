using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EducationalFundingCo.Utilities
{
    public class BlobStorageService
    {
        string accessKey = string.Empty;

        public IConfiguration GetConfiguration { get; set; }

        public string UploadFileToBlob(string strContainerName, string strFileName, IFormFile formFile, string fileMimeType)
        {
            try
            {

                var _task = Task.Run(() => this.UploadFileToBlobAsync(strContainerName, strFileName, formFile, fileMimeType));
                _task.Wait();
                string fileUrl = _task.Result;
                return fileUrl;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task<bool> IsAContainerAsync(string strContainerName)
        {
            accessKey = GetConfiguration["BlobConnections:AccessKey"];
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName.ToLower());

            return await cloudBlobContainer.ExistsAsync();
        }

        public async Task<IEnumerable<IListBlobItem>> ListBlobsFlatListingAsync(string strContainerName, int? segmentSize)
        {
            BlobContinuationToken continuationToken = null;
            accessKey = GetConfiguration["BlobConnections:AccessKey"];
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = cloudBlobClient.GetContainerReference(strContainerName.ToLower());
            try
            {
                if (await container.ExistsAsync())
                {
                    BlobResultSegment resultSegment;
                    // Call the listing operation and enumerate the result segment.
                    // When the continuation token is null, the last segment has been returned
                    // and execution can exit the loop.
                    do
                    {
                        resultSegment = await container.ListBlobsSegmentedAsync(string.Empty,
                            true, BlobListingDetails.Metadata, segmentSize, continuationToken, null, null);

                        // Get the continuation token and loop until it is null.
                        continuationToken = resultSegment.ContinuationToken;

                    } while (continuationToken != null);

                    return resultSegment.Results;
                }
                else
                    return null;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }


        private async Task<string> UploadFileToBlobAsync(string strContainerName, string strFileName, IFormFile formFile, string fileMimeType)
        {
            try
            {
                byte[] fileData = ConvertFileToByte(formFile);
                accessKey = GetConfiguration["BlobConnections:AccessKey"];
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName.ToLower());
                //string fileName = this.GenerateFileName(strFileName);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (strFileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(strFileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                return "";
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private byte[] ConvertFileToByte(IFormFile formFile)
        {
            byte[] fileBytes = new byte[0];
            if (formFile.Length > 0)
            {
                using var ms = new MemoryStream();
                formFile.CopyTo(ms);
                fileBytes = ms.ToArray();
                string s = Convert.ToBase64String(fileBytes);
                // act on the Base64 data
            }

            return fileBytes;
        }
    }


}
