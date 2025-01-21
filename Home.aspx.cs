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
    public partial class Home : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                con.Open();
                LoadFeaturedCourse();
                con.Close();

                SignupBecomeTut.Visible = true;

                string signupBecomeTut_Content = @"
                            <h3 class='sectionSubheading'>Sign Up Now</h3>
                            <a href='SignUp.aspx'>
                                <div class='btnToPage'>
                                    <p>Get Started</p>
                                </div>
                            </a>";

                if (Session["Role"] != null)
                {
                    if (Session["Role"].ToString().Equals("Student")) // student view
                    {
                        signupBecomeTut_Content = @"
                            <h3 class='sectionSubheading'>Share Your Skills</h3>
                            <a href='User/BecomeTutor.aspx'>
                                <div class='btnToPage'>
                                    <p>Join as Tutor</p>
                                </div>
                            </a>";


                        SignupBecomeTut.InnerHtml = signupBecomeTut_Content;
                    }
                    else
                    {
                        SignupBecomeTut.Visible = false;
                    }
                }
                else
                {
                    SignupBecomeTut.InnerHtml = signupBecomeTut_Content;
                }

            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_Home.txt");

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

        protected void btnHomeSearch_Click(object sender, EventArgs e)
        {
            // Get the search keyword from the textbox
            string searchCourse = txtHomeSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchCourse))
            {
                // Construct the URL with the query string
                string redirectUrl = "~/Course.aspx?searchCourse=" + Server.UrlEncode(searchCourse);

                // Redirect to Course.aspx with the search keyword
                Response.Redirect(redirectUrl);
            }
        }

        // Load Featured Course
        private void LoadFeaturedCourse()
        {
            SqlCommand cmdFeatured = new SqlCommand(
                "SELECT DISTINCT Course.*, AVG(CourseFeedback.CourseFeedbackRating) AS CourseRating " +
                "FROM (" +
                "      SELECT TOP 3 Course.CourseID " +
                "      FROM Course " +
                "      LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
                "      GROUP BY Course.CourseID " +
                "      ORDER BY COUNT(CourseEnrolment.CourseRegNo) DESC" +
                ") AS TopCourses " +
                "INNER JOIN Course ON Course.CourseID = TopCourses.CourseID " +
                "INNER JOIN CourseFeedback ON TopCourses.CourseID = CourseFeedback.CourseID " +
                "GROUP BY " +
                "       Course.CourseID, Course.CourseName, Course.CourseShortDesc, Course.CourseDesc, " +
                "       Course.CourseObj, Course.CourseLevel, Course.CourseLang, Course.CourseFee, Course.CourseCreationDatetime, " +
                "       Course.CourseImgPath, Course.CourseIconPath " +
                "ORDER BY CourseRating DESC;", con);

            SqlDataReader readerFeatured = cmdFeatured.ExecuteReader();
            if (readerFeatured.HasRows)
            {
                DisplayFeaturedCourse(readerFeatured);
            }
            readerFeatured.Close();
        }

        // Display Featured Course
        private void DisplayFeaturedCourse(SqlDataReader readerFeatured)
        {
            string featuredWrapperContent = string.Empty;

            while (readerFeatured.Read())
            {
                featuredWrapperContent += $@"
                    <div class='featured_box {AlgoLabMaster.CourseLevel_ToClassName(readerFeatured["CourseLevel"].ToString())}'>
                        <a href='CourseDetails.aspx?courseId={readerFeatured["CourseID"].ToString()}'>
                            <div class='featured_card'>
                                <div class='featured_img' style='background-image: url(""{Page.ResolveUrl(readerFeatured["CourseImgPath"].ToString())}"");'></div>
                                <div class='featured_descBox'>
                                    <h2>{readerFeatured["CourseName"].ToString()}</h2>
                                    <div class='featured_rate'>
                                        <i class=""fa fa-star""></i>&nbsp;&nbsp;{Convert.ToDouble(readerFeatured["CourseRating"]).ToString("F1")} / 5.0
                                    </div>
                                    <div class='featured_desc'>
                                        <p>{readerFeatured["CourseShortDesc"].ToString()}</p>
                                    </div>
                                    <div class='btnToPage'>
                                        <p>Explore Course</p>
                                    </div>
                                </div>
                            </div>
                        </a>
                    </div>";
            }

            FeaturedWrapper.InnerHtml = featuredWrapperContent;
        }
    }
}