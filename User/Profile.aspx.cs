using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Services.Description;
using System.Web.UI;

namespace AlgoLab
{
    public partial class Profile : System.Web.UI.Page
    {

        string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            string userID = Session["UserID"]?.ToString();
            string username = Session["Username"]?.ToString();
            if (string.IsNullOrEmpty(userID))
            {
                ShowAlert("You are not a user!");
            }

            if (!IsPostBack)
            {
                GetUserProfileData(userID);
            }

            string role = Session["Role"]?.ToString();

            // Check if the role is null or empty
            if (!IsPostBack)
            {
                string loginType = Request.QueryString["role"]; // Get the role from the query string

                // Set the page title based on the role
                Div1.Visible = true;
            }

        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_Profile.txt");

            // Ensure the Logs folder exists; create it if it doesn't
            if (!System.IO.Directory.Exists(logFolder))
            {
                System.IO.Directory.CreateDirectory(logFolder);
            }

            // Append the error details to the log file
            string logEntry = $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n\n";
            System.IO.File.AppendAllText(logFile, logEntry);

            // Clear the error to prevent it from propagating further
            Server.ClearError();

            // Redirect to a friendly error page, specific to this page
            Response.Redirect("~/ErrorPages/ErrorApp.aspx");
        }


        // Method to display a JavaScript alert
        private void ShowAlert(string message)
        {
            string script = $"<script type=\"text/javascript\">alert('{message}');</script>";

            // Register the script on the page
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script);
        }




        protected void SaveProfileChanges(object sender, EventArgs e)
        {
            string username = Session["Username"]?.ToString();
            string CustID = Session["UserID"]?.ToString();
            //Get the new values from the controls on the page
            string newFirstName = txtFirstName.Text;
            string newLastName = txtLastName.Text;
            string newEmail = txtEmail.Text;
            string newPhoneNumber = txtPhoneNumber.Text;
            string newGender = rblGender.SelectedValue;
            string newPassword = txtPassword.Text;
            string newCustUsername = txtCustUsername.Text;

            // Validate email
            if (!IsValidEmail(newEmail))
            {
                ShowAlert("Invalid email format. Example: user@example.com");
                return;
            }

            // Validate phone number
            if (!IsValidPhoneNumber(newPhoneNumber))
            {
                ShowAlert("Invalid phone number format. Example: 012-1234556");
                return;
            }

            //// Define the connection string
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;



            // SQL query to update the user profile
            string query = @"
                    UPDATE Customer
                    SET CustFname = @FirstName,
                        CustLname = @LastName,
                        CustEmail = @Email,
                        CustTel = @PhoneNumber,
                        CustUsername = @CustUsername,
                        CustPassword = @Password,
                        CustGender = @Gender
                    
                    WHERE CustID = @CustID";

            // Execute the query
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    string userID = Session["UserID"]?.ToString();
                    // Add parameters
                    cmd.Parameters.AddWithValue("@FirstName", newFirstName);
                    cmd.Parameters.AddWithValue("@LastName", newLastName);
                    cmd.Parameters.AddWithValue("@Email", newEmail);
                    cmd.Parameters.AddWithValue("@PhoneNumber", newPhoneNumber);
                    cmd.Parameters.AddWithValue("@Gender", newGender);
                    cmd.Parameters.AddWithValue("@Password", newPassword);
                    cmd.Parameters.AddWithValue("@CustUsername", newCustUsername);
                    cmd.Parameters.AddWithValue("@CustID", CustID);

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "SuccessAlert", "alert('Profile updated successfully.');", true);
                            GetUserProfileData(userID);
                            String message = Session["UserID"].ToString();
                            ShowAlert(message);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "FailureAlert", "alert('No changes made.');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowAlert(ex.Message);
                    }
                }
            }



        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
        }

        // Method to validate phone number (simple format: xxx-xxxxxxx)
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            string phonePattern = @"^\d{3}-\d{7}$"; // e.g., 012-1234567
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, phonePattern);
        }




        private void GetUserProfileData(string CustID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            string query = @"
        SELECT CustID, CustUsername, CustPassword, CustFname, CustLname, CustGender, CustEmail, CustTel, CustRole, CustDateJoined, CustProfileImgPath 
        FROM Customer 
        WHERE CustID = @CustID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustID", CustID);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();
                            lblCustID.Text = reader["CustID"].ToString();
                            lblCustUsername.Text = reader["CustUsername"].ToString();
                            lblPassword.Text = reader["CustPassword"].ToString();
                            lblGender.Text = reader["CustGender"].ToString();
                            lblFirstName.Text = reader["CustFname"].ToString();
                            lblLastName.Text = reader["CustLname"].ToString();
                            lblEmail.Text = reader["CustEmail"].ToString();
                            lblPhoneNumber.Text = reader["CustTel"].ToString();
                            lblRole.Text = reader["CustRole"].ToString();
                            lblDateJoined.Text = Convert.ToDateTime(reader["CustDateJoined"]).ToString("MMMM dd, yyyy");

                            string profileImgPath = reader["CustProfileImgPath"].ToString();
                            imgProfile.ImageUrl = ResolveUrl(profileImgPath);
                        }
                        else
                        {
                            ShowAlert("No profile found.");
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        ShowAlert(ex.Message);
                    }
                }
            }
        }
    }
}
