using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AlgoLab;
using static System.Collections.Specialized.BitVector32;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AlgoLab
{
    public partial class ApproveTutor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTutors();
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_ApproveTutor.txt");

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

        private void LoadTutors()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = @"
    SELECT 
        T.TutorInfoID, 
        T.ExpertiseArea, 
        T.EducationLevel, 
        T.Certification, 
        T.Bio, 
        T.BonusRate, 
        T.Commission, 
        C.CustUsername, 
        C.CustFname, 
        C.CustLname
    FROM 
        TutorInfo T
    INNER JOIN 
        Customer C
    ON 
        T.TutorID = C.CustID
    WHERE 
        T.RequestStatus = 'Pending'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                StringBuilder html = new StringBuilder();
                if (!reader.HasRows)
                {
                    html.Append("<div class='NoneRequest'>");
                    html.Append("<p class='NoneRequestMessage'>No pending requests to process.</p>");
                    html.Append("</div>");
                }
                else
                {
                    while (reader.Read())
                    {
                        html.Append("<div class='box'>");
                        html.AppendFormat("<div class='Request_container' id='TutorInfo-{0}'>", reader["TutorInfoID"]);
                        html.Append("<div class='tutor-box'>");
                        html.AppendFormat("<p><strong>Username:</strong> {0}</p>", reader["CustUsername"]);
                        html.AppendFormat("<p><strong>First Name:</strong> {0}</p>", reader["CustFname"]);
                        html.AppendFormat("<p><strong>Last Name:</strong> {0}</p>", reader["CustLname"]);
                        html.AppendFormat("<p><strong>Expertise Area:</strong> {0}</p>", reader["ExpertiseArea"]);
                        html.AppendFormat("<p><strong>Certification:</strong> <a href='{0}' target='_blank' class='certification-link'>View Certification</a></p>", reader["Certification"] ?? "#");
                        html.Append("</div>");
                        html.Append("<div class='ViewBtn'>");
                        html.AppendFormat("<button class='ViewDetailBtn' type='button' onclick='showDetailWindow(\"{0}\")'>View Detail</button>", reader["TutorInfoID"]);
                        html.Append("</div>");
                        html.Append("</div>");
                        html.AppendFormat("<div class='DetailWindow collapsed' id='Detail-{0}'>", reader["TutorInfoID"]);
                        html.AppendFormat("<h3>Tutor ID: {0}</h3>", reader["TutorInfoID"]);
                        html.AppendFormat("<p><strong>Expertise Area:  </strong> {0}</p>", reader["ExpertiseArea"]);
                        html.AppendFormat("<p><strong>Education Level:</strong> {0}</p>", reader["EducationLevel"] ?? "N/A");

                        html.AppendFormat("<p><strong>Bio:</strong> {0}</p>", reader["Bio"] ?? "N/A");

                        // Input Fields
                        html.Append("<div class='input-group'>");
                        html.AppendFormat("<label for='BonusRate-{0}'>Bonus Rate:</label>", reader["TutorInfoID"]);
                        html.AppendFormat("<input type='number' id='BonusRate-{0}' name='BonusRate-{0}' class='BonusRateInput' placeholder='Enter bonus rate' />", reader["TutorInfoID"]);

                        html.AppendFormat("<label for='Commission-{0}'>Commission:</label>", reader["TutorInfoID"]);
                        html.AppendFormat("<select id='Commission-{0}' name='Commission-{0}' class='CommissionInput'>", reader["TutorInfoID"]);
                        for (decimal i = 0.10M; i <= 0.50M; i += 0.05M)
                        {
                            html.AppendFormat("<option value='{0:F2}'>{0:F2}</option>", i);
                        }
                        html.Append("</select>");
                        html.Append("</div>");

                        // Decision Buttons
                        html.Append("<div class='Decide_btn'>");
                        html.AppendFormat("<button class='DecideBtn Approve' onclick='handleAction(\"{0}\", \"Approve\")'>Approve</button>", reader["TutorInfoID"]);
                        html.AppendFormat("<button class='DecideBtn Reject' onclick='handleAction(\"{0}\", \"Reject\")'>Reject</button>", reader["TutorInfoID"]);
                        html.Append("</div>");
                        html.Append("</div>");
                        html.Append("</div>");
                    }
                }

                Approve_Container.InnerHtml = html.ToString();
            }
        }
        [System.Web.Services.WebMethod]
        public static string ApproveRequest(string tutorInfoID, decimal? newBonusRate = null, decimal? newCommission = null)
        {
            string result;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                string email = string.Empty;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Update TutorInfo and get the user's email
                    string updateQuery = @"
                UPDATE TutorInfo
                SET 
                    RequestStatus = 'Approved',
                    BonusRate = ISNULL(@NewBonusRate, BonusRate), 
                    Commission = ISNULL(@NewCommission, Commission)
                WHERE TutorInfoID = @TutorInfoID;

                UPDATE Customer
                SET CustRole = 'TUT'
                WHERE CustID = (SELECT TutorID FROM TutorInfo WHERE TutorInfoID = @TutorInfoID);

                SELECT C.CustEmail
                FROM Customer C
                INNER JOIN TutorInfo T ON C.CustID = T.TutorID
                WHERE T.TutorInfoID = @TutorInfoID;";

                    SqlCommand command = new SqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@TutorInfoID", tutorInfoID);
                    command.Parameters.AddWithValue("@NewBonusRate", (object)newBonusRate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NewCommission", (object)newCommission ?? DBNull.Value);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            email = reader["CustEmail"].ToString();
                        }
                    }
                }

                // Send email notification
                if (!string.IsNullOrEmpty(email))
                {
                    SendNotificationEmail(email, "Approved", tutorInfoID);
                }

                result = "Tutor Approved Successfully with updated information.";
            }
            catch (Exception ex)
            {
                result = $"Error: {ex.Message}";
            }

            return result;
        }

        [System.Web.Services.WebMethod]
        public static string RejectRequest(string tutorInfoID)
        {
            string result;
            string email = string.Empty;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Update RequestStatus and get the user's email
                    string updateQuery = @"
                UPDATE TutorInfo
                SET RequestStatus = 'Rejected'
                WHERE TutorInfoID = @TutorInfoID;

                SELECT C.CustEmail
                FROM Customer C
                INNER JOIN TutorInfo T ON C.CustID = T.TutorID
                WHERE T.TutorInfoID = @TutorInfoID;";

                    SqlCommand command = new SqlCommand(updateQuery, connection);
                    command.Parameters.AddWithValue("@TutorInfoID", tutorInfoID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            email = reader["CustEmail"].ToString();
                        }
                    }
                }

                // Send email notification
                if (!string.IsNullOrEmpty(email))
                {
                    SendNotificationEmail(email, "Rejected", tutorInfoID);
                }

                result = "Tutor Rejected Successfully.";
            }
            catch (Exception ex)
            {
                result = $"Error: {ex.Message}";
            }

            return result;
        }
        private static void SendNotificationEmail(string recipientEmail, string status, string tutorInfoID)
        {
            try
            {
                string smtpUserName = ConfigurationManager.AppSettings["smtpUserName"];
                string smtpPassword = ConfigurationManager.AppSettings["smtpPassword"];
                string smtpClient = ConfigurationManager.AppSettings["smtpClient"];
                int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]);

                string subject = $"Your Tutor Application Request is {status}";
                string body = $@"
<html>
                 <body style='font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; padding: 20px;'>
                        <div style='max-width: 600px; margin: 0 auto; background-color: rgb(0,0,0,0.8); padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
                            <p style='font-size: 16px; line-height: 1.6; text-align: center; color: #fff;'>Your tutor application with ID <strong>{tutorInfoID}</strong> has been <strong>{status}</strong>.</p>
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
    }
}
