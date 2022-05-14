using System;
using System.Data.SqlClient;
using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using TestWebApp.Models;

namespace TestWebApp.Services
{
    public class CourseService : ICourseService
    {
        private IConfiguration config;

        // configuration is passed through with dependency injection, not currently used but it was used for retrieving the connection string for the database.
        public CourseService(IConfiguration configuration)
        {
            config = configuration;
        }

        private SqlConnection GetConnection()
        {
            // Create a client to connect to the Key Vault. DefaultAzureCredential will use my Azure account locally and a managed identity in Azure when deployed.
            var client = new SecretClient(new Uri("https://az204appservicekeyvault.vault.azure.net/"), new DefaultAzureCredential());
            // Retrieve the connection string from the Key Vault
            return new SqlConnection(client.GetSecret("SQLConnection").Value.Value);
        }

        public IEnumerable<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();
            string query = "SELECT CourseID, CourseName, Rating from Course";
            SqlConnection connection = GetConnection();
            connection.Open();
            SqlCommand sqlCommand = new SqlCommand(query, connection);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Course c = new Course()
                    {
                        CourseID = reader.GetInt32(0),
                        CourseName = reader.GetString(1),
                        Rating = reader.GetDecimal(2)
                    };

                    courses.Add(c);
                }
            }

            connection.Close();
            return courses;
        }

        // Retrieves courses from an Azure Function instead of directly going against the database.
        public async Task<IEnumerable<Course>> GetCoursesFromFunction()
        {
            string functionUrl = "https://az204patrickfunction.azurewebsites.net/api/GetCourses?code=T4OvfG30RMJxrKoI6KyogxeUDeSVZPe0pfjco0u8WnnGAzFuVNf89g==";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(functionUrl);
                string content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<Course>>(content);
            }
        }
    }
}