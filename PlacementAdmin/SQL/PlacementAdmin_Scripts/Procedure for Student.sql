Use PlacementAdmin;
GO

CREATE PROCEDURE GetAvailableTestsForUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.TestId, t.TestName, t.CourseStreamId, t.CreatedDate
    FROM Tests t
    INNER JOIN StudentTests st ON t.TestId = st.TestId
    WHERE st.UserId = @UserId AND st.Completed = 0;
END

GO

CREATE PROCEDURE GetTestById
    @TestId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TestId, TestName, CourseStreamId, CreatedDate
    FROM Tests
    WHERE TestId = @TestId;
END

GO
ALTER PROCEDURE GetTestQuestions
    @TestId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        qb.QuestionId, 
        qb.QuestionText, 
        qb.QuestionType, 
        o.OptionId, 
        o.OptionText, 
        o.IsCorrect
    FROM TestQuestions tq
    JOIN QuestionBank qb ON tq.QuestionId = qb.QuestionId
    LEFT JOIN Options o ON qb.QuestionId = o.QuestionId
    WHERE tq.TestId = @TestId;
END



GO
EXEC GetTestQuestions @TestId = 1;

SELECT * FROM Options;


Use PlacementAdmin;


CREATE PROCEDURE GetStudentTestResults
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
