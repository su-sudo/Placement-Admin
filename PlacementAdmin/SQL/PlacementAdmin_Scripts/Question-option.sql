USE PlacementAdmin;
GO

--Add New course for placement
--*****************************************

GO
CREATE PROCEDURE AddCourseStream
    @Name NVARCHAR(100),
    @Description NVARCHAR(500)
AS
BEGIN
    INSERT INTO CourseStreams (Name, Description)
    VALUES (@Name, @Description);
END

--***********************************************
GO

--*********************************************************************
--question bank CRUD
--**********************************************************************

CREATE TABLE QuestionBank (
    QuestionId INT PRIMARY KEY IDENTITY(1,1),
    QuestionText NVARCHAR(MAX),
    QuestionType NVARCHAR(50), -- 'MCQ' or 'Code'
    CourseStreamId INT FOREIGN KEY REFERENCES CourseStreams(CourseStreamId),
    DifficultyLevel NVARCHAR(50) -- 'Easy', 'Medium', 'Hard'
);
GO
CREATE TABLE Options (
    OptionId INT PRIMARY KEY IDENTITY(1,1),
    QuestionId INT FOREIGN KEY REFERENCES QuestionBank(QuestionId),
    OptionText NVARCHAR(MAX),
    IsCorrect BIT
);

GO
--Add the question
-----------
CREATE PROCEDURE AddQuestion
    @QuestionText NVARCHAR(MAX),
    @QuestionType NVARCHAR(50),
    @CourseStreamId INT,
    @DifficultyLevel NVARCHAR(50)
AS
BEGIN
    INSERT INTO QuestionBank (QuestionText, QuestionType, CourseStreamId, DifficultyLevel)
    VALUES (@QuestionText, @QuestionType, @CourseStreamId, @DifficultyLevel);
    SELECT SCOPE_IDENTITY() AS NewQuestionId;
END

GO
CREATE PROCEDURE AddOption
    @QuestionId INT,
    @OptionText NVARCHAR(MAX),
    @IsCorrect BIT
AS
BEGIN
    INSERT INTO Options (QuestionId, OptionText, IsCorrect)
    VALUES (@QuestionId, @OptionText, @IsCorrect);
END

GO
