using api.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
    }
}
