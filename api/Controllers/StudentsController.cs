using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        [HttpGet]
        public string GetStudents(string orderBy)
        {
            return $"Kowalski, Majewski, Andrzejewski sortowanie= {orderBy}";
        }

        [HttpGet("{id}")]
        public IActionResult GetStudents(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            } else if (id == 2)
            {
                return Ok("Majewski");
            }

            return NotFound("Nie znaleziono studenta");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student student)
        {
            //aktualizacja studenta o {id}
            return Ok("Aktualizacja studenta o id " + id + " udana");
        }

        [HttpDelete("{id}")]
        public IActionResult UpdateStudent(int id)
        {
            //usuwanie studenta o {id}
            return Ok("Usuwanie studenta o id " + id + "zakończone");
        }



    }
}