--GO
--USE master

--GO
--CREATE DATABASE StaffDb

--GO
--USE StaffDb


--GO
--CREATE TABLE Positions
--(
--[Id] INT PRIMARY KEY IDENTITY,
--[Title] NVARCHAR (255) NOT NULL UNIQUE,
--[Salary] DECIMAL (9,2) NOT NULL CHECK (Salary > 0)
--)


--GO
--CREATE PROCEDURE AddPosition 
--    @Title NVARCHAR(255), 
--    @Salary DECIMAL(9,2)
--AS
--BEGIN
--    BEGIN TRY
--        -- Attempt to insert the record into the Positions table
--        INSERT INTO Positions (Title, Salary)
--        OUTPUT inserted.Id, inserted.Title, inserted.Salary
--        VALUES (@Title, @Salary);
--    END TRY
--    BEGIN CATCH
--        -- Check for unique constraint violation
--        IF ERROR_NUMBER() IN (2627, 2601)
--        BEGIN
--            -- Return a custom error message for duplicate title
--            RAISERROR('A position with this title already exists.', 16, 1);
--        END
--        ELSE
--        BEGIN
--            -- Rethrow any other errors
--            THROW;
--        END
--    END CATCH
--END



--GO
--CREATE PROC GetPositions
--AS 
--BEGIN
--SELECT * FROM Positions
--ORDER BY Positions.Id
--END


--GO 
--CREATE PROC GetPositionById @Id INT
--AS
--BEGIN
--SELECT * FROM Positions WHERE Positions.Id = @Id
--END


--GO
--CREATE PROCEDURE UpdatePosition 
--    @Id INT, 
--    @Title NVARCHAR(255), 
--    @Salary DECIMAL(9,2)
--AS
--BEGIN
--    -- Update the record in the Positions table
--    UPDATE Positions 
--    SET Title = @Title, 
--        Salary = @Salary
--    WHERE Id = @Id;

--    -- Return the updated row
--    SELECT Id, Title, Salary 
--    FROM Positions 
--    WHERE Id = @Id;
--END

--GO
--CREATE PROC DeletePositionById @Id INT
--AS
--BEGIN 
--DELETE FROM Positions WHERE Positions.Id = @Id
--END

----POSITIONS BULK INSERT
--GO
--insert into Positions (Title, Salary) values (N'Operator', 7353.03);
--insert into Positions (Title, Salary) values (N'Nurse Practicioner', 9521.86);
--insert into Positions (Title, Salary) values (N'Computer Systems Analyst II', 7637.01);
--insert into Positions (Title, Salary) values (N'Chemical Engineer', 968.64);
--insert into Positions (Title, Salary) values (N'Food Chemist', 3326.94);
--insert into Positions (Title, Salary) values (N'Senior Quality Engineer', 9318.03);
--insert into Positions (Title, Salary) values (N'VP Product Management', 5087.23);
--insert into Positions (Title, Salary) values (N'General Manager', 6433.76);
--insert into Positions (Title, Salary) values (N'Teacher', 5662.26);
--insert into Positions (Title, Salary) values (N'Product Engineer', 7081.79);
























--GO
--CREATE TABLE People 
--(
--[Id] INT PRIMARY KEY IDENTITY,
--[Name] NVARCHAR(100) NOT NULL,
--[Surname] NVARCHAR(100) NOT NULL,
--[Phone] NVARCHAR(255) UNIQUE NOT NULL,
--[Email] NVARCHAR(255) UNIQUE NOT NULL ,
--[ImageUrl] NVARCHAR(255),
--[PositionId] INT FOREIGN KEY REFERENCES Positions(Id) NOT NULL
--)


----PEOPLE BULK INSERT
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Myrtie', 'Caukill', '222-570-2912', 'mcaukill0@flickr.com', null, 6);
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Jareb', 'Dutt', '275-475-8921', 'jdutt1@ow.ly', null, 1);
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Maureen', 'Hanlon', '387-907-8131', 'mhanlon2@newyorker.com', null, 5);
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Roseanne', 'Schuricht', '534-944-1381', 'rschuricht3@wired.com', null, 10);
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Minna', 'Dalton', '661-291-4406', 'mdalton4@simplemachines.org', null, 1);
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Leila', 'MacQueen', '700-193-7711', 'lmacqueen5@flickr.com', null, 6);
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Sibyl', 'Blemen', '178-274-7166', 'sblemen6@seattletimes.com', null, 8);
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Eyde', 'Blanko', '732-332-5001', 'eblanko7@jimdo.com', null, 3);
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Kettie', 'Hegg', '808-348-6052', 'khegg8@mapquest.com', null, 9);
--insert into People (Name, Surname, Phone, Email, ImageUrl, PositionId) values ('Kinnie', 'Wickey', '628-786-3449', 'kwickey9@mapy.cz', null, 4);



--GO
--CREATE PROCEDURE AddPerson
--    @Name NVARCHAR(100),
--    @Surname NVARCHAR(100),
--    @Phone NVARCHAR(255),
--    @Email NVARCHAR(255),
--    @ImageUrl NVARCHAR(255) = NULL, -- Allow null for ImageUrl
--    @PositionId INT
--AS
--BEGIN
--    SET NOCOUNT ON; -- Improves efficiency

--    BEGIN TRY
--        -- Check if the PositionId exists
--        IF NOT EXISTS (SELECT 1 FROM Positions WHERE Id = @PositionId)
--        BEGIN
--            RAISERROR ('Position with Id %d does not exist.', 16, 1, @PositionId);
--            RETURN;
--        END

--        -- Insert the new person
--        INSERT INTO People (Name, Surname, Phone, Email, ImageUrl, PositionId)
--        VALUES (@Name, @Surname, @Phone, @Email, @ImageUrl, @PositionId);

--        -- Get the Id of the newly inserted person
--        DECLARE @NewPersonId INT;
--        SET @NewPersonId = SCOPE_IDENTITY();

--        -- Select the newly added person along with their position details
--        SELECT 
--            p.Id,
--            p.Name,
--            p.Surname,
--            p.Phone,
--            p.Email,
--            p.ImageUrl,
--            p.PositionId,
--            pos.Id,
--            pos.Title,
--            pos.Salary
--        FROM 
--            People p
--        JOIN 
--            Positions pos ON p.PositionId = pos.Id
--        WHERE 
--            p.Id = @NewPersonId;
--    END TRY
--    BEGIN CATCH
--        -- Handle errors, like unique constraint violations
--        DECLARE @ErrorMessage NVARCHAR(4000);
--        DECLARE @ErrorSeverity INT;
--        DECLARE @ErrorState INT;

--        SELECT 
--            @ErrorMessage = ERROR_MESSAGE(),
--            @ErrorSeverity = ERROR_SEVERITY(),
--            @ErrorState = ERROR_STATE();

--        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
--    END CATCH
--END;




--GO
--CREATE PROCEDURE GetPeople 
--@Skip INT,  -- Number of rows to skip
--@Take INT	-- Number of rows to fetch
--AS 
--BEGIN
--    SELECT 
--        ppl.Id,
--        ppl.Name,
--        ppl.Surname,
--        ppl.Phone,
--        ppl.Email,
--        ppl.ImageUrl,
--		ppl.PositionId,
--        pos.Id,
--        pos.Title ,
--        pos.Salary
--    FROM 
--        People AS ppl
--    JOIN 
--        Positions AS pos ON ppl.PositionId = pos.Id
--    ORDER BY 
--        ppl.Id
--OFFSET @Skip ROWS
--FETCH NEXT @Take ROWS ONLY;
--END




--GO
--CREATE PROCEDURE GetPersonById
--    @Id INT
--AS 
--BEGIN
--    SELECT 
--        ppl.Id,
--        ppl.Name,
--        ppl.Surname,
--        ppl.Phone,
--        ppl.Email,
--        ppl.ImageUrl,
--        ppl.PositionId,
--        pos.Id,
--        pos.Title,
--        pos.Salary
--    FROM 
--        People AS ppl
--    JOIN 
--        Positions AS pos ON ppl.PositionId = pos.Id
--    WHERE 
--        ppl.Id = @Id;
--END


--GO
--CREATE PROCEDURE UpdatePerson 
--    @Id INT, 
--    @Name NVARCHAR(100), 
--    @Surname NVARCHAR(100), 
--    @Phone NVARCHAR(255), 
--    @Email NVARCHAR(255), 
--    @ImageUrl NVARCHAR(255), 
--    @PositionId INT
--AS
--BEGIN
--    -- Check if Person exists
--    IF EXISTS (SELECT 1 FROM People WHERE Id = @Id)
--    BEGIN
--        -- Check if the PositionId exists
--        IF EXISTS (SELECT 1 FROM Positions WHERE Id = @PositionId)
--        BEGIN
--            -- Update the person
--            UPDATE People
--            SET 
--                Name = @Name,
--                Surname = @Surname,
--                Phone = @Phone,
--                Email = @Email,
--                ImageUrl = @ImageUrl,
--                PositionId = @PositionId
--            WHERE Id = @Id;

--            -- Retrieve the updated person with the joined position
--            SELECT 
--                p.Id,          
--                p.Name, 
--                p.Surname, 
--                p.Phone, 
--                p.Email, 
--                p.ImageUrl, 
--                p.PositionId,  
--                pos.Id,         
--                pos.Title, 
--                pos.Salary
--            FROM People p
--            JOIN Positions pos ON p.PositionId = pos.Id
--            WHERE p.Id = @Id;
--        END
--        ELSE
--        BEGIN
--            -- Throw an error if the position does not exist
--            RAISERROR('Position with Id %d does not exist.', 16, 1, @PositionId);
--            RETURN;
--        END
--    END
--    ELSE
--    BEGIN
--        -- Throw an error if the person does not exist
--        RAISERROR('Person with Id %d does not exist.', 16, 1, @Id);
--        RETURN;
--    END
--END;





--GO
--CREATE PROC DeletePersonById @Id INT
--AS
--BEGIN 
--DELETE FROM People WHERE People.Id = @Id
--END


--GO
--CREATE PROC GetPeopleCount
--AS
--BEGIN
--SELECT COUNT(*) FROM People
--END


