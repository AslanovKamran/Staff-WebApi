GO
USE StaffDb

GO 
CREATE TABLE Users(
[Id] INT PRIMARY KEY IDENTITY,
[Login] NVARCHAR (100) UNIQUE NOT NULL,
[Password] NVARCHAR (255) NOT NULL,
[Salt] NVARCHAR (255) NOT NULL,
[RoleId] INT FOREIGN KEY REFERENCES Roles(Id) NOT NULL,
[CreatedAt] DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
[UpdatedAt] DATETIME2 NULL
)

GO
CREATE NONCLUSTERED INDEX IX_Users_Login
ON Users (Login);

----Trigger that will chnage UpdatedAt timestamp when the table is Updated

GO
CREATE TRIGGER trg_UpdateUsersTimestamp
ON Users
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET UpdatedAt = SYSDATETIME()
    FROM Users u
    INNER JOIN inserted i ON u.Id = i.Id;
END

GO
CREATE PROCEDURE GetUsers
AS
BEGIN
    SELECT 
        Users.Id,
        Users.Login,
        Users.Password,
        Users.Salt,
        Users.RoleId,
        Users.CreatedAt,
        Users.UpdatedAt,
        Roles.Id,
        Roles.Name
    FROM 
        Users
    LEFT JOIN 
        Roles ON Users.RoleId = Roles.Id
    ORDER BY 
        Users.Id;
END

GO
CREATE PROCEDURE GetUserById @Id INT
AS
BEGIN
    SELECT 
        Users.Id,
        Users.Login,
        Users.Password,
        Users.Salt,
        Users.RoleId,
        Users.CreatedAt,
        Users.UpdatedAt,
        Roles.Id,
        Roles.Name
    FROM 
        Users
    JOIN 
        Roles ON Users.RoleId = Roles.Id
		WHERE Users.Id = @Id
    ORDER BY 
        Users.Id;
END

GO
CREATE PROCEDURE AddUser
    @Login NVARCHAR(100),
    @Password NVARCHAR(255),
    @RoleId INT,
    @Salt NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

	-- Check if the login already exists
	IF EXISTS (SELECT 1 FROM Users WHERE Login = @Login)
    BEGIN
        RAISERROR('A user with this login already exists.', 16, 1);
        RETURN;
    END

    -- Check if the role with the given RoleId exists
    IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = @RoleId)
    BEGIN
        RAISERROR('Role with the given Id does not exist.', 16, 1);
        RETURN;
    END

    -- Insert the new user
    INSERT INTO Users (Login, Password, RoleId, Salt)
    VALUES (@Login, @Password, @RoleId, @Salt);

    -- Retrieve the newly inserted user with their role details
    SELECT 
        u.Id,
        u.Login,
        u.Password,
        u.Salt,
        u.RoleId,
        u.CreatedAt,
        u.UpdatedAt,
        r.Id,
        r.Name 
    FROM 
        Users u
    JOIN 
        Roles r ON u.RoleId = r.Id
    WHERE 
        u.Id = SCOPE_IDENTITY();
END


GO
CREATE PROCEDURE GetUserByLogin @Login NVARCHAR(100)
AS
BEGIN
    SELECT 
        Users.Id,
        Users.Login,
        Users.Password,
        Users.Salt,
        Users.RoleId,
        Users.CreatedAt,
        Users.UpdatedAt,
        Roles.Id,
        Roles.Name
    FROM 
        Users
    JOIN 
        Roles ON Users.RoleId = Roles.Id
		WHERE Users.Login = @Login COLLATE Latin1_General_CS_AS
    ORDER BY 
        Users.Id;
END


GO
CREATE PROC ChangeUserPassword
    @Login NVARCHAR(100), 
    @OldPassword NVARCHAR(255),
    @NewPassword NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the user exists with the provided login and old password
    IF EXISTS (
        SELECT 1 
        FROM [Users] 
        WHERE [Login] = @Login COLLATE Latin1_General_CS_AS 
        AND [Password] = @OldPassword COLLATE Latin1_General_CS_AS
    )
    BEGIN
        -- If user exists, update the password
        UPDATE [Users] 
        SET [Password] = @NewPassword, 
            [UpdatedAt] = SYSDATETIME() 
        WHERE [Login] = @Login COLLATE Latin1_General_CS_AS;
    END
    ELSE
    BEGIN
        -- If user does not exist, raise an error
        RAISERROR('Invalid login or old password.', 16, 1);
    END
END;





