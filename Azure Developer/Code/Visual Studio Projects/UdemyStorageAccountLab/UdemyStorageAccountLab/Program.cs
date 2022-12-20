using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

// Strings for using access keys
string connectionString = "your connection string";
string containerName = "scripts";
string blobName = "File you'd like to store";


// Strings for using an app registration instead of access keys
string clientId = "";
string tenantId = "";
string clientSecret = "";
string blobUri = "URI for the blob you need to access";

// Strings applicable to everything
string filePath = "path to file you'd like to store";
string downloadPath = "path to where you'd like the file to be downloaded";

// Code is divided into individual sections that can be run 

// Access keys section
//await createBlobContainer(containerName);
//await uploadBlobToContainer(blobName, filePath);
//await listBlobsInContainer(containerName);
//await downloadBlobFromContainer(blobName);
//await setBlobMetaData(blobName);
//await getMetadata(blobName);
//await acquireLease(blobName);

// App registration section
await DownloadBlobUsingAppRegistration();

// Connecting to a storage account and creating a container
async Task createBlobContainer(string containerName)
{
    BlobServiceClient blobServiceClient = new(connectionString);

    await blobServiceClient.CreateBlobContainerAsync(containerName);

    Console.WriteLine("Container Created");
}

// Upload a blob to the specified container
async Task uploadBlobToContainer(string blobName, string filePath)
{
    BlobContainerClient blobContainerClient = new(connectionString, containerName);

    var blobClient = blobContainerClient.GetBlobClient(blobName);
    await blobClient.UploadAsync(filePath, true);

    Console.WriteLine("Uploaded blob");
}

// List the blobs in the specified container
async Task listBlobsInContainer(string containerName)
{
    BlobContainerClient blobContainerClient = new(connectionString, containerName);

    await foreach(BlobItem blobItem in blobContainerClient.GetBlobsAsync())
    {
        Console.WriteLine($"Blob name: {blobItem.Name}, content type: {blobItem.Properties.ContentType}, content length: {blobItem.Properties.ContentLength}");
    }
}

// Download a blob from a container
async Task downloadBlobFromContainer(string blobName)
{
    BlobClient blobClient = new(connectionString, containerName, blobName);
    await blobClient.DownloadToAsync(downloadPath);

    Console.WriteLine("Download complete");
}

// Put metadata on the specified blob
async Task setBlobMetaData(string blobName)
{
    BlobClient blobClient = new(connectionString, containerName, blobName);

    IDictionary<string, string> metadata = new Dictionary<string, string>();
    metadata.Add("Department", "Azure");
    metadata.Add("Application", "App");

    await blobClient.SetMetadataAsync(metadata);

    Console.WriteLine("Metadata added");
}

// List the metadata on the specified blob
async Task getMetadata(string blobName)
{
    BlobClient blobClient = new(connectionString, containerName, blobName);
    BlobProperties properties = await blobClient.GetPropertiesAsync();

    foreach(var metadata in properties.Metadata)
    {
        Console.WriteLine($"The key is {metadata.Key}. The value is {metadata.Value}.");
    }
}

// Acquire a minute long lease on the specified blob
async Task acquireLease(string blobName)
{
    BlobClient blobClient = new(connectionString, containerName, blobName);
    BlobLeaseClient leaseClient = blobClient.GetBlobLeaseClient();

    // 1 minute lease
    TimeSpan leaseTime = new(0, 1, 0);

    Response<BlobLease> response = await leaseClient.AcquireAsync(leaseTime);

    Console.WriteLine($"Lease ID is {response.Value.LeaseId}");
}

// Download a blob using an app registration
async Task DownloadBlobUsingAppRegistration()
{
    ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret);

    BlobClient blobClient = new(new Uri(blobUri), clientSecretCredential);

    await blobClient.DownloadToAsync(downloadPath);

    Console.WriteLine("The blob is downloaded");
}