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
    public class LoginModel : PageModel
    {
        public User use = new User();
        public Module mod = new Module();
        public String errorMessage = "";
        public String successMessage = "";
        public UserManager userManager = new UserManager("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TimeManagementDB;Integrated Security=True"); // Initialize your UserManager        
        public string CurrentUserUsername;

        public void OnGet()
        {
        }

        public void OnPost() 
        {
            use.Username = Request.Form["Username"];
            use.PasswordHash = Request.Form["Password"];

            User authenticatedUser = userManager.AuthenticateUser(use.Username, use.PasswordHash);            

            if (authenticatedUser != null)
            {
                // Successful login
                // Store the authenticated user details as needed
                CurrentUserUsername = authenticatedUser.Username;
                Response.Redirect("/Modules/Index");
            }
            else
            {
                errorMessage = "Unable to log in";
            }
        }
    }
}
