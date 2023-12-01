using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeManagementLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using TimeManagementWebFinal.Pages.Users;

namespace TimeManagementWebFinal.Pages.Modules
{
    public class IndexModel : PageModel
    {
        public LoginModel login = new LoginModel();
        public Module mod = new Module();
        public List<Module> listModules = new List<Module>();

        public void OnGet()
        {
            try
            {
                
                String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True";
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    GetUserIdByUsername(connection, login.CurrentUserUsername);
                    String sql = "SELECT * from Modules WHERE UserId = (SELECT Id FROM Users WHERE Username = @Username)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", login.CurrentUserUsername);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Module mod = new Module();  //from the class library
                                mod.Id = reader.GetInt32(0);
                                mod.Code = reader.GetString(1);
                                mod.Name = reader.GetString(2);
                                mod.Credits = reader.GetInt32(3);
                                mod.ClassHoursPerWeek = reader.GetInt32(4);

                                listModules.Add(mod);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception:  " + ex.ToString());
            }
        }

        private int GetUserIdByUsername(SqlConnection connection, string username)
        {
            string query = "SELECT Id FROM Users WHERE Username = @Username";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                int userId = (int)command.ExecuteScalar(); // Assuming Id is an integer

                return userId;
            }
        }
    }
}
