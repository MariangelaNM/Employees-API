using Employees_Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Employees_Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DepartmentController : ControllerBase
  {
    public readonly string con;
    public DepartmentController(IConfiguration configuration)
    {
      con = configuration["ConnectionStrings:conexion"];
    }

    [HttpGet]
    public IEnumerable<Department> Get()
    {
      List<Department> Departments = new();

      using (SqlConnection connection = new(con))
      {
        connection.Open();
        using (SqlCommand cmd = new("GetDepartments", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          using (SqlDataReader reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              Department p = new Department
              {
                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                DepartmentName = reader["DepartmentName"].ToString(),

              };

              Departments.Add(p);
            }
          }
        }
      }
      return Departments;
    }

    [HttpPost]
    public IActionResult Post([FromBody] Department department)
    {
      // Check if the department name already exists
      if (DepartmentExists(department.DepartmentName, null))
      {
        ModelState.AddModelError("DepartmentName", "Department with this name already exists.");
        return BadRequest(ModelState);
      }
      // The rest of your code for inserting the department
      try
      {
        using (SqlConnection connection = new SqlConnection(con))
        {
          connection.Open();
          using (SqlCommand cmd = new SqlCommand("AddDepartment", connection))
          {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
            cmd.ExecuteNonQuery();
          }
        }
        return Ok(); // Department inserted successfully
      }
      catch (Exception ex)
      {
        // Handle any other exceptions that might occur during the insertion
        return StatusCode(500, $"Internal Server Error: {ex.Message}");
      }
    }

    [HttpPut("{id}")]
    public IActionResult Put([FromBody] Department p, int id)
    {
      // Check if the department with the given ID exists before updating
      if (!DepartmentExists(p.DepartmentName, null))

        if (DepartmentExists(null, id))
        {
          using (SqlConnection connection = new SqlConnection(con))
          {
            connection.Open();
            using (SqlCommand cmd = new SqlCommand("UpdateDepartment", connection))
            {
              cmd.CommandType = System.Data.CommandType.StoredProcedure;
              cmd.Parameters.AddWithValue("@DepartmentID", id);
              cmd.Parameters.AddWithValue("@NewDepartmentName", p.DepartmentName);
              cmd.ExecuteNonQuery();
            }
            // Return a success response, e.g., a 204 No Content response
            return NoContent();
          }
        }
        else
        {
          // Add a model error for the "DepartmentName" field
          ModelState.AddModelError("DepartmentID", "Department with this id does not exist.");
          // Return a BadRequest response with the ModelState containing the error
          return BadRequest(ModelState);
        }

      else
      {
        // Add a model error for the "DepartmentName" field
        ModelState.AddModelError("DepartmentName", "Department with this name does not exist.");
        // Return a BadRequest response with the ModelState containing the error
        return BadRequest(ModelState);
      }
    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      // Check if the department with the given ID exists before deleting
      if (DepartmentExists(null, id)) // Pass null as the name parameter, as it's not used in the delete operation
      {
        using (SqlConnection connection = new SqlConnection(con))
        {
          connection.Open();
          using (SqlCommand cmd = new SqlCommand("DeleteDepartment", connection))
          {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DepartmentID", id);
            cmd.ExecuteNonQuery();
          }

          // Return a success response, e.g., a 204 No Content response
          return NoContent();
        }
      }
      else
      {
        // Add a model error for the "DepartmentID" field
        ModelState.AddModelError("DepartmentID", "Department with this ID does not exist.");

        // Return a BadRequest response with the ModelState containing the error
        return BadRequest(ModelState);
      }
    }

    // Method to check if an employee name already exists
    private bool DepartmentExists(string name, int? id = null)
    {
      using (SqlConnection connection = new SqlConnection(con))
      {
        connection.Open();
        using (SqlCommand cmd = new SqlCommand("CheckDepartmentExists", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@DepartmentName", name);

          // Check if id is not null before adding it as a parameter
          if (id.HasValue)
          {
            cmd.Parameters.AddWithValue("@DepartmentId", id.Value);
          }
          else
          {
            // If id is null, set it to DBNull.Value
            cmd.Parameters.AddWithValue("@DepartmentId", DBNull.Value);
          }

          // Use ExecuteScalar safely by checking for DBNull.Value
          object result = cmd.ExecuteScalar();
          return result != null && Convert.ToBoolean(result);
        }
      }
    }

  }
}
