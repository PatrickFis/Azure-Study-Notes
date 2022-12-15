using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;
using System.Text;

// Generate these from an app registration in AAD
string tenantId = "";
string clientId = "";
string clientSecret = "";

string keyVaultUri = "https://az204udemykeyvault.vault.azure.net/";
string keyName = "appKey";
string secretName = "testsecret";
string secretVersion = "ba7c48181b564be3a4415acb67cd41a2";
string textToEncrypt = "This is a secret text";

//await EncryptAndDecryptText();
//await GetSecret();
await GetSecretUsingManagedIdentity();
//await UseManagedIdentityManually();

async Task EncryptAndDecryptText()
{
    ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret);
    KeyClient keyClient = new(new Uri(keyVaultUri), clientSecretCredential);

    var key = await keyClient.GetKeyAsync(keyName);

    var cryptoClient = new CryptographyClient(key.Value.Id, clientSecretCredential);

    byte[] textToBytes = Encoding.UTF8.GetBytes(textToEncrypt);

    EncryptResult result = cryptoClient.Encrypt(EncryptionAlgorithm.RsaOaep, textToBytes);

    Console.WriteLine($"The encrypted string {Convert.ToBase64String(result.Ciphertext)}");

    byte[] cipherToBytes = result.Ciphertext;

    DecryptResult textDecrypted = cryptoClient.Decrypt(EncryptionAlgorithm.RsaOaep, cipherToBytes);

    Console.WriteLine($"The decrypted string {Encoding.UTF8.GetString(textDecrypted.Plaintext)}");
}

async Task GetSecret()
{
    ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret);
    SecretClient secretClient = new(new Uri(keyVaultUri), clientSecretCredential);

    var secret = await secretClient.GetSecretAsync(secretName);

    Console.WriteLine($"The secret is {secret.Value.Value}");
}

async Task GetSecretUsingManagedIdentity()
{
    TokenCredential tokenCredential = new DefaultAzureCredential();
    SecretClient secretClient = new(new Uri(keyVaultUri), tokenCredential);

    var secret = await secretClient.GetSecretAsync(secretName);

    Console.WriteLine($"The secret is {secret.Value.Value}");
}

async Task UseManagedIdentityManually()
{
    // Get an access token
    string tokenUri = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://vault.azure.net";

    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("Metadata", "true");

    HttpResponseMessage response = await client.GetAsync(tokenUri);

    string content = await response.Content.ReadAsStringAsync();

    Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

    foreach(KeyValuePair<string, string> pair in values)
    {
        Console.WriteLine($"{pair.Key}:, {pair.Value}");
    }

    // Access the secret using the token
    string secretUri = $"{keyVaultUri}/secrets/{secretName}/{secretVersion}?api-version=7.3";
    HttpClient secretClient = new HttpClient();
    secretClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", values["access_token"]);
    
    HttpResponseMessage secretResponse = await secretClient.GetAsync(secretUri);
    string secret = await secretResponse.Content.ReadAsStringAsync();
    Console.WriteLine($"The secret is {secret}");
}