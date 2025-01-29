using Microsoft.Data.SqlClient;
using PlacementAdmin.Models;
using PlacementAdmin.services;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PlacementAdmin.DAL
{
    public class UserDAL
    {
        private readonly DatabaseHelper databaseHelper;
        

        public UserDAL(DatabaseHelper databaseHelper)
        {
            this.databaseHelper = databaseHelper;
           
        }

        public async Task AddUser(User user)
        {
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("AddUser", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@Username", user.Username);
                        sqlCommand.Parameters.AddWithValue("@Password", user.Password);
                        sqlCommand.Parameters.AddWithValue("@Email", user.Email);
                        sqlCommand.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                        sqlCommand.Parameters.AddWithValue("@Gender", user.Gender);
                        sqlCommand.Parameters.AddWithValue("@ProfilePicture", user.ProfilePicture ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@CourseStreamId", user.CourseStreamId);
                        sqlCommand.Parameters.AddWithValue("@Role", user.Role);

                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public async Task<User> GetUserById(int userId)
        {
            User user = null;

            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("GetUserById", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.Read())
                            {
                                user = new User
                                {
                                    Id = Convert.ToInt32(sqlDataReader["Id"]),
                                    Username = sqlDataReader["Username"].ToString(),
                                    Password = sqlDataReader["Password"].ToString(),
                                    Email = sqlDataReader["Email"].ToString(),
                                    Gender = sqlDataReader["Gender"].ToString(),
                                    ProfilePicture = sqlDataReader["ProfilePicture"].ToString(),
                                    CourseStreamId = Convert.ToInt32(sqlDataReader["CourseStreamId"]),
                                    Role = sqlDataReader["Role"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return user;
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("UpdateUser", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@Id", user.Id);
                        sqlCommand.Parameters.AddWithValue("@Username", user.Username);
                        sqlCommand.Parameters.AddWithValue("@Password", user.Password);
                        sqlCommand.Parameters.AddWithValue("@Email", user.Email);
                        sqlCommand.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                        sqlCommand.Parameters.AddWithValue("@Gender", user.Gender);
                        sqlCommand.Parameters.AddWithValue("@ProfilePicture", user.ProfilePicture ?? (object)DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@CourseStreamId", user.CourseStreamId);
                        sqlCommand.Parameters.AddWithValue("@Role", user.Role);

                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public async Task DeleteUser(int userId)
        {
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("DeleteUser", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@UserId", userId);

                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public async Task<User> GetUserByUsernameAndPassword(string username, string password)
        {
            User user = null;

            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("GetUserByUsernameAndPassword", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@Username", username);
                        sqlCommand.Parameters.AddWithValue("@Password", password);

                       using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.Read())
                            {
                                user = new User
                                {
                                    Id = Convert.ToInt32(sqlDataReader["Id"]),
                                    Username = sqlDataReader["Username"].ToString(),
                                    Password = sqlDataReader["Password"].ToString(),
                                    Email = sqlDataReader["Email"].ToString(),
                                    Role = sqlDataReader["Role"].ToString(),
                                    ProfilePicture = sqlDataReader["ProfilePicture"].ToString()
                                };
                            }
                        }

                        
                    }
                   
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return user;
        }

        public async Task<List<CourseStream>> GetAvailableCourseStreamsAsync()
        {
            List<CourseStream> courseStreams = new List<CourseStream>();
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync(); 

                    using (SqlCommand sqlCommand = new SqlCommand("GetAvailableCourseStreams", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync()) 
                        {
                            while (await sqlDataReader.ReadAsync()) 
                            {
                                courseStreams.Add(new CourseStream
                                {
                                    CourseStreamId = Convert.ToInt32(sqlDataReader["CourseStreamId"]),
                                    Name = sqlDataReader["Name"].ToString(),
                                    Description = sqlDataReader["Description"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return courseStreams;
        }


        public async Task<List<Test>> GetAvailableTestsForUser(int userId)
        {
            var tests = new List<Test>();
            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("GetAvailableTestsForUser", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tests.Add(new Test
                                {
                                    TestId = reader.GetInt32("TestId"),
                                    TestName = reader.GetString("TestName"),
                                    CourseStreamId = reader.GetInt32("CourseStreamId"),
                                    CreatedDate = reader.GetDateTime("CreatedDate")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving available tests: " + ex.Message);
            }

            return tests;
        }


        public async Task<TestDetails> GetTestQuestionsAsync(int testId)
        {
            var testDetails = new TestDetails
            {
                TestId = testId,
                Questions = new List<QuestionWithOptions>()
            };

            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("GetTestQuestions", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@TestId", testId);

                        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var questionId = reader.GetInt32("QuestionId");
                                var questionText = reader.GetString("QuestionText");
                                var questionType = reader.GetString("QuestionType");

                                var question = testDetails.Questions.Find(q => q.QuestionId == questionId);
                                if (question == null)
                                {
                                    question = new QuestionWithOptions
                                    {
                                        QuestionId = questionId,
                                        QuestionText = questionText,
                                        QuestionType = questionType,
                                        Options = new List<Option>()
                                    };
                                    testDetails.Questions.Add(question);
                                }

                                if (!reader.IsDBNull(reader.GetOrdinal("OptionId")))
                                {
                                    var option = new Option
                                    {
                                        OptionId = reader.GetInt32("OptionId"),
                                        OptionText = reader.GetString("OptionText"),
                                        IsCorrect = reader.GetBoolean("IsCorrect")
                                    };
                                    question.Options.Add(option);
                                }
                            }
                            Console.WriteLine("Exit Loop");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving test questions: " + ex.Message);
            }

            return testDetails;
        }
        public async Task SaveStudentAnswersAsync(List<StudentAnswer> answers)
        {
            var table = new DataTable();
            table.Columns.Add("UserId", typeof(int));
            table.Columns.Add("TestId", typeof(int));
            table.Columns.Add("QuestionId", typeof(int));
            table.Columns.Add("SelectedOptionId", typeof(int));
            table.Columns.Add("AnswerText", typeof(string));

            foreach (var answer in answers)
            {
                table.Rows.Add(
                    answer.UserId,
                    answer.TestId,
                    answer.QuestionId,
                    (object)answer.SelectedOptionId ?? DBNull.Value,
                    (object)answer.AnswerText ?? DBNull.Value
                );
            }

            try
            {
                using (SqlConnection sqlConnection = databaseHelper.GetConnection())
                {
                    await sqlConnection.OpenAsync();
                    using (SqlCommand sqlCommand = new SqlCommand("BulkInsertStudentAnswers", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@StudentAnswers", table).SqlDbType = SqlDbType.Structured;
                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while saving student answers: " + ex.Message);
            }
        }



         public async Task<List<TestResult>> GetStudentTestResultsAsync(int userId)
         {
             var testResults = new List<TestResult>();

             using (SqlConnection sqlConnection = databaseHelper.GetConnection())
             {
                 
             using (SqlCommand sqlCommand = new SqlCommand("GetStudentTestResults", sqlConnection))
             {
                 sqlCommand.CommandType = CommandType.StoredProcedure;
                 sqlCommand.Parameters.AddWithValue("@UserId", userId);

                 await sqlConnection.OpenAsync();
                 using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync())
                 {
                     while (await reader.ReadAsync())
                     {
                         testResults.Add(new TestResult
                         {
                             UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                             Username = reader.GetString(reader.GetOrdinal("Username")),
                             TestId = reader.GetInt32(reader.GetOrdinal("TestId")),
                             TestName = reader.GetString(reader.GetOrdinal("TestName")),
                             CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                             TotalQuestions = reader.GetInt32(reader.GetOrdinal("TotalQuestions")),
                             CorrectAnswers = reader.GetInt32(reader.GetOrdinal("CorrectAnswers")),
                             ScorePercentage = (double)reader.GetDecimal(reader.GetOrdinal("ScorePercentage"))
                         });
                     }
                 }
             }
   
                
             }

             return testResults;
         }
        



    }
}
