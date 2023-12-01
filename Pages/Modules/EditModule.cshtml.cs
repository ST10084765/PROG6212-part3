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


namespace TimeManagementWebFinal.Pages.Modules
{
    public class EditModuleModel : PageModel
    {
        public Module mod = new Module();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
                String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM Modules WHERE Id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {                                
                                mod.Id = reader.GetInt32(0);
                                mod.Code = reader.GetString(1);
                                mod.Name = reader.GetString(2);
                                mod.Credits = reader.GetInt32(3);
                                mod.ClassHoursPerWeek = reader.GetInt32(4);

                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
                return;
            }
        }

        public void OnPost()
        {
            if (int.TryParse(Request.Form["id"], out int moduleId))
            {
                mod.Id = moduleId;
            }
            else
            {
                // Handle the case where the conversion fails, e.g., log an error or return an error message.
                errorMessage = "Invalid module ID provided.";
                return;
            }

            mod.Code = Request.Form["Code"];
            mod.Name = Request.Form["Name"];

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

            try
            {
                String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Modules " +
                                 "SET Code=@Code, Name=@Name, Credits=@Credits, ClassHoursPerWeek=@ClassHoursPerWeek " +
                                 "WHERE Id=@Id";

                    using (SqlCommand Command = new SqlCommand(sql, connection))
                    {
                        Command.Parameters.AddWithValue("@Id", mod.Id);
                        Command.Parameters.AddWithValue("@Code", mod.Code);
                        Command.Parameters.AddWithValue("@Name", mod.Name);
                        Command.Parameters.AddWithValue("@Credits", mod.Credits);
                        Command.Parameters.AddWithValue("@ClassHoursPerWeek", mod.ClassHoursPerWeek);

                        Console.WriteLine(Command.CommandText);

                        int rowsAffected = Command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            errorMessage = "No rows updated. Check if the record with the specified Id exists.";
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Modules/Index");
        }
    }
}
