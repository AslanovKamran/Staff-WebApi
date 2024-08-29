GO
USE master

GO
CREATE DATABASE StaffDb

GO
USE StaffDb


GO
CREATE TABLE Positions
(
[Id] INT PRIMARY KEY IDENTITY,
[Title] NVARCHAR (255) NOT NULL UNIQUE,
[Salary] DECIMAL (9,2) NOT NULL CHECK (Salary > 0)
)


GO
CREATE PROC GetPositions
AS 
BEGIN
SELECT * FROM Positions
ORDER BY Positions.Id
END


GO 
CREATE PROC GetPositionById @Id INT
AS
BEGIN
SELECT * FROM Positions WHERE Positions.Id = @Id
END

GO
CREATE PROCEDURE AddPosition 
    @Title NVARCHAR(255), 
    @Salary DECIMAL(9,2)
AS
BEGIN
    BEGIN TRY
        -- Attempt to insert the record into the Positions table
        INSERT INTO Positions (Title, Salary)
        OUTPUT inserted.Id, inserted.Title, inserted.Salary
        VALUES (@Title, @Salary);
    END TRY
    BEGIN CATCH
        -- Check for unique constraint violation
        IF ERROR_NUMBER() IN (2627, 2601)
        BEGIN
            -- Return a custom error message for duplicate title
            RAISERROR('A position with this title already exists.', 16, 1);
        END
        ELSE
        BEGIN
            -- Rethrow any other errors
            THROW;
        END
    END CATCH
END

GO
CREATE PROCEDURE UpdatePosition 
    @Id INT, 
    @Title NVARCHAR(255), 
    @Salary DECIMAL(9,2)
AS
BEGIN
    -- Update the record in the Positions table
    UPDATE Positions 
    SET Title = @Title, 
        Salary = @Salary
    WHERE Id = @Id;

    -- Return the updated row
    SELECT Id, Title, Salary 
    FROM Positions 
    WHERE Id = @Id;
END


GO
CREATE PROC DeletePositionById @Id INT
AS
BEGIN 
DELETE FROM Positions WHERE Positions.Id = @Id
END


--POSITIONS BULK INSERT
GO
INSERT INTO Positions (Title, Salary) VALUES (N'Operator', 7353.03);
INSERT INTO Positions (Title, Salary) VALUES (N'Nurse Practicioner', 9521.86);
INSERT INTO Positions (Title, Salary) VALUES (N'Computer Systems Analyst II', 7637.01);
INSERT INTO Positions (Title, Salary) VALUES (N'Chemical Engineer', 968.64);
INSERT INTO Positions (Title, Salary) VALUES (N'Food Chemist', 3326.94);
INSERT INTO Positions (Title, Salary) VALUES (N'Senior Quality Engineer', 9318.03);
INSERT INTO Positions (Title, Salary) VALUES (N'VP Product Management', 5087.23);
INSERT INTO Positions (Title, Salary) VALUES (N'General Manager', 6433.76);
INSERT INTO Positions (Title, Salary) VALUES (N'Teacher', 5662.26);
INSERT INTO Positions (Title, Salary) VALUES (N'Product Engineer', 7081.79);