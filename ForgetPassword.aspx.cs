using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlgoLab
{
    public partial class ForgetPassword : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Ensure the reset part is hidden initially
                reset_pass_part.Style["display"] = "none";
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_ForgetPassword.txt");

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

        protected void checkemail_Click(object sender, EventArgs e)
        {
            string email = Foremail.Text.Trim();
            string username = Forusername.Text.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username))
            {
                resetMessageEmail.Text = "All fields are required.";
                resetMessageEmail.Attributes["style"] = "color: red;";
                resetMessageEmail.Visible = true;
                return; // Exit the method if validation fails
            }

            // Proceed with database checks
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Customer WHERE CustEmail = @Email AND CustUsername = @Username", con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Username", username);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    resetMessageEmail.Text = "Email and username matched. Please reset your password.";
                    resetMessageEmail.Attributes["style"] = "color: green;";
                    resetMessageEmail.Visible = true;
                    resetpassForm.Visible = false;
                    reset_pass_part.Style["display"] = "block";
                }
                else
                {
                    resetMessageEmail.Text = "Invalid email or username. Please try again.";
                    resetMessageEmail.Attributes["style"] = "color: red;";
                    resetMessageEmail.Visible = true;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                resetMessageEmail.Text = "An error occurred: " + ex.Message;
                resetMessageEmail.Attributes["style"] = "color: red;";
                resetMessageEmail.Visible = true;
            }
            finally
            {
                con.Close();
            }
        }

        protected void resetpassword_Click(object sender, EventArgs e)
        {
            string newPasswordText = newPassword.Text.Trim();
            string confirmPasswordText = confirmPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(newPasswordText) || string.IsNullOrWhiteSpace(confirmPasswordText))
            {
                resetMessage.Text = "All fields are required.";
                resetMessage.ForeColor = System.Drawing.Color.Red;
                resetMessage.Visible = true;
                return; // Exit the method if validation fails
            }

            if (newPasswordText != confirmPasswordText)
            {
                resetMessage.Text = "Passwords do not match.";
                resetMessage.ForeColor = System.Drawing.Color.Red;
                resetMessage.Visible = true;
                return; // Exit the method if passwords don't match
            }

            // Proceed with password update
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Customer SET CustPassword = @NewPassword WHERE CustUsername = @Username", con);
                cmd.Parameters.AddWithValue("@NewPassword", newPasswordText);
                cmd.Parameters.AddWithValue("@Username", Forusername.Text.Trim());

                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    resetMessage.Text = "Password reset successfully.";
                    resetMessage.ForeColor = System.Drawing.Color.Green;
                    resetMessage.Visible = true;

                    // Block interaction and start countdown
                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "startCountdown",
                        "startCountdown(3, 'Home.aspx');",
                        true);
                }
                else
                {
                    resetMessage.Text = "Failed to reset password. Please try again.";
                    resetMessage.ForeColor = System.Drawing.Color.Red;
                    resetMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                resetMessage.Text = "An error occurred: " + ex.Message;
                resetMessage.ForeColor = System.Drawing.Color.Red;
                resetMessage.Visible = true;
            }
            finally
            {
                con.Close();
            }
        }
    }
}