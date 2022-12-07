using Microsoft.FeatureManagement;
using Newtonsoft.Json;
using System.Data.SqlClient;
using UdemyWebApp.Models;

namespace UdemyWebApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly IFeatureManager _featureManager;

        public ProductService(IConfiguration configuration, IFeatureManager featureManager)
        {
            _configuration = configuration;
            _featureManager = featureManager;
        }

        public async Task<bool> IsBeta()
        {
            if (await _featureManager.IsEnabledAsync("udemyBeta"))
            {
                return true;
            }
            return false;
        }

        public async Task<List<Product>> GetProducts()
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(_configuration["GetProductsUrl"]);
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Product>>(content);
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration["SQLConnection"]);
        }
    }
}
