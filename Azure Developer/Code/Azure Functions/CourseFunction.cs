using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;

namespace My.Functions
{
    public static class CourseFunction
    {
        [FunctionName("GetCourses")]
        public static async Task<IActionResult> GetCourses(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            List<Course> courses = GetCoursesFromDatabase();

            return new OkObjectResult(JsonConvert.SerializeObject(courses));
        }

        [FunctionName("AddCourse")]
        public static async Task<IActionResult> AddCourse(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Read the course from the request and deserialize it into an object
            string request = await new StreamReader(req.Body).ReadToEndAsync();
            Course data = JsonConvert.DeserializeObject<Course>(request);
            log.LogInformation($"Course: {data}");

            InsertCourse(data);

            return new OkObjectResult("Course added");
        }

        private static SqlConnection GetConnection()
        {
            // Create a client to connect to the Key Vault. DefaultAzureCredential will use my Azure account locally and a managed identity in Azure when deployed.
            var client = new SecretClient(new Uri("https://az204appservicekeyvault.vault.azure.net/"), new DefaultAzureCredential());
            // Retrieve the connection string from the Key Vault
            return new SqlConnection(client.GetSecret("SQLConnection").Value.Value);
        }

        public static List<Course> GetCoursesFromDatabase()
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

        public static void InsertCourse(Course course)
        {
            string insertStatement = "INSERT INTO COURSE(CourseID, CourseName, Rating) VALUES(@param1, @param2, @param3)";
            SqlConnection connection = GetConnection();
            connection.Open();

            using (SqlCommand insertCommand = new SqlCommand(insertStatement, connection))
            {
                insertCommand.Parameters.Add("@param1", System.Data.SqlDbType.Int).Value = course.CourseID;
                insertCommand.Parameters.Add("@param2", System.Data.SqlDbType.VarChar).Value = course.CourseName;
                insertCommand.Parameters.Add("@param3", System.Data.SqlDbType.Decimal).Value = course.Rating;
                insertCommand.CommandType = System.Data.CommandType.Text;
                insertCommand.ExecuteNonQuery();
            }
        }
    }

    public class Course
    {
        public int CourseID { get; set; }

        public string CourseName { get; set; }

        public decimal Rating { get; set; }
    }
}
