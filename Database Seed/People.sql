GO
USE StaffDb


GO
CREATE TABLE People 
(
[Id] INT PRIMARY KEY IDENTITY,
[Name] NVARCHAR(100) NOT NULL,
[Surname] NVARCHAR(100) NOT NULL,
[Phone] NVARCHAR(255) UNIQUE NOT NULL,
[Email] NVARCHAR(255) UNIQUE NOT NULL ,
[ImageUrl] NVARCHAR(255),
[PositionId] INT FOREIGN KEY REFERENCES Positions(Id) NOT NULL
)


GO
CREATE PROCEDURE GetPeople 
@Skip INT,  -- Number of rows to skip
@Take INT	-- Number of rows to fetch
AS 
BEGIN
    SELECT 
        ppl.Id,
        ppl.Name,
        ppl.Surname,
        ppl.Phone,
        ppl.Email,
        ppl.ImageUrl,
		ppl.PositionId,
        pos.Id,
        pos.Title ,
        pos.Salary
    FROM 
        People AS ppl
    JOIN 
        Positions AS pos ON ppl.PositionId = pos.Id
    ORDER BY 
        ppl.Id
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY;
END




GO
CREATE PROCEDURE GetPersonById
    @Id INT
AS 
BEGIN
    SELECT 
        ppl.Id,
        ppl.Name,
        ppl.Surname,
        ppl.Phone,
        ppl.Email,
        ppl.ImageUrl,
        ppl.PositionId,
        pos.Id,
        pos.Title,
        pos.Salary
    FROM 
        People AS ppl
    JOIN 
        Positions AS pos ON ppl.PositionId = pos.Id
    WHERE 
        ppl.Id = @Id;
END

GO
CREATE PROCEDURE AddPerson
    @Name NVARCHAR(100),
    @Surname NVARCHAR(100),
    @Phone NVARCHAR(255),
    @Email NVARCHAR(255),
    @ImageUrl NVARCHAR(255) = NULL, -- Allow null for ImageUrl
    @PositionId INT
AS
BEGIN
    SET NOCOUNT ON; -- Improves efficiency

    BEGIN TRY
        -- Check if the PositionId exists
        IF NOT EXISTS (SELECT 1 FROM Positions WHERE Id = @PositionId)
        BEGIN
            RAISERROR ('Position with Id %d does not exist.', 16, 1, @PositionId);
            RETURN;
        END
        -- Insert the new person
        INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId)
        VALUES (@Name, @Surname, @Phone, @Email, @ImageUrl, @PositionId);

        -- Get the Id of the newly inserted person
        DECLARE @NewPersonId INT;
        SET @NewPersonId = SCOPE_IDENTITY();

        -- Select the newly added person along with their position details
        SELECT 
            p.Id,
            p.Name,
            p.Surname,
            p.Phone,
            p.Email,
            p.ImageUrl,
            p.PositionId,
            pos.Id,
            pos.Title,
            pos.Salary
        FROM 
            People p
        JOIN 
            Positions pos ON p.PositionId = pos.Id
        WHERE 
            p.Id = @NewPersonId;
    END TRY
    BEGIN CATCH
        -- Handle errors, like unique constraint violations
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;

GO
CREATE PROCEDURE UpdatePerson 
    @Id INT, 
    @Name NVARCHAR(100), 
    @Surname NVARCHAR(100), 
    @Phone NVARCHAR(255), 
    @Email NVARCHAR(255), 
    @ImageUrl NVARCHAR(255), 
    @PositionId INT
AS
BEGIN
    -- Check if Person exists
    IF EXISTS (SELECT 1 FROM People WHERE Id = @Id)
    BEGIN
        -- Check if the PositionId exists
        IF EXISTS (SELECT 1 FROM Positions WHERE Id = @PositionId)
        BEGIN
            -- Update the person
            UPDATE People
            SET 
                Name = @Name,
                Surname = @Surname,
                Phone = @Phone,
                Email = @Email,
                ImageUrl = @ImageUrl,
                PositionId = @PositionId
            WHERE Id = @Id;

            -- Retrieve the updated person with the joined position
            SELECT 
                p.Id,          
                p.Name, 
                p.Surname, 
                p.Phone, 
                p.Email, 
                p.ImageUrl, 
                p.PositionId,  
                pos.Id,         
                pos.Title, 
                pos.Salary
            FROM People p
            JOIN Positions pos ON p.PositionId = pos.Id
            WHERE p.Id = @Id;
        END
        ELSE
        BEGIN
            -- Throw an error if the position does not exist
            RAISERROR('Position with Id %d does not exist.', 16, 1, @PositionId);
            RETURN;
        END
    END
    ELSE
    BEGIN
        -- Throw an error if the person does not exist
        RAISERROR('Person with Id %d does not exist.', 16, 1, @Id);
        RETURN;
    END
END;
GO
CREATE PROC DeletePersonById @Id INT
AS
BEGIN 
DELETE FROM People WHERE People.Id = @Id
END
GO
CREATE PROC GetPeopleCount
AS
BEGIN
SELECT COUNT(*) FROM People
END




GO
--PEOPLE BULK INSERT (Make sure that the positions are inserted in advance)
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Myrtie', 'Caukill', '222-570-2912', 'mcaukill0@flickr.com', null, 6);
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Jareb', 'Dutt', '275-475-8921', 'jdutt1@ow.ly', null, 1);
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Maureen', 'Hanlon', '387-907-8131', 'mhanlon2@newyorker.com', null, 5);
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Roseanne', 'Schuricht', '534-944-1381', 'rschuricht3@wired.com', null, 10);
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Minna', 'Dalton', '661-291-4406', 'mdalton4@simplemachines.org', null, 1);
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Leila', 'MacQueen', '700-193-7711', 'lmacqueen5@flickr.com', null, 6);
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Sibyl', 'Blemen', '178-274-7166', 'sblemen6@seattletimes.com', null, 8);
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Eyde', 'Blanko', '732-332-5001', 'eblanko7@jimdo.com', null, 3);
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Kettie', 'Hegg', '808-348-6052', 'khegg8@mapquest.com', null, 9);
INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId) VALUES ('Kinnie', 'Wickey', '628-786-3449', 'kwickey9@mapy.cz', null, 4);