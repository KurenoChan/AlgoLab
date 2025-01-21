using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace AlgoLab
{
    public partial class AdminIncomeReport : System.Web.UI.Page
    {
        // Variables to hold the values to be shown on the page
        public int NumberOfEnrollments { get; set; }
        public int NumberOfCourses { get; set; }
        public decimal TotalNetIncome { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            if (!IsPostBack)
            {
                // Call the method to get the reports and bind them to a GridView or another control
                List<IncomeReport> reports = GetIncomeReports(connectionString);
                BindReportsToGrid(reports);

            }

        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_AdminIncomeReport.txt");

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

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            // Get the selected month and year from the dropdowns
            int selectedMonth = int.Parse(ddlMonth.SelectedValue);
            int selectedYear = int.Parse(ddlYear.SelectedValue);

            // Get the filtered income reports based on the selected month and year
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<IncomeReport> reports = GetIncomeReports(connectionString, selectedMonth, selectedYear);

            // Bind the filtered data to the GridView
            BindReportsToGrid(reports);
        }

        private List<IncomeReport> GetIncomeReports(string connectionString, int month, int year)
        {
            List<IncomeReport> reports = new List<IncomeReport>();
            decimal totalNetIncome = 0;
            string enrollmentQuery = @"
    SELECT 
        COUNT(DISTINCT CE.CustID) AS NumberOfEnrollments
    FROM 
        [dbo].[CourseEnrolment] CE
    WHERE 
        CE.EnrolmentStatus = 'Active'
        AND MONTH(CE.EnrolmentDate) = @Month
        AND YEAR(CE.EnrolmentDate) = @Year;
    ";

            string courseQuery = @"
    SELECT 
        COUNT(DISTINCT C.CourseID) AS NumberOfCourses
    FROM 
        [dbo].[Course] C;
    ";

            string incomeQuery = @"
    SELECT 
        CA.TutorID AS TutorID,
        C.CourseID AS CourseID,
        C.CourseName AS CourseName,
        C.CourseFee AS CourseFee,
        COUNT(CE.CustID) AS NumberOfStudents,
        (COUNT(CE.CustID) * C.CourseFee) AS TotalIncome,
        DATEPART(YEAR, CE.EnrolmentDate) AS Year,         
        DATEPART(MONTH, CE.EnrolmentDate) AS Month, 
        ((COUNT(CE.CustID) * C.CourseFee) * (1 - TI.Commission)) AS NetIncome,
        SUM((COUNT(CE.CustID) * C.CourseFee) * (1 - TI.Commission)) OVER () AS TotalNetIncome
    FROM 
        [dbo].[CourseAssignment] CA
    INNER JOIN 
        [dbo].[Course] C ON CA.CourseID = C.CourseID
    LEFT JOIN 
        [dbo].[CourseEnrolment] CE ON C.CourseID = CE.CourseID
    INNER JOIN 
        [dbo].[TutorInfo] TI ON CA.TutorID = TI.TutorID
    WHERE 
        CE.EnrolmentStatus = 'Active'
        AND MONTH(CE.EnrolmentDate) = @Month
        AND YEAR(CE.EnrolmentDate) = @Year
    GROUP BY 
         CA.TutorID, C.CourseID, C.CourseName, C.CourseFee, TI.Commission, DATEPART(YEAR, CE.EnrolmentDate), DATEPART(MONTH, CE.EnrolmentDate)
    ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                // Get the total number of enrollments
                using (SqlCommand cmd = new SqlCommand(enrollmentQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        NumberOfEnrollments = Convert.ToInt32(reader["NumberOfEnrollments"]);
                    }
                    reader.Close();
                }

                // Get the total number of courses
                using (SqlCommand cmd = new SqlCommand(courseQuery, connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        NumberOfCourses = Convert.ToInt32(reader["NumberOfCourses"]);
                    }
                    reader.Close();
                }

                // Get the income data
                using (SqlCommand cmd = new SqlCommand(incomeQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var report = new IncomeReport
                        {
                            TutorID = reader["TutorID"].ToString(),
                            CourseID = reader["CourseID"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            CourseFee = Convert.ToDecimal(reader["CourseFee"]),
                            NumberOfStudents = Convert.ToInt32(reader["NumberOfStudents"]),
                            TotalIncome = Convert.ToDecimal(reader["TotalIncome"]),
                            NetIncome = Convert.ToDecimal(reader["NetIncome"]),
                            TotalNetIncome = Convert.ToDecimal(reader["TotalNetIncome"]),
                            Year = Convert.ToInt32(reader["Year"]),
                            Month = Convert.ToInt32(reader["Month"])
                        };

                        reports.Add(report);
                        totalNetIncome = report.TotalNetIncome; // Consistent for all rows
                    }
                    reader.Close();
                }
            }
            TotalNetIncome = totalNetIncome;

            // Return the filtered list of reports
            return reports;
        }


        private List<IncomeReport> GetIncomeReports(string connectionString)
        {
            List<IncomeReport> reports = new List<IncomeReport>();
            decimal totalNetIncome = 0;

            string enrollmentQuery = "SELECT COUNT(DISTINCT CE.CustID) AS NumberOfEnrollments FROM [dbo].[CourseEnrolment] CE WHERE CE.EnrolmentStatus = 'Active';";
            string courseQuery = "SELECT COUNT(DISTINCT C.CourseID) AS NumberOfCourses FROM [dbo].[Course] C;";
            string incomeQuery = @"
        SELECT 
            CA.TutorID AS TutorID,
            C.CourseID AS CourseID,
            C.CourseName AS CourseName,
            C.CourseFee AS CourseFee,
            COUNT(CE.CustID) AS NumberOfStudents,
            (COUNT(CE.CustID) * C.CourseFee) AS TotalIncome,
            ((COUNT(CE.CustID) * C.CourseFee) * (1 - TI.Commission)) AS NetIncome,
            SUM((COUNT(CE.CustID) * C.CourseFee) * (1 - TI.Commission)) OVER () AS TotalNetIncome
        FROM 
            [dbo].[CourseAssignment] CA
        INNER JOIN 
            [dbo].[Course] C ON CA.CourseID = C.CourseID
        LEFT JOIN 
            [dbo].[CourseEnrolment] CE ON C.CourseID = CE.CourseID
        INNER JOIN 
            [dbo].[TutorInfo] TI ON CA.TutorID = TI.TutorID
        WHERE 
            CE.EnrolmentStatus = 'Active'
        GROUP BY 
            CA.TutorID, C.CourseID, C.CourseName, C.CourseFee, TI.Commission;
    ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Get total enrollments
                using (SqlCommand cmd = new SqlCommand(enrollmentQuery, connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        NumberOfEnrollments = Convert.ToInt32(reader["NumberOfEnrollments"]);
                    reader.Close();
                }

                // Get total courses
                using (SqlCommand cmd = new SqlCommand(courseQuery, connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        NumberOfCourses = Convert.ToInt32(reader["NumberOfCourses"]);
                    reader.Close();
                }

                // Get income reports and total net income
                using (SqlCommand cmd = new SqlCommand(incomeQuery, connection))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var report = new IncomeReport
                        {
                            TutorID = reader["TutorID"].ToString(),
                            CourseID = reader["CourseID"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            CourseFee = Convert.ToDecimal(reader["CourseFee"]),
                            NumberOfStudents = Convert.ToInt32(reader["NumberOfStudents"]),
                            TotalIncome = Convert.ToDecimal(reader["TotalIncome"]),
                            NetIncome = Convert.ToDecimal(reader["NetIncome"]),
                            TotalNetIncome = Convert.ToDecimal(reader["TotalNetIncome"])
                        };

                        reports.Add(report);
                        totalNetIncome = report.TotalNetIncome; // Consistent for all rows
                    }
                    reader.Close();
                }
            }

            TotalNetIncome = totalNetIncome;
            return reports;
        }


        // Method to bind the data to a GridView or another control
        private void BindReportsToGrid(List<IncomeReport> reports)
        {
            // Assuming you have a GridView with ID "GridViewReports"
            GridViewReports.DataSource = reports;
            GridViewReports.DataBind();
        }

        // IncomeReport class to hold the data
        public class IncomeReport
        {
            public string TutorID { get; set; }
            public string CourseID { get; set; }
            public string CourseName { get; set; }
            public decimal CourseFee { get; set; }
            public int NumberOfStudents { get; set; }
            public decimal TotalIncome { get; set; }
            public decimal NetIncome { get; set; }
            public decimal TotalNetIncome { get; set; }
            public decimal TotalMonthlyNetIncome { get; set; }
            public int Year { get; set; }  // Add Year
            public int Month { get; set; } // Add Month
        }
    }
}
