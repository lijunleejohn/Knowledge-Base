using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Communication.Email;
using EvoWordToPdf;
using System.Text;
using Azure;
using System.Net.Mime;

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
            string downloadedBlobFilePath = $"{Path.GetTempPath()}{name}";

            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadedBlobFilePath);

            // Get a reference to a blob
            BlobClient downloadBlobClient = containerClient.GetBlobClient(name);

            // Download the blob's contents and save it to a file
            downloadBlobClient.DownloadTo(downloadedBlobFilePath);

            // Convert to Pdf format
            string convertedFilePath = ConvertMsWordToPDF(downloadedBlobFilePath);

            // Upload the converted file back to Blob Storage
            BlobClient uploadBlobClient = containerClient.GetBlobClient(Path.GetFileName(convertedFilePath));

            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", downloadBlobClient.Uri);

            // Upload data from the local file
            uploadBlobClient.Upload(convertedFilePath, true);

            // Call Azure Email Communication Service to send the converted PDF as attachment out
            SendEmailWithAttachment(convertedFilePath);

            // delete local files
            File.Delete(downloadedBlobFilePath);
            File.Delete(convertedFilePath);
        }

        private string ConvertMsWordToPDF(string wordFilePath)
        {
            string convertedFilePath = $"{Path.GetTempPath()}{Path.GetFileNameWithoutExtension(wordFilePath)}.pdf";
            WordToPdfConverter converter = new WordToPdfConverter();
            converter.ConvertWordFileToFile(wordFilePath, convertedFilePath);

            return convertedFilePath;
        }

        private void SendEmailWithAttachment(string pdfFilePath)
        {
            string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");
            EmailClient emailClient = new EmailClient(connectionString);

            // Create the email content
            var emailContent = new EmailContent("Welcome to Azure Communication Service Email APIs.")
            {
                PlainText = "This email message is sent from Azure Communication Service Email.",
                Html = "<html><body><h1>Quick send email test</h1><br/><h4>This email message is sent from Azure Communication Service Email.</h4><p>This mail was sent using .NET SDK!!</p></body></html>"
            };

            // Create the EmailMessage
            var emailMessage = new EmailMessage(
                senderAddress: "DoNotReply@371abde4-1d08-4f01-a86d-cc6c9d77d568.azurecomm.net", // The email address of the domain registered with the Communication Services resource
                recipientAddress: Environment.GetEnvironmentVariable("RecipientEmail"),
                content: emailContent);

            // Create the EmailAttachment
            byte[] bytes = File.ReadAllBytes(pdfFilePath);
            var contentBinaryData = new BinaryData(bytes);
            var emailAttachment = new EmailAttachment(Path.GetFileName(pdfFilePath), MediaTypeNames.Application.Pdf, contentBinaryData);

            emailMessage.Attachments.Add(emailAttachment);

            try
            {
                EmailSendOperation emailSendOperation = emailClient.Send(WaitUntil.Completed, emailMessage);
                Console.WriteLine($"Email Sent. Status = {emailSendOperation.Value.Status}");

                /// Get the OperationId so that it can be used for tracking the message for troubleshooting
                string operationId = emailSendOperation.Id;
                Console.WriteLine($"Email operation id = {operationId}");
            }
            catch (RequestFailedException ex)
            {
                /// OperationID is contained in the exception message and can be used for troubleshooting purposes
                Console.WriteLine($"Email send operation failed with error code: {ex.ErrorCode}, message: {ex.Message}");
            }
        }
    }
}
