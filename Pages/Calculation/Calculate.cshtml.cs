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
using System.Data;

namespace TimeManagementWebFinal.Pages.Calculation
{
    public class CalculateModel : PageModel
    {
        public StudyHours stud = new StudyHours();
        public Module mod = new Module();
        public String errorMessage = "";
        public String successMessage = "";
        public StudyHoursManager shm = new StudyHoursManager("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True");

        public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
                String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM StudyHours WHERE ModuleId=@id";
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
                                stud.HoursSpent = reader.GetInt32(5);
                                stud.Date = reader.GetDateTime(6);


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

        [BindProperty]
        public int NumberOfWeeks { get; set; }

        [BindProperty]
        public DateTime StartDate{ get; set; }

        [BindProperty]
        public int HoursSpent { get; set; }

        [BindProperty]
        public DateTime StudyDate { get; set; }

        [BindProperty]
        public int CalculatedStudyHours { get; set; }


        public void OnPost()
        {
            if (!int.TryParse(Request.Form["NumberOfWeeks"], out int numberOfWeeks))
            {
                errorMessage = "Number of weeks must be a valid integer.";
                return;
            }

            NumberOfWeeks = numberOfWeeks;

            string startDateAsString = Request.Form["StartDate"];
            if (DateTime.TryParse(startDateAsString, out DateTime startDate))
            {
                StartDate = startDate;
            }
            else
            {
                errorMessage = "Invalid date format. Please provide a valid date.";
                return;
            }

            string rawHoursSpent = Request.Form["HoursSpent"];
            // Log the raw value for debugging
            Console.WriteLine($"Raw Hours Spent: {rawHoursSpent}");

            if (!decimal.TryParse(rawHoursSpent, out decimal hoursSpentDecimal))
            {
                errorMessage = $"Hours spent must be a valid decimal number. Received: {rawHoursSpent}";
                return;
            }

            // Convert decimal to integer
            int hoursSpent = (int)hoursSpentDecimal;

            // Now you have the integer value
            Console.WriteLine($"Parsed Hours Spent: {hoursSpent}");


            string studyDateAsString = Request.Form["StartDate"];
            if (DateTime.TryParse(studyDateAsString, out DateTime studyDate))
            {
                StudyDate = studyDate;
            }
            else
            {
                errorMessage = "Invalid date format. Please provide a valid date.";
                return;
            }


            try
            {
                string selectedModuleCode = mod.Code;

                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT
                                sh.Id,
                                sh.ModuleId,
                                sh.Date,
                                sh.HoursSpent,
                                m.Code AS ModuleCode,
                                m.Name AS ModuleName,
                                m.Credits AS ModuleCredits,
                                m.ClassHoursPerWeek AS ModuleClassHoursPerWeek
                            FROM
                                StudyHours sh
                            JOIN
                                Modules m ON sh.ModuleId = m.Id
                            WHERE
                                sh.ModuleId = @ModuleId;
                            ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ModuleId", stud.ModuleId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Access the data using reader
                                int studyHoursId = reader.GetInt32(reader.GetOrdinal("Id"));
                                int moduleId = reader.GetInt32(reader.GetOrdinal("ModuleId"));
                                DateTime StudyDate = reader.GetDateTime(reader.GetOrdinal("Date"));
                                int HoursSpent = reader.GetInt32(reader.GetOrdinal("HoursSpent"));
                                string moduleCode = reader.GetString(reader.GetOrdinal("ModuleCode"));
                                string moduleName = reader.GetString(reader.GetOrdinal("ModuleName"));
                                int moduleCredits = reader.GetInt32(reader.GetOrdinal("ModuleCredits"));
                                int moduleClassHoursPerWeek = reader.GetInt32(reader.GetOrdinal("ModuleClassHoursPerWeek"));

                                // Insert the study hours into the StudyHours table
                                InsertStudyHours(moduleId, StudyDate, HoursSpent);

                                // Calculate self-study hours per week based on the provided equation
                                int calculatedStudyHours = ((moduleCredits * 10) / NumberOfWeeks) - moduleClassHoursPerWeek;

                                // Assign the calculated value to the property
                                CalculatedStudyHours = calculatedStudyHours;

                                // Optionally, you can update other UI elements or perform additional actions here

                                successMessage = "Study hours recorded and calculated successfully.";
                            }
                            else
                            {
                                errorMessage = "No study hours found for the selected module.";
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

        private void InsertStudyHours(int moduleId, DateTime studyDate, int hoursSpent)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True"))
            {
                connection.Open();

                string insertStudyHourQuery = "INSERT INTO StudyHours (ModuleId, Date, HoursSpent) VALUES (@ModuleId, @Date, @HoursSpent)";
                using (SqlCommand insertStudyHourCommand = new SqlCommand(insertStudyHourQuery, connection))
                {
                    insertStudyHourCommand.Parameters.AddWithValue("@ModuleId", moduleId);
                    insertStudyHourCommand.Parameters.AddWithValue("@Date", studyDate);
                    insertStudyHourCommand.Parameters.AddWithValue("@HoursSpent", hoursSpent);

                    int rowsAffected = insertStudyHourCommand.ExecuteNonQuery();

                    if (rowsAffected <= 0)
                    {
                        throw new Exception("Failed to record study hours.");
                    }
                }
            }
        }

    }
}
