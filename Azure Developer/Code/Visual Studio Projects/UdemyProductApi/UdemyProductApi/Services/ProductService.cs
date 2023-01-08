using System.Data.SqlClient;
using UdemyAuthApi.Models;

namespace UdemyAuthApi.Services
{
    public class ProductService
    {
        private static SqlConnection GetConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SQLConnectionString");
            return new SqlConnection(connectionString);
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

        public Product GetProductByIDFromDB(int productId)
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

        public int AddProduct(Product product)
        {
            SqlConnection conn = GetConnection();
            string statement = "INSERT INTO DBO.PRODUCTS(ProductID, ProductName, Quantity) VALUES(@param1, @param2, @param3)";

            int rowsAffected = 0;
            conn.Open();
            using (SqlCommand command = new SqlCommand(statement, conn))
            {
                command.Parameters.Add("@param1", System.Data.SqlDbType.Int).Value = product.ProductId;
                command.Parameters.Add("@param2", System.Data.SqlDbType.VarChar, 1000).Value = product.ProductName;
                command.Parameters.Add("@param3", System.Data.SqlDbType.Int).Value = product.Quantity;
                command.CommandType = System.Data.CommandType.Text;
                rowsAffected = command.ExecuteNonQuery();
            }
            conn.Close();
            return rowsAffected;
        }

    }
}