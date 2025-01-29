CREATE PROCEDURE AddTest
    @TestName NVARCHAR(100),
    @CourseStreamId INT,
    @CreatedDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NewTestId INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO Tests (TestName, CourseStreamId, CreatedDate)
        VALUES (@TestName, @CourseStreamId, @CreatedDate, @);

        SET @NewTestId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT @NewTestId AS NewTestId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

GO


CREATE PROCEDURE AddQuestionsToTest
    @TestId INT,
    @QuestionIds TestQuestionsTVP READONLY -- TVP
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TestQuestions (TestId, QuestionId)
        SELECT @TestId, QuestionId
        FROM @QuestionIds;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END


GO

CREATE PROCEDURE AssignTestToStudents
    @TestId INT,
    @UserIds TestUserTVP READONLY, -- Assuming TestUserTVP is a TVP for user IDs
    @AssignedDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO StudentTests (UserId, TestId, AssignedDate, Completed)
        SELECT UserId, @TestId, @AssignedDate, 0
        FROM @UserIds;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

GO

--**********************************
--TVP
--*************************************

CREATE TYPE TestQuestionsTVP AS TABLE
(
    QuestionId INT
);

GO

CREATE TYPE TestUserTVP AS TABLE
(
    UserId INT
);

CREATE TYPE StudentAnswersTVP AS TABLE
(
    UserId INT,
    TestId INT,
    QuestionId INT,
    SelectedOptionId INT,
    AnswerText NVARCHAR(MAX)
);


--********************************************************
--STUDENT ANSER TO TABLE
--***********************************************************

CREATE PROCEDURE BulkInsertStudentAnswers
    @StudentAnswers StudentAnswersTVP READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO StudentAnswers (UserId, TestId, QuestionId, SelectedOptionId, AnswerText)
        SELECT UserId, TestId, QuestionId, SelectedOptionId, AnswerText
        FROM @StudentAnswers;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

GO
USE PlacementAdmin;
GO
CREATE PROCEDURE GetIncompleteTests
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.TestName, 
        cs.Name as 'StreamName', 
        COUNT(st.StudentTestId) as 'StudentCount',
        FORMAT(t.CreatedDate, 'dd/MM/yyyy') as 'DateOfCreation'
    FROM 
        Tests t
    JOIN 
        CourseStreams cs ON cs.CourseStreamId = t.CourseStreamId
    JOIN 
        StudentTests st ON st.TestId = t.TestId
    WHERE 
        st.Completed = 0
    GROUP BY 
        t.TestName, 
        cs.Name,
        t.CreatedDate;
END

GO

GO
use PlacementAdmin;
GO
CREATE PROCEDURE GetQuestionsByCriteria
    @CourseStream NVARCHAR(50) = NULL,
    @DifficultyLevel NVARCHAR(50) = NULL,
    @QuestionType NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        QuestionId,
        QuestionText,
        QuestionType,
        CourseStreamId,
        DifficultyLevel
    FROM 
        QuestionBank
    WHERE 
        (@CourseStream IS NULL OR CourseStreamId = @CourseStream)
        AND (@DifficultyLevel IS NULL OR DifficultyLevel = @DifficultyLevel)
        AND (@QuestionType IS NULL OR QuestionType = @QuestionType);
END

GO
CREATE PROCEDURE GetAllTests
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        TestId,
        TestName,
        CourseStreamId,
        CreatedDate
    FROM 
        Tests;
END

GO

ALTER PROCEDURE GetStudentsByCourseStream
    @CourseStreamId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id AS UserId,
        Username AS FullName,
        Email,
        CourseStreamId
    FROM 
        Users
    WHERE 
        CourseStreamId = @CourseStreamId
        AND Role = 'Student';
END


GO
CREATE PROCEDURE GetTestsByCourseStream
    @CourseStreamId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        TestId,
        TestName,
        CourseStreamId,
        CreatedDate
    FROM 
        Tests
    WHERE 
        CourseStreamId = @CourseStreamId;
END

GO

ALTER PROCEDURE [dbo].[GetStudentTestResults]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        sa.UserId,
        u.Username,
        sa.TestId,
        t.TestName,
        t.CreatedDate,
        COUNT(sa.QuestionId) AS TotalQuestions,
        SUM(CASE WHEN o.IsCorrect = 1 THEN 1 ELSE 0 END) AS CorrectAnswers,
        (SUM(CASE WHEN o.IsCorrect = 1 THEN 1 ELSE 0 END) * 100.0 / COUNT(sa.QuestionId)) AS ScorePercentage
    FROM 
        StudentAnswers sa
        INNER JOIN Users u ON sa.UserId = u.Id
        INNER JOIN Tests t ON sa.TestId = t.TestId
        INNER JOIN Options o ON sa.QuestionId = o.QuestionId AND sa.SelectedOptionId = o.OptionId
    WHERE 
        sa.UserId = @UserId
    GROUP BY 
        sa.UserId, u.Username, sa.TestId, t.TestName, t.CreatedDate
    ORDER BY 
        t.CreatedDate DESC;
END


GO

SELECT sa.UserId, sa.TestId, sa.QuestionId, sa.SelectedOptionId, o.IsCorrect FROM 
StudentAnswers sa INNER JOIN Options o ON sa.QuestionId = o.QuestionId AND 
sa.SelectedOptionId = o.OptionId WHERE sa.UserId = '3';

SELECT * FROM Users;
SELECT * From Options;
SELECT * from QuestionBank;
SELECT * FROM StudentAnswers;


DBCC CHECKIDENT ('Tests', RESEED, 1);


GO

-- Inserting Questions into QuestionBank
INSERT INTO QuestionBank (QuestionText, QuestionType, CourseStreamId, DifficultyLevel)
VALUES 
('What does HTML stand for?', 'MCQ', 1, 'Easy'),
('Which language is used for styling web pages?', 'MCQ', 1, 'Easy'),
('Which is not a programming language?', 'MCQ', 1, 'Easy'),
('What does SQL stand for?', 'MCQ', 1, 'Easy'),
('Which language is used for web development?', 'MCQ', 1, 'Easy'),
('Which of the following is a database management system?', 'MCQ', 1, 'Easy'),
('Which company developed the Java language?', 'MCQ', 1, 'Easy'),
('What is the name of the first web browser?', 'MCQ', 1, 'Easy'),
('What does HTTP stand for?', 'MCQ', 1, 'Easy'),
('Which of the following is a search engine?', 'MCQ', 1, 'Easy');

-- Get the New Question IDs (assuming sequential IDs for simplicity)
-- Note: This step is illustrative, you would fetch the IDs dynamically in an actual implementation

-- Example Question IDs (assuming they are from 1 to 10)
DECLARE @questionId1 INT = 1, @questionId2 INT = 2, @questionId3 INT = 3, @questionId4 INT = 4, 
        @questionId5 INT = 5, @questionId6 INT = 6, @questionId7 INT = 7, @questionId8 INT = 8, 
        @questionId9 INT = 9, @questionId10 INT = 10;

-- Inserting Options
-- For Question 1
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId1, 'Hyper Text Markup Language', 1),
(@questionId1, 'High Text Markup Language', 0),
(@questionId1, 'Hyperlinks and Text Markup Language', 0),
(@questionId1, 'Home Tool Markup Language', 0);

-- For Question 2
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId2, 'CSS', 1),
(@questionId2, 'HTML', 0),
(@questionId2, 'XML', 0),
(@questionId2, 'JavaScript', 0);

-- For Question 3
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId3, 'HTML', 1),
(@questionId3, 'Python', 0),
(@questionId3, 'Java', 0),
(@questionId3, 'C++', 0);

-- For Question 4
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId4, 'Structured Query Language', 1),
(@questionId4, 'Structured Question Language', 0),
(@questionId4, 'Simple Query Language', 0),
(@questionId4, 'Strong Question Language', 0);

-- For Question 5
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId5, 'JavaScript', 1),
(@questionId5, 'Python', 0),
(@questionId5, 'C', 0),
(@questionId5, 'C#', 0);

-- For Question 6
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId6, 'MySQL', 1),
(@questionId6, 'Windows', 0),
(@questionId6, 'Linux', 0),
(@questionId6, 'MacOS', 0);

-- For Question 7
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId7, 'Sun Microsystems', 1),
(@questionId7, 'Microsoft', 0),
(@questionId7, 'Google', 0),
(@questionId7, 'Apple', 0);

-- For Question 8
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId8, 'WorldWideWeb', 1),
(@questionId8, 'Internet Explorer', 0),
(@questionId8, 'Netscape Navigator', 0),
(@questionId8, 'Mosaic', 0);

-- For Question 9
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId9, 'HyperText Transfer Protocol', 1),
(@questionId9, 'HyperText Transfer Program', 0),
(@questionId9, 'HyperText Transport Protocol', 0),
(@questionId9, 'HyperText Transport Program', 0);

-- For Question 10
INSERT INTO Options (QuestionId, OptionText, IsCorrect) VALUES 
(@questionId10, 'Google', 1),
(@questionId10, 'Windows', 0),
(@questionId10, 'Yahoo', 0),
(@questionId10, 'Bing', 0);
