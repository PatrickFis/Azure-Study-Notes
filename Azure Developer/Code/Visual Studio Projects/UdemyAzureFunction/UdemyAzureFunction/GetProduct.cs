using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace UdemyAzureFunction
{
    public static class GetProduct
    {
        [FunctionName("GetProducts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            List<Product> products = GetProducts();


            return new OkObjectResult(JsonConvert.SerializeObject(products));
        }

        [FunctionName("GetProduct")]
        public static async Task<IActionResult> GetProductById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var productId = int.Parse(req.Query["id"]);

            try
            {
                return new OkObjectResult(JsonConvert.SerializeObject(GetProductByIDFromDB(productId)));
            }
            catch(Exception)
            {
                return new NotFoundObjectResult("No product found");
            }
        }
        private static SqlConnection GetConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SQLConnectionString");
            return new SqlConnection(connectionString);
        }

        public static List<Product> GetProducts()
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

        public static Product GetProductByIDFromDB(int productId)
        {
            SqlConnection conn = GetConnection();
            List<Product> values = new();
            string statement = String.Format("SELECT * FROM DBO.PRODUCTS WHERE PRODUCTID={0}", productId);

            conn.Open();

            SqlCommand cmd = new SqlCommand(statement, conn);
            Product product = new Product();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                product.ProductId = reader.GetInt32(0);
                product.ProductName = reader.GetString(1);
                product.Quantity = reader.GetInt32(2);
            }

            conn.Close();
            return product;
        }
    }
}
