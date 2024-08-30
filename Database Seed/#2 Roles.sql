GO
USE StaffDb

GO
CREATE TABLE Roles (
[Id] INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(100) UNIQUE NOT NULL
)

GO 
CREATE PROCEDURE GetRoles
AS
BEGIN
 SET NOCOUNT ON;

    -- Retrieve all roles
    SELECT Id, Name
    FROM Roles
	ORDER BY Roles.Id
END

GO
CREATE PROCEDURE GetRoleById @Id INT
AS
BEGIN
SET NOCOUNT ON;

    -- Retrieve a role by id
    SELECT Id, Name
    FROM Roles
	WHERE Id = @Id
END

GO
CREATE PROCEDURE AddRole
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the role name already exists
    IF EXISTS (SELECT 1 FROM Roles WHERE Name = @Name)
    BEGIN
        RAISERROR ('A role with this name already exists.', 16, 1);
        RETURN;
    END

    -- Try to insert the new role
    BEGIN TRY
        INSERT INTO Roles (Name)
        OUTPUT INSERTED.Id, INSERTED.Name
        VALUES (@Name);
    END TRY
    BEGIN CATCH
        -- Handle other potential errors
        RAISERROR ('An error occurred while inserting the role: %s', 16, 1);
    END CATCH
END

GO
CREATE PROCEDURE UpdateRole
    @Id INT,
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the new role name already exists
    IF EXISTS (SELECT 1 FROM Roles WHERE Name = @Name AND Id != @Id)
    BEGIN
        RAISERROR ('A role with this name already exists.', 16, 1);
        RETURN;
    END

    -- Ensure the role with the given Id exists
    IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @Id)
    BEGIN
        RAISERROR ('Role with the given Id does not exist.', 16, 1);
        RETURN;
    END

    -- Try to update the role name
    BEGIN TRY
        UPDATE Roles
        SET Name = @Name
        WHERE Id = @Id;

        -- Check if the update was successful
        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR ('An error occurred while updating the role.', 16, 1);
        END
        ELSE
        BEGIN
            -- Return the updated role data
            SELECT Id, Name
            FROM Roles
            WHERE Id = @Id;
        END
    END TRY
    BEGIN CATCH
        -- Handle other potential errors
        THROW;
    END CATCH
END

GO
CREATE PROCEDURE DeleteRoleById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the role is referenced in the Users table
    IF EXISTS (SELECT 1 FROM Users WHERE RoleId = @Id)
    BEGIN
        -- Raise an error if the role is being referenced
        RAISERROR ('Cannot delete the role because it is referenced by one or more users.', 16, 1);
        RETURN;
    END

    -- Proceed with deletion if no references are found
    DELETE FROM Roles
    WHERE Id = @Id;

    -- Optionally, you can check if the role was deleted and raise an error if not
    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR ('Role not found or could not be deleted.', 16, 1);
    END
END


--ROLES BULK INSERT
GO
INSERT INTO Roles (Name) VALUES (N'Admin')
INSERT INTO Roles (Name) VALUES (N'User')
INSERT INTO Roles (Name) VALUES (N'Guest')