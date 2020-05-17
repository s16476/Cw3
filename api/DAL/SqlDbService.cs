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
            

            //Czy istnieje kierunek
            command.CommandText = "select s from studies s where name = @studiesTitle";
            command.Parameters.AddWithValue("studiesTitle", enrollments.Studies);
            connection.Open();
            var data = command.ExecuteReader();

            var studies = new Studies();
            if (data.Read())
            {
                studies.IdStudy = Convert.ToInt32(data["IdStudy"]);
                studies.Name = data["Name"].ToString();
            } else
            {
                throw new InvalidArgumentException();
            }
            command.Parameters.Clear();


            //Najświeższy semestr wybranego kierunku
            command.CommandText = "select top 1 * from Enrollment where IdStudy = @studyId order by StartDate desc";
            command.Parameters.AddWithValue("studyId", studies.IdStudy);
            connection.Open();
            data = command.ExecuteReader();

            var enrollment = new Enrollment();
            var enrollmentId = 0;
            if (data.Read())
            {
                enrollment.IdEnrollment = Convert.ToInt32(data["IdEnrollment"]);
                enrollmentId = enrollment.IdEnrollment;
                enrollment.IdStudy = Convert.ToInt32(data["IdStudy"]);
                enrollment.Semester = Convert.ToInt32(data["Semester"]);
                enrollment.StartDate = Convert.ToDateTime(data["StartDate"]);
            }
            else
            //dodanie jeśli nie istnieje 
            {
                command.CommandText = "select top 1 * from Enrollment order by IdEnrollment desc";
                connection.Open();

                data.Read();
                enrollmentId = Convert.ToInt32(data["IdEnrollment"]) + 1;

                command.CommandText = "insert into Enrollment values (@id, 1, @studyId, GETDATE())";
                command.Parameters.AddWithValue("studyId", studies.IdStudy);
                command.Parameters.AddWithValue("id", enrollmentId);
                connection.Open();
            }
            command.Parameters.Clear();

            //Czy prawidłowy index
            command.CommandText = "select * from Student where IndexNumber =  @index";
            command.Parameters.AddWithValue("index", enrollments.IndexNumber);
            connection.Open();
            data = command.ExecuteReader();
            if (data.Read())
            {
                throw new InvalidArgumentException();
            }
            command.Parameters.Clear();

            //Dodajemy studenta
            command.CommandText = "insert into Student values (@index, @firstName, @lastName, @birthDate, @enrollment)";
            command.Parameters.AddWithValue("index", enrollments.IndexNumber);
            command.Parameters.AddWithValue("firstName", enrollments.FirstName);
            command.Parameters.AddWithValue("lastName", enrollments.LastName);
            command.Parameters.AddWithValue("birthDate", enrollments.BirthDate);
            command.Parameters.AddWithValue("enrollment", enrollmentId);
            connection.Open();
            data = command.ExecuteReader();
            command.Parameters.Clear();





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
