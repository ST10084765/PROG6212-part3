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
using System.Xml.Linq;
using TimeManagementWebFinal.Pages.Users;

namespace TimeManagementWebFinal.Pages.Modules
{
    public class AddModuleModel : PageModel
    {
        public Module mod = new Module();
        public String errorMessage = "";
        public String successMessage = "";
        public LoginModel login = new LoginModel();
        public void OnGet()
        {
        }

        public void OnPost()
        {
            mod.Code = Request.Form["code"];
            mod.Name = Request.Form["name"];

            if (!int.TryParse(Request.Form["Credits"], out int credits))
            {
                errorMessage = "Credits must be a valid integer.";
                return;
            }

            mod.Credits = credits;

            if (!int.TryParse(Request.Form["ClassHoursPerWeek"], out int classHours))
            {
                errorMessage = "Class Hours Per Week must be a valid integer.";
                return;
            }

            mod.ClassHoursPerWeek = classHours;

            if (string.IsNullOrEmpty(mod.Code) || string.IsNullOrEmpty(mod.Name) ||
                mod.Credits == 0 || mod.ClassHoursPerWeek == 0)
            {
                errorMessage = "All fields are required";
                return;
            }

            //save data into the database
            try
            {
                String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True";
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Fetch the user's Id based on the username
                    int userId = GetUserIdByUsername(connection, login.CurrentUserUsername);

                    if (userId > 0)
                    {
                        // SQL command to insert a new module
                        string insertModuleQuery = "INSERT INTO Modules (Code, Name, Credits, ClassHoursPerWeek, UserId) VALUES (@Code, @Name, @Credits, @ClassHours, @UserId)";

                        using (SqlCommand insertModuleCommand = new SqlCommand(insertModuleQuery, connection))
                        {
                            insertModuleCommand.Parameters.AddWithValue("@Code", mod.Code);
                            insertModuleCommand.Parameters.AddWithValue("@Name", mod.Name);
                            insertModuleCommand.Parameters.AddWithValue("@Credits", credits);
                            insertModuleCommand.Parameters.AddWithValue("@ClassHours", classHours);
                            insertModuleCommand.Parameters.AddWithValue("@UserId", userId);

                            insertModuleCommand.ExecuteNonQuery();

                        }
                    }
                    else
                    {
                        errorMessage = "User not found. Please log in again.";
                    }
                }
                
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            mod.Code = "";
            mod.Name = "";
            mod.Credits = 0;
            mod.ClassHoursPerWeek = 0;

            successMessage = "New module added successfully";

            Response.Redirect("/Modules/Index");
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
