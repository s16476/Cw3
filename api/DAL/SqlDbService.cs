using api.exceptions;
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

        public IEnumerable<Enrollment> GetEnrollmentsByStudentId(string id)
        {
            List<Enrollment> _enrollments = new List<Enrollment>();
            using (var connection = new SqlConnection("Data Source=localhost;Initial Catalog=apbd;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "select e.* from Enrollment e join Student s on e.IdEnrollment = s.IdEnrollment where s.IndexNumber = @id";
                command.Parameters.AddWithValue("id", id);

                connection.Open();
                var data = command.ExecuteReader();
                while (data.Read())
                {
                    var enrollment = new Enrollment();
                    enrollment.IdEnrollment = Convert.ToInt32(data["IdEnrollment"]);
                    enrollment.IdStudy = Convert.ToInt32(data["IdStudy"]);
                    enrollment.Semester = Convert.ToInt32(data["Semester"]);
                    enrollment.StartDate = Convert.ToDateTime(data["StartDate"]);
                    _enrollments.Add(enrollment);
                }

            }

            return _enrollments;
        }

        public Enrollment EnrollStudentToStudies(StudentEnrollment enrollments)
        {
            if (!IsValidEnrollments(enrollments))
            {
                throw new InvalidArgumentException();
            } 

            var connection = new SqlConnection("Data Source=localhost;Initial Catalog=apbd;Integrated Security=True");
            var command = new SqlCommand();
            command.Connection = connection;
            var tran = connection.BeginTransaction();


            
            command.CommandText = "select count(s.IdStudy) from studies s where name = @studiesTitle";
            command.Parameters.AddWithValue("studiesTitle", enrollments.Studies);
            connection.Open();
            var data = command.ExecuteReader();
            var enrollment = 0;
            while (data.Read())
            {
                enrollment = Convert.ToInt32(data[0]);
            }
            if (enrollment != 1)
            {
                throw new InvalidArgumentException();
            }


            tran.Commit();


            return null;
        }


        private Boolean IsValidEnrollments(StudentEnrollment enrollments)
        {
            if (String.IsNullOrWhiteSpace(enrollments.IndexNumber) ||
                String.IsNullOrWhiteSpace(enrollments.FirstName) ||
                String.IsNullOrWhiteSpace(enrollments.LastName) ||
                String.IsNullOrWhiteSpace(enrollments.Studies) ||
                enrollments.BirthDate == null)
            {
                return false;
            }
            return true;
        }

    }
}
