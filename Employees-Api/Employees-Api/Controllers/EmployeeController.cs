using Employees_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Employees_Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmployeeController : ControllerBase
  {
    private readonly string _connectionString;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(IConfiguration configuration, ILogger<EmployeeController> logger)
    {
      _connectionString = configuration["ConnectionStrings:conexion"];
      _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
      try
      {
        var employees = new List<Employee>();

        using (var connection = new SqlConnection(_connectionString))
        {
          await connection.OpenAsync();

          using (var cmd = new SqlCommand("GetEmployees", connection))
          {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            using (var reader = await cmd.ExecuteReaderAsync())
            {
              while (await reader.ReadAsync())
              {
                var employee = new Employee
                {
                  EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                  EmployeeDNI = Convert.ToInt32(reader["EmployeeDNI"]),
                  EmployeeName = reader["EmployeeName"].ToString(),
                  EmployeeLastName = reader["EmployeeLastName"].ToString(),
                  DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                };

                employees.Add(employee);
              }
            }
          }
        }

        return Ok(employees); // 200 OK
      }
      catch (Exception ex)
      {
        _logger.LogError($"An error occurred: {ex.Message}");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Employee employee)
    {
      try
      {
        if (await EmployeeExistsAsync(employee.EmployeeDNI, null))
        {
          ModelState.AddModelError("Employee DNI", "Employee with this DNI already exists.");
          return BadRequest(ModelState);
        }

        using (var connection = new SqlConnection(_connectionString))
        {
          await connection.OpenAsync();

          using (var cmd = new SqlCommand("InsertEmployee", connection))
          {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
            cmd.Parameters.AddWithValue("@EmployeeLastName", employee.EmployeeLastName);
            cmd.Parameters.AddWithValue("@DepartmentID", employee.DepartmentID);
            cmd.Parameters.AddWithValue("@EmployeeDNI", employee.EmployeeDNI);
            await cmd.ExecuteNonQueryAsync();
          }
        }

        return Ok(); // Employee inserted successfully
      }
      catch (Exception ex)
      {
        _logger.LogError($"Internal Server Error: {ex.Message}");
        return StatusCode(500, $"Internal Server Error: {ex.Message}");
      }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromBody] Employee p, int id)
    {
      try
      {
        if (!await EmployeeExistsAsync(p.EmployeeDNI, null))
        {
          ModelState.AddModelError("EmployeeDNI", "Employee with this DNI does not exist.");
          return BadRequest(ModelState);
        }

        if (await EmployeeExistsAsync(null, id))
        {
          using (var connection = new SqlConnection(_connectionString))
          {
            await connection.OpenAsync();
            using (var cmd = new SqlCommand("UpdateEmployee", connection))
            {
              cmd.CommandType = System.Data.CommandType.StoredProcedure;
              cmd.Parameters.AddWithValue("@EmployeeID", id);
              cmd.Parameters.AddWithValue("@EmployeeName", p.EmployeeName);
              cmd.Parameters.AddWithValue("@EmployeeLastName", p.EmployeeLastName);
              cmd.Parameters.AddWithValue("@DepartmentID", p.DepartmentID);
              await cmd.ExecuteNonQueryAsync();
            }

            return NoContent();
          }
        }
        else
        {
          ModelState.AddModelError("EmployeeDNI", "Employee with this id not exist.");
          return BadRequest(ModelState);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Internal Server Error: {ex.Message}");
        return StatusCode(500, $"Internal Server Error: {ex.Message}");
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        if (await EmployeeExistsAsync(null, id))
        {
          using (var connection = new SqlConnection(_connectionString))
          {
            await connection.OpenAsync();
            using (var cmd = new SqlCommand("DeleteEmployee", connection))
            {
              cmd.CommandType = System.Data.CommandType.StoredProcedure;
              cmd.Parameters.AddWithValue("@EmployeeID", id);

              await cmd.ExecuteNonQueryAsync();
            }
          }

          return NoContent();
        }
        else
        {
          ModelState.AddModelError("EmployeeDNI", "Employee with this DNI does not exist.");
          return BadRequest(ModelState);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Internal Server Error: {ex.Message}");
        return StatusCode(500, $"Internal Server Error: {ex.Message}");
      }
    }

    private async Task<bool> EmployeeExistsAsync(int? dni = null, int? id = null)
    {
      using (var connection = new SqlConnection(_connectionString))
      {
        await connection.OpenAsync();

        using (var cmd = new SqlCommand("CheckEmployeeExists", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@EmployeeDNI", dni);

          if (id.HasValue)
          {
            cmd.Parameters.AddWithValue("@EmployeeId", id.Value);
          }
          else
          {
            cmd.Parameters.AddWithValue("@EmployeeId", DBNull.Value);
          }

          var result = await cmd.ExecuteScalarAsync();
          return result != null && Convert.ToBoolean(result);
        }
      }
    }
  }
}
