SELECT * FROM TESTS;
SELECT * FROM StudentTests;
SELECT * FROM CourseStreams;
SELECT t.TestName, cs.Name as 'StreamName', COUNT(st.StudentTestId) as 'StudentCount'
,FORMAT(t.CreatedDate, 'dd/MM/yyyy') as 'Date of creation' FROM Tests t
JOIN CourseStreams cs ON cs.CourseStreamId = t.CourseStreamId
JOIN StudentTests st ON st.TestId = t.TestId
GROUP BY t.TestName, cs.Name,t.CreatedDate;


SELECT t.TestName, cs.Name as 'StreamName', st.StudentTestId as 'StudentCount'
,FORMAT(t.CreatedDate, 'dd/MM/yyyy') as 'Date of creation' FROM Tests t
JOIN CourseStreams cs ON cs.CourseStreamId = t.CourseStreamId
JOIN StudentTests st ON st.TestId = t.TestId;