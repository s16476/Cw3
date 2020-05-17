using System;
using api.DAL;
using api.models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {

        private readonly IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost()]
        public IActionResult EnrollStudentToStudies(StudentEnrollment enrollment)
        {
            try
            {
                return Ok(_dbService.EnrollStudentToStudies(enrollment));
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }





    }
}