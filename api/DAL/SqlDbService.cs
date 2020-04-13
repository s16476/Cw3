using api.models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace api.DAL
{
    public class SqlDbService : IDbService
    {
        public IEnumerable<Student> GetStudents()
        {
            List<Student> _students = new List<Student>();
            using (var connection = new SqlConnection("Data Source=localhost;Initial Catalog=apbd;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "select * from Student";

                connection.Open();
                var data = command.ExecuteReader();
                while (data.Read())
                {
                    var student = new Student();
                    student.FirstName = data["FirstName"].ToString();
                    student.LastName = data["LastName"].ToString();
                    student.IndexNumber = data["IndexNumber"].ToString();
                    _students.Add(student);
                }

            }

            return _students;
        }
    }
}
