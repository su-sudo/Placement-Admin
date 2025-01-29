CREATE TABLE Tests (
    TestId INT PRIMARY KEY IDENTITY(1,1),
    TestName NVARCHAR(100),
    CourseStreamId INT FOREIGN KEY REFERENCES CourseStreams(CourseStreamId),
    CreatedDate DATETIME
);
GO
ALTER TABLE QuestionBank
DROP COLUMN TestId;
GO

GO
CREATE TABLE TestQuestions (
    TestQuestionId INT PRIMARY KEY IDENTITY(1,1),
    TestId INT FOREIGN KEY REFERENCES Tests(TestId),
    QuestionId INT FOREIGN KEY REFERENCES QuestionBank(QuestionId)
);
GO
CREATE TABLE StudentTests (
    StudentTestId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    TestId INT FOREIGN KEY REFERENCES Tests(TestId),
    AssignedDate DATETIME,
    Completed BIT
);
GO

CREATE TABLE StudentAnswers (
    AnswerId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    TestId INT FOREIGN KEY REFERENCES Tests(TestId),
    QuestionId INT FOREIGN KEY REFERENCES QuestionBank(QuestionId),
    SelectedOptionId INT FOREIGN KEY REFERENCES Options(OptionId), -- Only for MCQ
    AnswerText NVARCHAR(MAX) -- Only for Code questions
);

