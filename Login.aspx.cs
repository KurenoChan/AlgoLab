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
    public partial class Login : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Safely retrieve the role from the query string
                string loginType = Request.QueryString["role"];

                if (string.IsNullOrEmpty(loginType))
                {
                    // Handle the case where no role is provided
                    LoginContainerTitle.InnerText = "Login";
                }
                else if (loginType == "customer")
                {
                    LoginContainerTitle.InnerText = "Customer Login";
                    LoginContainerTitle.Attributes["class"] = "customer-login";
                }
                else if (loginType == "staff")
                {
                    ForgotPasswordLink.Visible = false;
                    LoginContainerTitle.InnerText = "Staff Login";
                    LoginContainerTitle.Attributes["class"] = "staff-login";
                    SignUpSuggest.Visible = false;
                    Selection.Visible = false;
                }

                txtUsername.Focus();
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_Login.txt");

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
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            VerifyLoginCredentials();
        }

        protected void VerifyLoginCredentials()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Safely retrieve the role from the query string
            string role = Request.QueryString["role"];
            if (string.IsNullOrEmpty(role))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Role is not specified. Please access the correct login page.');", true);
                return;
            }

            if (role == "staff")
            {
                // Staff login validation
                if (username == "Administrator1" && password == "1234")
                {
                    Session["Role"] = "Admin";
                    Response.Redirect($"~/Management/AdminInterface.aspx?username={username}");
                }
                else if (username == "Administrator2" && password == "123456")
                {
                    Session["Role"] = "Admin";
                    Response.Redirect($"~/Management/AdminInterface.aspx?username={username}");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Invalid admin credentials!');", true);
                }
            }
            else if (role == "customer")
            {
                con.Open();
                if (IsValidCustomer(username, password))
                {
                    con.Close();

                    string returnUrl = Request.QueryString["ReturnUrl"];
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        string decodedUrl = HttpUtility.UrlDecode(returnUrl);
                        string resolvedUrl = ResolveUrl(decodedUrl);
                        Response.Redirect(resolvedUrl);
                    }
                    else
                    {
                        Response.Redirect("~/User/Profile.aspx");
                    }
                }
                con.Close();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Invalid role specified.');", true);
            }
        }

        private bool IsValidCustomer(string inputUsername, string inputPassword)
        {
            SqlCommand cmdUser = new SqlCommand(
                "SELECT * FROM Customer WHERE CustUsername = @Username;", con);

            cmdUser.Parameters.AddWithValue("@Username", inputUsername);
            SqlDataReader readerUser = cmdUser.ExecuteReader();

            if (readerUser.HasRows)
            {
                while (readerUser.Read())
                {
                    if (readerUser["CustPassword"].ToString().Equals(inputPassword))
                    {
                        // Set session data
                        Session["UserID"] = readerUser["CustID"].ToString();
                        Session["Username"] = readerUser["CustUsername"].ToString();
                        Session["Password"] = readerUser["CustPassword"].ToString();
                        Session["Email"] = readerUser["CustEmail"].ToString();
                        Session["ProfileImgPath"] = readerUser["CustProfileImgPath"].ToString();
                        Session["Role"] = SetCustRole(readerUser["CustRole"].ToString()); // Set the customer role here
                        readerUser.Close();
                        return true;
                    }
                }
                txtPassword.Text = "";
                txtPassword.Focus();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Incorrect Password');", true);
                readerUser.Close();
                return false;
            }
            else
            {
                txtUsername.Text = "";
                txtUsername.Focus();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Username is not found');", true);
                readerUser.Close();
                return false;
            }
        }

        // Event for Username TextChanged
        protected void txtUsername_TextChanged(object sender, EventArgs e)
        {

            VerifyLoginCredentials(); // Call login verification when username changes
        }

        // Event for Password TextChanged
        protected void txtPassword_TextChanged(object sender, EventArgs e)
        {
            VerifyLoginCredentials(); // Call login verification when password changes
        }

        protected String SetCustRole(string custRoleCode)
        {
            switch (custRoleCode)
            {
                case "STD": { return "Student"; }
                case "TUT": { return "Tutor"; }
                default: { return ""; }
            }
        }
    }
}