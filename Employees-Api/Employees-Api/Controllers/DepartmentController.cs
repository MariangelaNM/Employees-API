using Employees_Api.Models;
using Microsoft.AspNetCore.Http;
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
        using (SqlCommand cmd = new("GetDepartment", connection))
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
    public void Post([FromBody] Department p)
    {
      using (SqlConnection connection = new(con))
      {
        connection.Open();
        using (SqlCommand cmd = new("PostDepartment", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@DepartmentName", p.DepartmentName);
          cmd.ExecuteNonQuery();
        }
      }
    }


    [HttpPut("{id}")]
    public void Put([FromBody] Department p, int id)
    {
      using (SqlConnection connection = new(con))
      {
        connection.Open();
        using (SqlCommand cmd = new("UpdateDepartment", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@DepartmentID", id);
          cmd.Parameters.AddWithValue("@DepartmentName", p.DepartmentName);
          cmd.ExecuteNonQuery();
        }
      }
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
      using (SqlConnection connection = new(con))
      {
        connection.Open();
        using (SqlCommand cmd = new("DeleteDepartment", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@DepartmentID", id);

          cmd.ExecuteNonQuery();
        }
      }
    }
  }
}
