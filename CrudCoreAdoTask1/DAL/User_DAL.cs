using CrudCoreAdoTask1.Models;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using Microsoft.AspNetCore.Mvc;

namespace CrudCoreAdoTask1.DAL
{
    public class User_DAL
    {

        public static IConfiguration configuration { get; set; }

        public User_DAL(IConfiguration config)
        {
            configuration = config;
        }
        private string GetConnectionString()
        {
            return configuration.GetConnectionString("DefaultConnection");
        }

        public List<Users> GetAllUsers()
        {
            List <Users> userList = new List<Users>();
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand("ReadAllUser", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            while (sqlDataReader.Read()) {

                                Users user = new Users()
                                {
                                    Id = Convert.ToInt32(sqlDataReader["Id"]),
                                    Name = sqlDataReader["Name"].ToString(),
                                    DateOfBirth = Convert.ToDateTime(sqlDataReader["DateOfBirth"]).Date,
                                    ProfilePic = sqlDataReader["ProfilePic"].ToString(),
                                    


                                };
                                userList.Add(user);
                                
                            
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return userList;
        }

        
        public string AddUser(Users users)
        {
            int _result;
            

            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    using(SqlCommand sqlCommand = new SqlCommand("AddUser", sqlConnection))
                    {
                        sqlCommand.CommandType= CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("Name", users.Name);
                        sqlCommand.Parameters.AddWithValue("DateOfBirth",users.DateOfBirth);
                        sqlCommand.Parameters.AddWithValue("ProfilePic", users.ProfilePic);
                        sqlCommand.Parameters.AddWithValue("PasswordHash", users.PasswordHash);

                       _result = sqlCommand.ExecuteNonQuery();
                        Console.WriteLine($"{_result} Rows Affected");
                       
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return "Success";
        }

        public string DeleteUser(int Id )
        {
            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try {
                    sqlConnection.Open();
                    using(SqlCommand sqlCommand = new SqlCommand("DeleteUser", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("Id", Id);
                        int result = sqlCommand.ExecuteNonQuery();
                        Console.WriteLine($"Query executed ,{result} Lines affected");
                        
                    }
                    
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }
                return "Successfully deleted";
            }
        }

        public Users GetUserById(int Id)
        {
            Users user = null;

            using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand("GetById", sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@Id", Id);

                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.Read()) 
                            {
                                user = new Users()
                                {
                                    Id = Convert.ToInt32(sqlDataReader["Id"]),
                                    Name = sqlDataReader["Name"].ToString(),
                                    DateOfBirth = Convert.ToDateTime(sqlDataReader["DateOfBirth"]),
                                    ProfilePic = sqlDataReader["ProfilePic"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                   
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return user;
        }

        public string UpdateUser(Users user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand("EditUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                        command.Parameters.AddWithValue("@ProfilePic", user.ProfilePic);
                       

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }

        }


    }
}
