﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return $"Kowalski, Majewski, Andrzejewski sortowanie ={orderBy}";
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


    }
}