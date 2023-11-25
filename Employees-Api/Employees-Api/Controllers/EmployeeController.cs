using Employees_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Employees_Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class  EmployeeController : ControllerBase
  {
    public readonly string con;
    public  EmployeeController(IConfiguration configuration)
    {

      con = configuration["ConnectionStrings:conexion"];
    }

    [HttpGet]
    public IEnumerable< Employee> Get()
    {
      List< Employee>  Employees = new();

      using (SqlConnection connection = new(con))
      {
        connection.Open();
        using (SqlCommand cmd = new("GetEmployees", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          using (SqlDataReader reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
               Employee p = new  Employee
              {
                 EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                 EmployeeName = reader["EmployeeName"].ToString(),
                 EmployeeLastName = reader["EmployeeLastName"].ToString(),
                 DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
               };

               Employees.Add(p);
            }
          }
        }
      }
      return  Employees;
    }

    [HttpPost]
    public void Post([FromBody] Employee employee)
    {
      using (SqlConnection connection = new SqlConnection(con))
      {
        connection.Open();
        using (SqlCommand cmd = new SqlCommand("InsertEmployee", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
          cmd.Parameters.AddWithValue("@EmployeeLastName", employee.EmployeeLastName);
          cmd.Parameters.AddWithValue("@DepartmentID", employee.DepartmentID);
          cmd.ExecuteNonQuery();
        }
      }
    }



    [HttpPut("{id}")]
    public void Put([FromBody] Employee employee, int id)
    {
      using (SqlConnection connection = new SqlConnection(con))
      {
        connection.Open();
        using (SqlCommand cmd = new SqlCommand("UpdateEmployee", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@EmployeeID", id);
          cmd.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
          cmd.Parameters.AddWithValue("@EmployeeLastName", employee.EmployeeLastName);
          cmd.Parameters.AddWithValue("@DepartmentID", employee.DepartmentID);
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
        using (SqlCommand cmd = new("DeleteEmployee", connection))
        {
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@EmployeeID", id);

          cmd.ExecuteNonQuery();
        }
      }
    }
  }
}
