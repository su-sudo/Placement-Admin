use CrudCoreDb;

GO
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    ProfilePic NVARCHAR(MAX),
    PasswordHash NVARCHAR(256) NOT NULL
);

GO

CREATE PROCEDURE AddUser
    @Name NVARCHAR(100),
    @DateOfBirth DATE,
    @ProfilePic NVARCHAR(MAX),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    BEGIN TRY
        INSERT INTO Users (Name, DateOfBirth, ProfilePic, PasswordHash)
        VALUES (@Name, @DateOfBirth, @ProfilePic, @PasswordHash);
    END TRY
    BEGIN CATCH
       
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        SELECT @ErrorMessage AS ErrorMessage;
    END CATCH;
END;


GO

ALTER PROCEDURE ReadAllUser

AS
BEGIN
BEGIN TRY
	SELECT Id,Name,DateOfBirth,ProfilePic from Users;
END TRY
    BEGIN CATCH
       
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        SELECT @ErrorMessage AS ErrorMessage;
    END CATCH;
END;

GO

CREATE PROCEDURE DeleteUser
    @Id INT
AS
BEGIN
    BEGIN TRY
        DELETE FROM Users WHERE id = @Id;
    END TRY
    BEGIN CATCH
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

ALTER PROCEDURE EditUser(
    @Id INT,
    @Name NVARCHAR(100) = NULL,
    @DateOfBirth DATE = NULL,
    @ProfilePic NVARCHAR(MAX) = NULL
)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE Users
        SET 
            Name = COALESCE(@Name, Name),
            DateOfBirth = COALESCE(@DateOfBirth, DateOfBirth),
            ProfilePic = COALESCE(@ProfilePic, ProfilePic)
        WHERE Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;


GO

CREATE PROCEDURE GetById(@Id INT)
AS
BEGIN
    BEGIN TRY
        SELECT Id, Name, DateOfBirth, ProfilePic
        FROM Users
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;


select * from Users;