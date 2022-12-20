using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace UdemyAuthApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;
        public string accessToken = "Login again to get an access token";
        public string blobContent = "Login again to see blob contents";

        public IndexModel(ILogger<IndexModel> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task OnGetAsync()
        {
            // Try out getting an access token and displaying it to the user. Note that you'll need to sign in again by first going to /signout-oidc and then back to /.
            string[] scope = new string[] { "https://storage.azure.com/user_impersonation" };
            try
            {
                accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scope);
            }
            catch (Exception e)
            {
                _logger.LogError("Error", e);
            }

            try
            {
                TokenAcquisitionTokenCredential credential = new(_tokenAcquisition);
                Uri blobUri = new("https://az204patrickstorage.blob.core.windows.net/data/Cosmos.json");
                BlobClient blobClient = new(blobUri, credential);

                MemoryStream ms = new();
                blobClient.DownloadTo(ms);
                ms.Position = 0;

                StreamReader sr = new StreamReader(ms);
                blobContent = sr.ReadToEnd();
            } catch(Exception e)
            {
                _logger.LogError("Error", e);
            }
        }
    }
}