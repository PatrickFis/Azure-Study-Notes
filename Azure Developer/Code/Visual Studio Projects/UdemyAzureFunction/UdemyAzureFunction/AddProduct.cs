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

namespace UdemyAzureFunction
{
    public static class AddProduct
    {
        [FunctionName("AddProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Product data = JsonConvert.DeserializeObject<Product>(requestBody);

            SqlConnection connection = GetConnection();
            connection.Open();
            string statement = "INSERT INTO DBO.PRODUCTS(ProductID, ProductName, Quantity) VALUES(@param1, @param2, @param3)";

            using (SqlCommand command = new SqlCommand(statement, connection))
            {
                command.Parameters.Add("@param1", System.Data.SqlDbType.Int).Value = data.ProductId;
                command.Parameters.Add("@param2", System.Data.SqlDbType.VarChar, 1000).Value = data.ProductName;
                command.Parameters.Add("@param3", System.Data.SqlDbType.Int).Value = data.Quantity;
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();
            }
            connection.Close();

            return new OkObjectResult("Product added");
        }

        private static SqlConnection GetConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SQLConnectionString");
            return new SqlConnection(connectionString);
        }
    }
}
