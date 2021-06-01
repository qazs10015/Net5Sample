using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using webapi1.Models;
using Microsoft.AspNetCore.Authorization;

namespace webapi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : ControllerBase
    {
        public ContosouniversityContext dbContext { get; }
        public DepartmentController(ContosouniversityContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("")]
        public ActionResult<List<Department>> GetDepartments()
        {
            // dynamic oriItem = (from item in dbContext.Department join item2 in dbContext.Person on item.InstructorId equals item2.Id select new { item, item2 }).ToList();

            // vDepartment newItem = Mapper.Map<vDepartment>(oriItem);
            List<Department> oriItem = (from item in dbContext.Department select item).ToList();
            return oriItem;
        }

        [HttpGet("{id}")]
        public ActionResult<Department> GetDepartmentById(int id)
        {
            return null;
        }

        [HttpPost("")]
        public ActionResult<Department> PostDepartment(Department model)
        {
            return null;
        }

        [HttpPut("{id}")]
        public IActionResult PutDepartment(int id, Department model)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult<Department> DeleteDepartmentById(int id)
        {
            return null;
        }
    }
}