using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlgoLab
{
    public partial class AccountSelect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_AccountSelect.txt");

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

        protected void btnCustomer_Click(object sender, EventArgs e)
        {
            string redirectUrl = "~/Login.aspx?role=customer";
            if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                redirectUrl += $"&ReturnUrl={HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"].ToString())}";
            }

            Response.Redirect(redirectUrl);
        }

        protected void btnStaff_Click(object sender, EventArgs e)
        {
            string redirectUrl = "~/Login.aspx?role=staff";
            if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                redirectUrl += $"&ReturnUrl={HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"].ToString())}";
            }

            Response.Redirect(redirectUrl);
        }
    }
}