USE [EmployeesDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author: Mariangela
-- Purpose: Get the list of types Departments
-- =============================================
CREATE PROCEDURE [dbo].[GetDepartment]
AS
BEGIN
    BEGIN TRY
        -- Select department information
        SELECT [DepartmentID]
             , [DepartmentName]
        FROM [EmployeesDB].[dbo].[Departments]
    END TRY
    BEGIN CATCH
        -- Log or handle the error as needed
        INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (ERROR_MESSAGE(), GETDATE())
        THROW;
    END CATCH
END

-- =============================================
-- Author: Mariangela
-- Purpose: Add the types Departments
-- =============================================
CREATE PROCEDURE [dbo].[PostDepartment]
    @DepartmentName varchar(50)
AS
BEGIN
    BEGIN TRY
     INSERT INTO [dbo].[Departments]
           ([DepartmentName])
     VALUES
           (@DepartmentName)
    END TRY
    BEGIN CATCH
        -- Log or handle the error as needed
        INSERT INTO ErrorLog (ErrorMessage, ErrorTime) VALUES (ERROR_MESSAGE(), GETDATE())
        THROW;
    END CATCH
END