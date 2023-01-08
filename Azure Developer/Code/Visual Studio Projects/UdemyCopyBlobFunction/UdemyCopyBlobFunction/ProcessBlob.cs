using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace UdemyCopyBlobFunction
{
    public class ProcessBlob
    {
        [FunctionName("ProcessBlob")]
        public async Task Run([BlobTrigger("data/{name}", Connection = "connectionString")]Stream myBlob, [Blob("newdata/{name}", FileAccess.ReadWrite)] BlobClient newBlob, string name, ILogger log)
        {
            await newBlob.UploadAsync(myBlob);
            log.LogInformation("Blob is copied");
        }
    }
}
