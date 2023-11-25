-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = 'EmployeesDB')
BEGIN
    CREATE DATABASE EmployeesDB;
    PRINT 'EmployeesDB created successfully.';
END
ELSE
BEGIN
    PRINT 'EmployeesDB already exists.';
END
GO

-- Use the newly created or existing database
USE EmployeesDB;
GO

-- Create the departments table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Departments')
BEGIN
    CREATE TABLE Departments (
        DepartmentID INT PRIMARY KEY IDENTITY(1,1),
        DepartmentName VARCHAR(50) NOT NULL
    );
    PRINT 'Departments table created successfully.';
END
ELSE
BEGIN
    PRINT 'Departments table already exists.';
END
GO

-- Insert sample data into the departments table
INSERT INTO Departments (DepartmentName)
VALUES
    ('Development'),
    ('Human Resources');

-- Create the employees table with a foreign key referencing the departments table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Employees')
BEGIN
    CREATE TABLE Employees (
        EmployeeID INT PRIMARY KEY IDENTITY(1,1),
        EmployeeName VARCHAR(50) NOT NULL,
        EmployeeLastName VARCHAR(250) NOT NULL,
        DepartmentID INT,
        FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
    );
    PRINT 'Employees table created successfully.';
END
ELSE
BEGIN
    PRINT 'Employees table already exists.';
END
GO

-- Insert sample data into the employees table
INSERT INTO Employees (EmployeeName, EmployeeLastName, DepartmentID)
VALUES
    ('Juan', 'Alvarado', 1),
    ('Ana', 'Alvarado', 2);

-- Example query to retrieve master-detail information
SELECT
    e.EmployeeID,
    e.EmployeeName,
    e.EmployeeLastName,
    d.DepartmentID,
    d.DepartmentName
FROM
    Departments d
JOIN
    Employees e ON d.DepartmentID = e.DepartmentID;
