﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employees_Api.Models
{
  public class Department
  {
    public int DepartmentID { get; set; }
    public required string DepartmentName { get; set; }
  }
}