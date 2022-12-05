using Microsoft.FeatureManagement;
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

        public List<Product> GetProducts()
        {
            SqlConnection conn = GetConnection();
            List<Product> values = new();
            string statement = "SELECT * FROM DBO.PRODUCTS";

            conn.Open();

            SqlCommand cmd = new SqlCommand(statement, conn);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Product product = new()
                    {
                        ProductId = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        Quantity = reader.GetInt32(2)
                    };
                    values.Add(product);
                }
            }

            conn.Close();
            return values;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration["SQLConnection"]);
        }
    }
}
