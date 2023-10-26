using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EvoWordToPdf;
using System.Text;

namespace PrintToPDF
{
    public class UploadAndPrintBlobToPdf
    {
        [FunctionName("UploadAndPrintBlobToPdf")]
        public void Run([BlobTrigger("uploads/{name}", Connection = "StorageConnection")] Stream myBlob, string name, ILogger log)
        {
            if (name.EndsWith(".PDF", StringComparison.CurrentCultureIgnoreCase)) return;

            // get Environment variables
            string connectionString = Environment.GetEnvironmentVariable("StorageConnection");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            // Create a BlobServiceClient object 
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Download the blob to a local file
            string downloadedBlobFilePath = $"{Path.GetTempPath()}\\{name}";

            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadedBlobFilePath);

            // Get a reference to a blob
            BlobClient downloadBlobClient = containerClient.GetBlobClient(name);

            // Download the blob's contents and save it to a file
            downloadBlobClient.DownloadTo(downloadedBlobFilePath);

            // Convert to Pdf format
            string convertedFilePath = ConvertMsWordToPDF(downloadedBlobFilePath);

            // Upload the converted file back to Blob Storage
            BlobClient uploadBlobClient = containerClient.GetBlobClient(convertedFilePath);

            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", downloadBlobClient.Uri);

            // Upload data from the local file
            uploadBlobClient.Upload(Path.GetFileName(convertedFilePath), true);

            // delete local files
            File.Delete(downloadedBlobFilePath);
            File.Delete(convertedFilePath);
        }

        private string ConvertMsWordToPDF(string wordFilePath)
        {
            string convertedFilePath = $"{Path.GetFileNameWithoutExtension(wordFilePath)}.pdf";
            WordToPdfConverter converter = new WordToPdfConverter();
            converter.ConvertWordFileToFile(wordFilePath, convertedFilePath);

            return convertedFilePath;
        }
    }
}
