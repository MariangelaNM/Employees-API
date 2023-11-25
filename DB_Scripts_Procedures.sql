USE [EmployeesDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author: Mariangela
-- Purpose: Get the list of Departments
-- =============================================
CREATE PROCEDURE [dbo].[GetDepartments]
AS
BEGIN
    BEGIN TRY
        -- Select department information
        SELECT [DepartmentID], [DepartmentName]
        FROM [dbo].[Departments];
    END TRY
    BEGIN CATCH
        -- Log or handle the error as needed
        INSERT INTO [dbo].[ErrorLog] ([ErrorMessage], [ErrorTime])
        VALUES (ERROR_MESSAGE(), GETDATE());
        THROW;
    END CATCH
END
go

-- =============================================
-- Author: Mariangela
-- Purpose: Add a new Department
-- =============================================
CREATE PROCEDURE [dbo].[AddDepartment](
    @DepartmentName VARCHAR(50))
AS
BEGIN
    BEGIN TRY
        INSERT INTO [dbo].[Departments] ([DepartmentName])
        VALUES (@DepartmentName);
    END TRY
    BEGIN CATCH
        -- Log or handle the error as needed
        INSERT INTO [dbo].[ErrorLog] ([ErrorMessage], [ErrorTime])
        VALUES (ERROR_MESSAGE(), GETDATE());
        THROW
    END CATCH
END
go
-- =============================================
-- Author: Mariangela
-- Purpose: Update Department 
-- =============================================
CREATE PROCEDURE [dbo].[UpdateDepartment]
    @DepartmentID INT,
    @NewDepartmentName VARCHAR(50)
AS
BEGIN
    BEGIN TRY
        -- Update the department name
        UPDATE [dbo].[Departments]
        SET [DepartmentName] = @NewDepartmentName
        WHERE [DepartmentID] = @DepartmentID;
    END TRY
    BEGIN CATCH
        -- Log the error in the ErrorLog table
        INSERT INTO [dbo].[ErrorLog] ([ErrorMessage], [ErrorTime])
        VALUES (ERROR_MESSAGE(), GETDATE());

        -- Re-throw the error for further handling
        THROW;
    END CATCH
END
go
-- =============================================
-- Author: Mariangela
-- Purpose: Delete a Department
-- =============================================
CREATE PROCEDURE [dbo].[DeleteDepartment]
    @DepartmentID INT
AS
BEGIN
    BEGIN TRY
        -- Delete the department
        DELETE FROM [dbo].[Departments]
        WHERE [DepartmentID] = @DepartmentID;
    END TRY
    BEGIN CATCH
        -- Log the error in the ErrorLog table
        INSERT INTO [dbo].[ErrorLog] ([ErrorMessage], [ErrorTime])
        VALUES (ERROR_MESSAGE(), GETDATE());

        -- Re-throw the error for further handling
        THROW;
    END CATCH
END
go
-- =============================================
-- Author: Mariangela
-- Purpose: Add a new Employee
-- =============================================
CREATE PROCEDURE [dbo].[InsertEmployee]
    @EmployeeName VARCHAR(50),
    @EmployeeLastName VARCHAR(250),
    @DepartmentID INT
AS
BEGIN
    BEGIN TRY
        INSERT INTO [dbo].[Employees] (EmployeeName, EmployeeLastName, DepartmentID)
        VALUES (@EmployeeName, @EmployeeLastName, @DepartmentID);
    END TRY
    BEGIN CATCH
        -- Log the error in the ErrorLog table
        INSERT INTO [dbo].[ErrorLog] ([ErrorMessage], [ErrorTime])
        VALUES (ERROR_MESSAGE(), GETDATE());

        -- Re-throw the error for further handling
        THROW;
    END CATCH
END
go
-- =============================================
-- Author: Mariangela
-- Purpose: Get Employees
-- =============================================
CREATE PROCEDURE [dbo].[GetEmployees]
AS
BEGIN
    BEGIN TRY
        SELECT * FROM [dbo].[Employees];
    END TRY
    BEGIN CATCH
        -- Log or handle the error as needed
        INSERT INTO [dbo].[ErrorLog] ([ErrorMessage], [ErrorTime])
        VALUES (ERROR_MESSAGE(), GETDATE());
        THROW;
    END CATCH
END
go
-- =============================================
-- Author: Mariangela
-- Purpose: Update Employee
-- =============================================
CREATE PROCEDURE [dbo].[UpdateEmployee]
    @EmployeeID INT,
    @EmployeeName VARCHAR(50),
    @EmployeeLastName VARCHAR(250),
    @DepartmentID INT
AS
BEGIN
    BEGIN TRY
        UPDATE [dbo].[Employees]
        SET [EmployeeName] = @EmployeeName,
            [EmployeeLastName] = @EmployeeLastName,
            [DepartmentID] = @DepartmentID
        WHERE [EmployeeID] = @EmployeeID;
    END TRY
    BEGIN CATCH
        -- Log or handle the error as needed
        INSERT INTO [dbo].[ErrorLog] ([ErrorMessage], [ErrorTime])
        VALUES (ERROR_MESSAGE(), GETDATE());
        THROW;
    END CATCH
END
go
-- =============================================
-- Author: Mariangela
-- Purpose: Delete Employee
-- =============================================
CREATE PROCEDURE [dbo].[DeleteEmployee]
    @EmployeeID INT
AS
BEGIN
    BEGIN TRY
        DELETE FROM [dbo].[Employees]
        WHERE [EmployeeID] = @EmployeeID;
    END TRY
    BEGIN CATCH
        -- Log or handle the error as needed
        INSERT INTO [dbo].[ErrorLog] ([ErrorMessage], [ErrorTime])
        VALUES (ERROR_MESSAGE(), GETDATE());
        THROW;
    END CATCH
END;
go