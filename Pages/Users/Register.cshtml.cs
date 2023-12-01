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

namespace TimeManagementWebFinal.Pages.Users
{
    public class RegisterModel : PageModel
    {
        public User use = new User();
        public Module mod = new Module();
        public String errorMessage = "";
        public String successMessage = "";
        private UserManager userManager = new UserManager("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True"); // Initialize your UserManager        

        public void OnGet()
        {
        }

        public void OnPost() 
        {
            use.Username = Request.Form["Username"];
            use.PasswordHash = Request.Form["Password"];

            if (string.IsNullOrEmpty(use.Username) || string.IsNullOrEmpty(use.PasswordHash))
            {
                errorMessage = "All fields are required";
                return;
            }

            //save data into the database
            try
            {
                
                userManager.RegisterUser(use.Username, use.PasswordHash);                

                successMessage = "You may now log in";

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        }
    }
}
