GO
USE StaffDb

CREATE TABLE RefreshTokens
(
[Id] INT PRIMARY KEY IDENTITY,
[Token] NVARCHAR(255) NOT NULL,
[Expires] DATETIME2 NOT NULL,
[UserId] INT REFERENCES Users(Id) NOT NULL
)


GO
CREATE PROC AddRefreshToken 
    @Token NVARCHAR(255), 
    @Expires DATETIME2, 
    @UserId INT
AS
BEGIN
    INSERT INTO RefreshTokens ([Token], [Expires], [UserId]) 
    VALUES (@Token, @Expires, @UserId)
END


 

GO
 CREATE PROC GetUserByRefreshToken  @Token NVARCHAR(255)
 AS
 BEGIN
 SELECT rt.*, us.*,rl.* FROM RefreshTokens AS rt
 JOIN Users AS us ON us.Id = rt.UserId
 JOIN Roles AS rl ON rl.Id = us.RoleId
 WHERE rt.Token = @Token
 END

GO
CREATE PROC DeleteRefreshToken @Token NVARCHAR(255)
 AS
 BEGIN
 DELETE FROM RefreshTokens WHERE RefreshTokens.Token = @Token
 END


GO
CREATE PROC DeleteUserRefreshTokens @UserId INT
AS
BEGIN
    DELETE FROM RefreshTokens WHERE UserId = @UserId
END