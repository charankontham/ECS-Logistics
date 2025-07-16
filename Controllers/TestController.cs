using ECS_Logistics.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECS_Logistics.Controllers;
[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private static List<Employee> _employees =
    [
        new Employee { Id = 1, Name = "Alice", Department = "HR" },
        new Employee { Id = 2, Name = "Bob", Department = "Engineering" },
        new Employee { Id = 3, Name = "Charlie", Department = "Marketing" }
    ];
    
    [HttpGet]
    public IActionResult GetAll() {
        return Ok(_employees);
    }
    
    [HttpGet("getById/{id}")]
    public IActionResult GetById(int id) {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null) return NotFound();
        return Ok(employee);
    }
    
    [HttpPost]
    public IActionResult Create([FromBody] Employee newEmployee) {
        newEmployee.Id = _employees.Max(e => e.Id) + 1;
        _employees.Add(newEmployee);
        return CreatedAtAction(nameof(GetById), new { id = newEmployee.Id }, newEmployee);
    }

    [HttpPut("/{id}")]
    public IActionResult Update(int id, Employee updatedEmployee) {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null) return NotFound();

        employee.Name = updatedEmployee.Name;
        employee.Department = updatedEmployee.Department;
        return Ok(employee);
    }

    [HttpDelete("/{id}")]
    public IActionResult Delete(int id) {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null) return NotFound();

        _employees.Remove(employee);
        return NoContent();
    }
    
}