using Microsoft.Data.SqlClient;
using PlacementAdmin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PlacementAdmin.DAL
{
    public class AdminDAL
    {
        private readonly DatabaseHelper databaseHelper;

        public AdminDAL(DatabaseHelper databaseHelper)
        {
            this.databaseHelper = databaseHelper;
        }

        
        public async Task AddCourseStream(CourseStream courseStream)
        {
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("AddCourseStream", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@Name", courseStream.Name);
                        sqlCommand.Parameters.AddWithValue("@Description", courseStream.Description ?? (object)DBNull.Value);

                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public async Task<int> AddQuestionWithOptions(QuestionBank question, List<Option> options)
        {
            try
            {
                using (var sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (var transaction = sqlConnection.BeginTransaction())
                    {
                        try
                        {
                            Console.WriteLine($"AddQuestion: Connection state before executing command: {sqlConnection.State}");

                            using (var sqlCommand = new SqlCommand("AddQuestion", sqlConnection, transaction))
                            {
                                sqlCommand.CommandType = CommandType.StoredProcedure;
                                sqlCommand.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                                sqlCommand.Parameters.AddWithValue("@QuestionType", question.QuestionType);
                                sqlCommand.Parameters.AddWithValue("@CourseStreamId", question.CourseStreamId);
                                sqlCommand.Parameters.AddWithValue("@DifficultyLevel", question.DifficultyLevel);
                                var newQuestionId = await sqlCommand.ExecuteScalarAsync();

                                if (question.QuestionType == "MCQ")
                                {
                                    foreach (var option in options)
                                    {
                                        using (var optionCommand = new SqlCommand("AddOption", sqlConnection, transaction))
                                        {
                                            optionCommand.CommandType = CommandType.StoredProcedure;
                                            optionCommand.Parameters.AddWithValue("@QuestionId", newQuestionId);
                                            optionCommand.Parameters.AddWithValue("@OptionText", option.OptionText);
                                            optionCommand.Parameters.AddWithValue("@IsCorrect", option.IsCorrect);
                                            await optionCommand.ExecuteNonQueryAsync();
                                        }
                                    }
                                }
                                else if(question.QuestionType == "Code")
                                {
                                    var codeAnswer = question.Options.FirstOrDefault();
                                    if(codeAnswer != null)
                                    {
                                        using( var codesqlcommand = new SqlCommand("AddCodeAnswer", sqlConnection, transaction))
                                        {
                                            codesqlcommand.CommandType = CommandType.StoredProcedure;
                                            codesqlcommand.Parameters.AddWithValue("@QuestionId", newQuestionId);
                                            codesqlcommand.Parameters.AddWithValue("@CodeAnswer", codeAnswer.CodeAnswer);
                                            await codesqlcommand.ExecuteNonQueryAsync();
                                        }
                                    }
                                    
                                }
                                transaction.Commit();
                                Console.WriteLine($"AddQuestionWithOptions: Successfully executed commands. NewQuestionId: {newQuestionId}");
                                return Convert.ToInt32(newQuestionId);
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine("An error occurred: " + ex.Message);
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding a question with options: " + ex.Message);
                return -1;
            }
        }




        
        public async Task<int> AddTest(Test test)
        {
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("AddTest", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@TestName", test.TestName);
                        sqlCommand.Parameters.AddWithValue("@CourseStreamId", test.CourseStreamId);
                        sqlCommand.Parameters.AddWithValue("@CreatedDate", test.CreatedDate);

                        var newTestId = await sqlCommand.ExecuteScalarAsync();
                        return Convert.ToInt32(newTestId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding a test: " + ex.Message);
                return -1;
            }
        }

       
        public async Task AddQuestionsToTest(int testId, List<TestQuestion> testQuestions)
        {
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("AddQuestionsToTest", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@TestId", testId);

                        DataTable questionIdsTable = new DataTable();
                        questionIdsTable.Columns.Add("QuestionId", typeof(int));
                        foreach (var testQuestion in testQuestions)
                        {
                            questionIdsTable.Rows.Add(testQuestion.QuestionId);
                        }
                        SqlParameter tvpParam = sqlCommand.Parameters.AddWithValue("@QuestionIds", questionIdsTable);
                        tvpParam.SqlDbType = SqlDbType.Structured;

                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding questions to the test: " + ex.Message);
            }
        }

        
        public async Task AssignTestToStudents(int testId, List<StudentTest> studentTests, DateTime assignedDate)
        {
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("AssignTestToStudents", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@TestId", testId);
                        sqlCommand.Parameters.AddWithValue("@AssignedDate", assignedDate);

                        DataTable userIdsTable = new DataTable();
                        userIdsTable.Columns.Add("UserId", typeof(int));
                        foreach (var studentTest in studentTests)
                        {
                            userIdsTable.Rows.Add(studentTest.UserId);
                        }
                        SqlParameter tvpParam = sqlCommand.Parameters.AddWithValue("@UserIds", userIdsTable);
                        tvpParam.SqlDbType = SqlDbType.Structured;

                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while assigning the test to students: " + ex.Message);
            }
        }

        public async Task<List<dynamic>> GetIncompleteTests()
        {
            var tests = new List<dynamic>();

            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("GetIncompleteTests", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tests.Add(new
                                {
                                    TestName = reader.GetString(reader.GetOrdinal("TestName")),
                                    StreamName = reader.GetString(reader.GetOrdinal("StreamName")),
                                    StudentCount = reader.GetInt32(reader.GetOrdinal("StudentCount")),
                                    DateOfCreation = reader.GetString(reader.GetOrdinal("DateOfCreation"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving incomplete tests: " + ex.Message);
            }

            return tests;
        }

        public async Task<List<Test>> GetAllTests()
        {
            var tests = new List<Test>();

            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("GetAllTests", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tests.Add(new Test
                                {
                                    TestId = reader.GetInt32(reader.GetOrdinal("TestId")),
                                    TestName = reader.GetString(reader.GetOrdinal("TestName")),
                                    CourseStreamId = reader.GetInt32(reader.GetOrdinal("CourseStreamId")),
                                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching tests: " + ex.Message);
            }

            return tests;
        }

        public async Task<List<QuestionBank>> GetQuestionsByCriteria(string courseStream, string difficultyLevel, string questionType)
        {
            var questions = new List<QuestionBank>();

            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("GetQuestionsByCriteria", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@CourseStream", string.IsNullOrEmpty(courseStream) ? (object)DBNull.Value : courseStream);
                        sqlCommand.Parameters.AddWithValue("@DifficultyLevel", string.IsNullOrEmpty(difficultyLevel) ? (object)DBNull.Value : difficultyLevel);
                        sqlCommand.Parameters.AddWithValue("@QuestionType", string.IsNullOrEmpty(questionType) ? (object)DBNull.Value : questionType);

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                questions.Add(new QuestionBank
                                {
                                    QuestionId = reader.GetInt32(reader.GetOrdinal("QuestionId")),
                                    QuestionText = reader.GetString(reader.GetOrdinal("QuestionText")),
                                    QuestionType = reader.GetString(reader.GetOrdinal("QuestionType")),
                                    CourseStreamId = reader.GetInt32(reader.GetOrdinal("CourseStreamId")),
                                    DifficultyLevel = reader.GetString(reader.GetOrdinal("DifficultyLevel"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching questions by criteria: " + ex.Message);
            }

            return questions;
        }

        public async Task<List<User>> GetStudentsByCourseStream(int courseStreamId)
        {
            var students = new List<User>();

            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("GetStudentsByCourseStream", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@CourseStreamId", courseStreamId);

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                students.Add(new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    Username = reader.GetString(reader.GetOrdinal("FullName")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    CourseStreamId = reader.GetInt32(reader.GetOrdinal("CourseStreamId"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching students by course stream: " + ex.Message);
            }

            return students;
        }

        public async Task<List<Test>> GetTestsByCourseStream(int courseStreamId)
        {
            var tests = new List<Test>();

            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();

                    using (SqlCommand sqlCommand = new SqlCommand("GetTestsByCourseStream", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@CourseStreamId", courseStreamId);

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tests.Add(new Test
                                {
                                    TestId = reader.GetInt32(reader.GetOrdinal("TestId")),
                                    TestName = reader.GetString(reader.GetOrdinal("TestName")),
                                    CourseStreamId = reader.GetInt32(reader.GetOrdinal("CourseStreamId")),
                                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while fetching tests by course stream: " + ex.Message);
            }

            return tests;
        }



        public async Task<List<TopThreeStudents>> GetTopThreeStudents()
        {
            List <TopThreeStudents> topThreeStudentsList = new List<TopThreeStudents>();
                ;
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                   await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("GetTopStudents", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync()) {
                                var student = new TopThreeStudents
                                {
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    UserName = reader.GetString(reader.GetOrdinal("Username")),
                                    TotalScore = reader.GetInt32(reader.GetOrdinal("TotalScore"))
                                };
                                topThreeStudentsList.Add(student);
                            }
                        };
                    };
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exceptuion {ex} has beem occured");
            }
            return topThreeStudentsList;
        }


    }
}
