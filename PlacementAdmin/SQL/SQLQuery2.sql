use PlacementAdmin;

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
	Email NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
	DateOfBirth DATE NOT NULL,
	Gender NVARCHAR(20) NOT NULL,
	profilePicture NVARCHAR(MAX) NOT NULL,
	CourseStreamId INT ,FOREIGN KEY(CourseStreamId) REFERENCES CourseStreams(Id),
    Role NVARCHAR(20) NOT NULL

);

Drop table Users;
GO

CREATE TABLE CourseStreams (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500)
);


GO
ALTER TABLE Users
ADD ProfilePicture NVARCHAR(MAX) NULL;

GO
ALTER PROCEDURE AddUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(255),
    @Email NVARCHAR(100),
    @DateOfBirth DATE,
    @Gender NVARCHAR(20),
    @ProfilePicture NVARCHAR(MAX),
    @CourseStreamId INT,
    @Role NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT INTO Users (Username, Password, Email, DateOfBirth, Gender, ProfilePicture, CourseStreamId, Role)
        VALUES (@Username, @Password, @Email, @DateOfBirth, @Gender, @ProfilePicture, @CourseStreamId, @Role);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

GO
CREATE PROCEDURE GetUserById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Users WHERE Id = @UserId;
END


GO
EXEC AddUser
    @Username = 'JohnDoe',
    @Password = 'password123',
    @Email = 'johndoe@example.com',
    @DateOfBirth = '1990-01-01',
    @Gender = 'Male',
    @ProfilePicture = 'profilepic.jpg',
    @CourseStreamId = 1,
    @Role = 'User';

GO

CREATE PROCEDURE GetAvailableCourseStreams
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		SELECT * From CourseStreams;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END
GO

use PlacementAdmin;
INSERT INTO [PlacementAdmin].[dbo].[Users] ([Username], [Password], [Email], [Role])
VALUES ('admin1', '123', 'admin@gmail.com', 'Admin');

INSERT INTO CourseStreams (Name)values('');

SELECT * FROM CourseStreams;
GO
EXEC GetAvailableCourseStreams;

GO
SELECT * from Users;
