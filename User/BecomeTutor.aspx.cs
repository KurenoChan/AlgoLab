using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlgoLab
{
    public partial class BecomeTutor : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                string returnUrl = HttpContext.Current.Request.Url.PathAndQuery; // Get the full path and query string of the current page
                Response.Redirect($"../AccountSelect.aspx?ReturnUrl={HttpUtility.UrlEncode(returnUrl)}");
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_BecomeTutor.txt");

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

        protected void Submit_Click(object sender, EventArgs e)
        {
            string tutorID = Session["UserID"]?.ToString();

            if (string.IsNullOrEmpty(tutorID))
            {
                DisplayError("Invalid session. Please log in again.");
                return;
            }

            // Check if the user has already submitted a request
            if (IsTutorApplicationExists(tutorID))
            {
                DisplayError("You have already submitted an application. Please wait for an email regarding your request's status.");
                ClearForm();
                return;
            }

            string TutorInfoID = GenerateUniqueTutorID();
            string expertiseArea = Expertise_Area.Text.Trim();
            string educationLevel = Education_Level.Text.Trim();
            string bio = Bio.Text.Trim();
            string jobRole = Job_Role.Text.Trim();
            string company = Company.Text.Trim();
            string requestStatus = "Pending";

            // Validate inputs
            if (string.IsNullOrEmpty(expertiseArea) || string.IsNullOrEmpty(educationLevel) || string.IsNullOrEmpty(bio))
            {
                DisplayError("Please fill in all required fields.");
                return;
            }

            // Handle file upload
            string certificationPath = string.Empty;
            if (Certification.HasFile)
            {
                string fileExtension = Path.GetExtension(Certification.FileName).ToLower();
                string[] allowedExtensions = { ".pdf", ".doc", ".docx" };

                if (Array.Exists(allowedExtensions, ext => ext == fileExtension))
                {
                    string fileName = $"{Guid.NewGuid()}{fileExtension}";
                    string tutorFolderPath = $"/Assets/Images/BecomeTutor/{TutorInfoID}/";
                    string serverFolderPath = Server.MapPath(tutorFolderPath);

                    try
                    {
                        if (!Directory.Exists(serverFolderPath))
                        {
                            Directory.CreateDirectory(serverFolderPath);
                        }

                        string serverFilePath = Path.Combine(serverFolderPath, fileName);
                        Certification.SaveAs(serverFilePath);

                        certificationPath = Path.Combine(tutorFolderPath, fileName).Replace("\\", "/");
                    }
                    catch (Exception ex)
                    {
                        DisplayError($"Error uploading file: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    DisplayError("Only PDF or Word documents are allowed.");
                    return;
                }
            }
            else
            {
                DisplayError("Please upload a certification file.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    conn.Open();

                    string sql = @"
            INSERT INTO TutorInfo (TutorInfoID, JobTitle, Company, ExpertiseArea, EducationLevel, Bio, Certification, RequestStatus, TutorID)
            VALUES (@TutorInfoID, @JobTitle, @Company, @ExpertiseArea, @EducationLevel, @Bio, @Certification, @RequestStatus, @TutorID)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TutorInfoID", TutorInfoID);
                        cmd.Parameters.AddWithValue("@JobTitle", jobRole);
                        cmd.Parameters.AddWithValue("@Company", company);
                        cmd.Parameters.AddWithValue("@ExpertiseArea", expertiseArea);
                        cmd.Parameters.AddWithValue("@EducationLevel", educationLevel);
                        cmd.Parameters.AddWithValue("@Bio", bio);
                        cmd.Parameters.AddWithValue("@Certification", certificationPath);
                        cmd.Parameters.AddWithValue("@RequestStatus", requestStatus);
                        cmd.Parameters.AddWithValue("@TutorID", tutorID);

                        cmd.ExecuteNonQuery();
                    }
                }

                string email = Session["Email"]?.ToString();
                if (!string.IsNullOrEmpty(email))
                {
                    SendEmailNotification(email, TutorInfoID);
                }

                DisplaySuccess("Your application has been submitted successfully. Your request status is 'Pending'.");
                ClearForm();

                string script = "<script type='text/javascript'>" +
                                    "alert('Request Submitted. Check your email for approval.');" +
                                    "window.location = '" + Page.ResolveUrl("~/Home.aspx") + "';" +
                                "</script>";

                ClientScript.RegisterStartupScript(this.GetType(), "Check Email Alert", script, true);
            }
            catch (Exception ex)
            {
                DisplayError($"Error submitting application: {ex.Message}");
            }
        }

        private string GenerateUniqueTutorID()
        {
            string prefix = "TID-";
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"); // Get current timestamp
            string newTutorID = $"{prefix}{timestamp}-0001"; // Start with the default sequence number (0001)

            // Get all existing CourseFeedbackIDs from the database and check for uniqueness
            List<string> existingFeedbackIDs = GetAllExistingTutorIDs();

            // Keep generating a new ID until it's unique
            while (existingFeedbackIDs.Contains(newTutorID))
            {
                // Increment the sequence number to avoid duplication
                string lastSequence = newTutorID.Split('-')[1]; // Get the sequence part (after the second '-')
                int nextSeq = int.Parse(lastSequence) + 1; // Increment the sequence number
                newTutorID = $"{prefix}{timestamp}-{nextSeq:D4}"; // Ensure the sequence is always 4 digits (e.g., 0001, 0002)
            }

            return newTutorID;
        }
        private List<string> GetAllExistingTutorIDs()
        {
            List<string> TutorIDs = new List<string>();

            // Query to get all CourseFeedbackIDs
            SqlCommand cmd = new SqlCommand("SELECT CustID FROM TutorInfo", conn);
            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string TutorID = reader["TutorInfoID"].ToString();
                    TutorIDs.Add(TutorID); // Add each ID to the list
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

            return TutorIDs;
        }
        private void DisplayError(string message)
        {
            UploadStatusLabel.Text = message;
            UploadStatusLabel.ForeColor = System.Drawing.Color.Red;
        }

        private void DisplaySuccess(string message)
        {
            UploadStatusLabel.Text = message;
            UploadStatusLabel.ForeColor = System.Drawing.Color.Green;
        }

        private void ClearForm()
        {
            Expertise_Area.Text = string.Empty;
            Education_Level.Text = string.Empty;
            Bio.Text = string.Empty;
            Job_Role.Text = string.Empty;
            Company.Text = string.Empty;
            Certification.Attributes.Clear();
        }
        private void SendEmailNotification(string recipientEmail, string tutorID)
        {
            try
            {
                string smtpUserName = ConfigurationManager.AppSettings["smtpUserName"];
                string smtpPassword = ConfigurationManager.AppSettings["smtpPassword"];
                string smtpClient = ConfigurationManager.AppSettings["smtpClient"];
                int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]);
                string subject = "Tutor Application Submitted";
                string body = $@"
            <html>
                 <body style='font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; padding: 20px;'>
                        <div style='max-width: 600px; margin: 0 auto; background-color: rgb(0,0,0,0.8); padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
                            <p style='font-size: 16px; line-height: 1.6; text-align: center; color: #fff;'>Your application is currently under review.We will notify you once the status is updated.</p>
                            <div style='text-align: center;'>
                                <p style='font-size: 14px; color: #888;'>Need help? Our support team is just a message away!</p>
                                <p style='color: #777;'><a href='mailto:algolab17@gmail.com' style='color: #aaa;'>Email us</a> or <a href='tel:+1072335546' style='color: #aaa;'>Contact Us</a> here</p>
                            </div>
                        </div>
                </body>
            </html>";

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(smtpUserName, "AlgoLab"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(recipientEmail);

                SmtpClient smtp = new SmtpClient(smtpClient, smtpPort)
                {
                    Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword),
                    EnableSsl = enableSSL
                };


                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in sending email: {ex.Message}");
            }
        }
        private bool IsTutorApplicationExists(string tutorID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM TutorInfo WHERE TutorID = @TutorID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TutorID", tutorID);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

    }
}