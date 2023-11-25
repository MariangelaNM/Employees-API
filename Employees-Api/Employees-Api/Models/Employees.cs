﻿namespace Employees_Api.Models
{
  public class Employee
  {
    public int EmployeeID { get; set; }
    public required string EmployeeName { get; set; }
    public required string EmployeeLastName { get; set; }
    public int DepartmentID { get; set; }

    public required Department Department { get; set; }
  }
}