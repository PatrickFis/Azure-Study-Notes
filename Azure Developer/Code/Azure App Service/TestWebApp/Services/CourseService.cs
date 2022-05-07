using System;
using System.Data.SqlClient;
using System.Text;
using TestWebApp.Models;

namespace TestWebApp.Services
{
    public class CourseService : ICourseService
    {
        private IConfiguration config;

        // configuration is passed through with dependency injection
        public CourseService(IConfiguration configuration)
        {
            config = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(config.GetConnectionString("SQLConnection"));
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
    }
}