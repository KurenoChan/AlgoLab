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
    public partial class Feedback : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            con.Open();

            // Check if user is logged in, redirect if not
            if (Session["UserID"] == null)
            {
                string returnUrl = HttpContext.Current.Request.Url.PathAndQuery; // Get the full path and query string of the current page
                Response.Redirect($"../AccountSelect.aspx?ReturnUrl={HttpUtility.UrlEncode(returnUrl)}");
            }
            if (!IsPostBack)
            {
                ErrorTooltipWrapper.Visible = false;
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_Feedback.txt");

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

        protected void btnSubmitFeedback_Click(object sender, EventArgs e)
        {
            string errorTooltipWrapperInnerHTML = string.Empty;

            // Check if each RadioButtonList has a selected value
            if (string.IsNullOrEmpty(rblContentRating.SelectedValue))
            {
                errorTooltipWrapperInnerHTML += setErrorTooltipMsg("Please select a rating for course content.");
            }
            if (string.IsNullOrEmpty(rblInstructorRating.SelectedValue))
            {
                errorTooltipWrapperInnerHTML += setErrorTooltipMsg("Please select a rating for the instructor.");
            }
            if (string.IsNullOrEmpty(rblPlatformRating.SelectedValue))
            {
                errorTooltipWrapperInnerHTML += setErrorTooltipMsg("Please select a rating for the platform.");
            }
            if (string.IsNullOrEmpty(rblPerformanceRating.SelectedValue))
            {
                errorTooltipWrapperInnerHTML += setErrorTooltipMsg("Please select a rating for your learning performance.");
            }

            if (!string.IsNullOrEmpty(errorTooltipWrapperInnerHTML))
            {
                ErrorTooltipWrapper.Visible = true;
                ErrorTooltipWrapper.InnerHtml = errorTooltipWrapperInnerHTML;
            }
            else
            {
                ErrorTooltipWrapper.Visible = false;

                // Write to Database
                if (SaveFeedback())
                {
                    // Display a success message using JavaScript alert and then redirect
                    string script = "<script type='text/javascript'>" +
                                    "alert('Thank you for your feedback!');" +
                                    "window.location = '" + Page.ResolveUrl("~/Home.aspx") + "';" +
                                    "</script>";
                    Response.Write(script);

                    // Clear selections and feedback fields
                    rblContentRating.ClearSelection();
                    rblInstructorRating.ClearSelection();
                    rblPlatformRating.ClearSelection();
                    rblPerformanceRating.ClearSelection();
                    txtFeedbackDetails.Text = "";
                }

            }

        }

        // Helper method to clear tooltips
        private string setErrorTooltipMsg(string errorMsg)
        {
            return
                $"<div class='errorTooltip'>" +
                $"  <div class='errorTooltipImg'></div>" +
                $"  <p class='errorTooltipMsg'>{errorMsg}</p>" +
                $"</div>";
        }

        private bool SaveFeedback()
        {
            string sysFeedbackID = GenerateUniqueSysFeedbackID();
            string contentRating = rblContentRating.SelectedValue.ToString();
            string instructorRating = rblInstructorRating.SelectedValue.ToString();
            string platformRating = rblPlatformRating.SelectedValue.ToString();
            string performanceRating = rblPerformanceRating.SelectedValue.ToString();
            string feedbackDetails = txtFeedbackDetails.Text.ToString();
            string custID = Session["UserID"].ToString();

            SqlCommand cmd = new SqlCommand(
                "INSERT INTO SystemFeedback (SysFeedbackID, ContentRating, InstructorRating, PlatformRating, PerformanceRating, FeedbackDetails, CustID) " +
                "VALUES (@SysFeedbackID, @ContentRating, @InstructorRating, @PlatformRating, @PerformanceRating, @FeedbackDetails, @CustID);", con);

            cmd.Parameters.AddWithValue("@SysFeedbackID", sysFeedbackID);
            cmd.Parameters.AddWithValue("@ContentRating", contentRating);
            cmd.Parameters.AddWithValue("@InstructorRating", instructorRating);
            cmd.Parameters.AddWithValue("@PlatformRating", platformRating);
            cmd.Parameters.AddWithValue("@PerformanceRating", performanceRating);
            cmd.Parameters.AddWithValue("@FeedbackDetails", feedbackDetails);
            cmd.Parameters.AddWithValue("@CustID", custID);

            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Feedback Insertion Failed",
                        $"alert('Error in saving feedback: {ex}')", true);
                return false;
            }

        }

        private string GenerateUniqueSysFeedbackID()
        {
            string prefix = "SYSFDB-";
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");  // Use the passed transactionDatetime for the timestamp
            string newSysFeedbackID = $"{prefix}{timestamp}-0001"; // Start with the default sequence number (0001)

            // Get all existing TransactionNos from the database and check for uniqueness
            List<string> existingSysFeedbackIDs = GetAllExistingSysFeedbackID();

            // Keep generating a new ID until it's unique
            while (existingSysFeedbackIDs.Contains(newSysFeedbackID))
            {
                // Increment the sequence number to avoid duplication
                string lastSequence = newSysFeedbackID.Split('-')[1]; // Get the sequence part (after the second '-')
                int nextSeq = int.Parse(lastSequence) + 1; // Increment the sequence number
                newSysFeedbackID = $"{prefix}{timestamp}-{nextSeq:D4}"; // Ensure the sequence is always 4 digits (e.g., 0001, 0002)
            }

            return newSysFeedbackID;
        }

        private List<string> GetAllExistingSysFeedbackID()
        {
            List<string> sysFeedbackIDs = new List<string>();

            // Query to get all TransactioNos
            SqlCommand cmd = new SqlCommand("SELECT SysFeedbackID FROM SystemFeedback", con);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string sysFeedbackID = reader["SysFeedbackID"].ToString();
                    sysFeedbackIDs.Add(sysFeedbackID); // Add each ID to the list
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
                con.Close(); // Ensure the connection is closed
            }

            return sysFeedbackIDs;
        }

    }
}