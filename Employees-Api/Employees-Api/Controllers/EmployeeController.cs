using Employees_Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

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
                                    Department = new Department
                                    {
                                        DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                        DepartmentName = reader["DepartmentName"].ToString(),
                                    }
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
                        cmd.Parameters.AddWithValue("@DepartmentID", employee.Department.DepartmentID);
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
                if (await EmployeeExistsAsync(p.EmployeeDNI, id))
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
                            cmd.Parameters.AddWithValue("@EmployeeDNI", p.EmployeeDNI);
                            cmd.Parameters.AddWithValue("@EmployeeName", p.EmployeeName);
                            cmd.Parameters.AddWithValue("@EmployeeLastName", p.EmployeeLastName);
                            cmd.Parameters.AddWithValue("@DepartmentID", p.Department.DepartmentID);
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
        
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("GetEmployeeByID", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeID", id);


                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Employee departmentID = new Employee
                                {
                                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                    EmployeeDNI = Convert.ToInt32(reader["EmployeeDNI"]),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    EmployeeLastName = reader["EmployeeLastName"].ToString(),
                                    Department = new Department
                                    {
                                        DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                        DepartmentName = reader["DepartmentName"].ToString(),
                                    }

                                };

                                return Ok(departmentID); // 200 OK with the specific Employee
                            }
                            else
                            {
                                // Return a 404 Not Found if the Employee with the specified id is not found
                                return NotFound("Employee not found");
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Return a 500 Internal Server Error with a meaningful error message
                return StatusCode(500, "Internal Server Error");
            }

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (await EmployeeExistsAsync( null, id))
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
                    ModelState.AddModelError("EmployeeID", "Employee with this id does not exist.");
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
