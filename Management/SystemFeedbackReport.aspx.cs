using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace AlgoLab.Management
{
    public partial class SystemFeedbackReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Fetch average ratings from the database
                var ratings = GetAverageRatings();

                // Pass ratings to JavaScript
                string script = $@"
                    const chartData = {{
                        contentRating: {ratings.ContentAvg},
                        instructorRating: {ratings.InstructorAvg},
                        platformRating: {ratings.PlatformAvg},
                        performanceRating: {ratings.PerformanceAvg}
                    }};
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "chartDataScript", script, true);


                // Load the latest 4 comments
                LoadLatestComments();
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_SystemFeedbackReport.txt");

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

        private (double ContentAvg, double InstructorAvg, double PlatformAvg, double PerformanceAvg) GetAverageRatings()
        {
            double contentAvg = 0, instructorAvg = 0, platformAvg = 0, performanceAvg = 0;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                string query = @"
                    SELECT 
                        AVG(ContentRating) AS ContentAvg,
                        AVG(InstructorRating) AS InstructorAvg,
                        AVG(PlatformRating) AS PlatformAvg,
                        AVG(PerformanceRating) AS PerformanceAvg
                    FROM SystemFeedback";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    contentAvg = reader.IsDBNull(0) ? 0 : (double)reader.GetDecimal(0);
                    instructorAvg = reader.IsDBNull(1) ? 0 : (double)reader.GetDecimal(1);
                    platformAvg = reader.IsDBNull(2) ? 0 : (double)reader.GetDecimal(2);
                    performanceAvg = reader.IsDBNull(3) ? 0 : (double)reader.GetDecimal(3);
                }
            }

            return (contentAvg, instructorAvg, platformAvg, performanceAvg);
        }

        private void LoadLatestComments()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT
            c.CustUsername AS CustomerName, 
            sf.SysFeedbackDatetime AS FeedbackDateTime, 
            sf.FeedbackDetails AS Comment
        FROM SystemFeedback sf
        INNER JOIN Customer c ON sf.CustID = c.CustID
        WHERE sf.FeedbackDetails IS NOT NULL AND sf.FeedbackDetails != ''
        ORDER BY sf.SysFeedbackDatetime DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                RepeaterCustomerComments.DataSource = reader;
                RepeaterCustomerComments.DataBind();
            }
        }





    }
}
