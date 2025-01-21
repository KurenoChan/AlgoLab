using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace AlgoLab
{
    public partial class SignUp : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_SignUp.txt");

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

        protected void RegisterButton_Click(object sender, EventArgs e)
        {


            // Collect form values
            string custID = GenerateUniqueCustID(); // Unique Customer ID
            string username = Username.Text.Trim();
            string password = Password.Text.Trim();
            string confirmPassword = ConfirmPassword.Text.Trim();
            string firstName = FirstName.Text.Trim();
            string lastName = LastName.Text.Trim();
            string gender = MaleGender.Checked ? "M" : (FemaleGender.Checked ? "F" : null);
            string email = UserEmail.Text.Trim();
            string phoneNumber = PhoneNumber.Text.Trim();
            string role = "STD"; // Default role for a new user

            // Validate form inputs
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber))
            {
                DisplayError("All fields are required.");
                return;
            }

            if (username.Length > 30)
            {
                DisplayError("Username cannot exceed 30 characters.");
                return;
            }

            if (password.Length > 30)
            {
                DisplayError("Password cannot exceed 30 characters.");
                return;
            }

            if (password != confirmPassword)
            {
                DisplayError("Passwords do not match.");
                return;
            }

            if (string.IsNullOrEmpty(gender))
            {
                DisplayError("Please select a gender.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                DisplayError("Invalid email format.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^01[0-9]-[0-9]{7}$"))
            {
                DisplayError("Phone number must be in the format 01X-XXXXXXX.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // Check if username already exists
                    string checkUsernameQuery = "SELECT COUNT(*) FROM Customer WHERE CustUsername = @CustUsername";
                    using (SqlCommand checkCmd = new SqlCommand(checkUsernameQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@CustUsername", username);
                        int usernameExists = (int)checkCmd.ExecuteScalar();

                        if (usernameExists > 0)
                        {
                            DisplayError("Username already exists. Please choose another.");
                            return;
                        }
                    }

                    // Insert new customer record
                    string sql = @"INSERT INTO Customer 
                            (CustID, CustUsername, CustPassword, CustFname, CustLname, CustGender, CustEmail, CustTel, CustRole)
                           VALUES 
                            (@CustID, @CustUsername, @CustPassword, @CustFname, @CustLname, @CustGender, @CustEmail, @CustTel, @CustRole)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@CustID", custID);
                        cmd.Parameters.AddWithValue("@CustUsername", username);
                        cmd.Parameters.AddWithValue("@CustPassword", password);
                        cmd.Parameters.AddWithValue("@CustFname", firstName);
                        cmd.Parameters.AddWithValue("@CustLname", lastName);
                        cmd.Parameters.AddWithValue("@CustGender", gender);
                        cmd.Parameters.AddWithValue("@CustEmail", email);
                        cmd.Parameters.AddWithValue("@CustTel", phoneNumber);
                        cmd.Parameters.AddWithValue("@CustRole", role);

                        try
                        {
                            cmd.ExecuteNonQuery();


                            // Relative path to the folder you want to create
                            string relativePath = $"~/Assets/Images/customer/{custID}/";

                            // Convert relative path to physical path
                            string physicalPath = Server.MapPath(relativePath);

                            // Check if the folder already exists
                            if (!Directory.Exists(physicalPath))
                            {
                                // Create the folder
                                Directory.CreateDirectory(physicalPath);
                            }

                            DisplaySuccess("Registration successful! You can now log in.");
                            ClearForm();
                        }
                        catch (Exception ex)
                        {
                            DisplayError("Registration failed. Please try again.\nError: " + ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError($"An error occurred: {ex.Message}");
            }
        }
        private string GenerateUniqueCustID()
        {
            string prefix = "CUST-";
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"); // Get current timestamp
            string newCustID = $"{prefix}{timestamp}-0001"; // Start with the default sequence number (0001)

            // Get all existing CourseFeedbackIDs from the database and check for uniqueness
            List<string> existingFeedbackIDs = GetAllExistingCustIDs();

            // Keep generating a new ID until it's unique
            while (existingFeedbackIDs.Contains(newCustID))
            {
                // Increment the sequence number to avoid duplication
                string lastSequence = newCustID.Split('-')[1]; // Get the sequence part (after the second '-')
                int nextSeq = int.Parse(lastSequence) + 1; // Increment the sequence number
                newCustID = $"{prefix}{timestamp}-{nextSeq:D4}"; // Ensure the sequence is always 4 digits (e.g., 0001, 0002)
            }

            return newCustID;
        }
        private List<string> GetAllExistingCustIDs()
        {
            List<string> CustIDs = new List<string>();

            // Query to get all CourseFeedbackIDs
            SqlCommand cmd = new SqlCommand("SELECT CustID FROM Customer", conn);
            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string custID = reader["CustID"].ToString();
                    CustIDs.Add(custID); // Add each ID to the list
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // Display the exception message in a JavaScript alert
                string script = $"alert('An error occurred: {ex.Message}');";
                ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", script, true);
            }
            finally
            {
                conn.Close(); // Ensure the connection is closed
            }

            return CustIDs;
        }
        private void DisplayError(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", $"alert('{message}');", true);
        }

        private void DisplaySuccess(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SuccessAlert", $"alert('{message}');", true);
        }

        private void ClearForm()
        {
            FirstName.Text = string.Empty;
            LastName.Text = string.Empty;
            Username.Text = string.Empty;
            UserEmail.Text = string.Empty;
            PhoneNumber.Text = string.Empty;
            Password.Text = string.Empty;
            ConfirmPassword.Text = string.Empty;
            MaleGender.Checked = false;
            FemaleGender.Checked = false;
        }
    }
}