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
    public partial class AlgoLabMaster : System.Web.UI.MasterPage
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                con.Open();

                LoadNavCourseContent();

                con.Close();

                // Anonymous User
                if (Session["Role"] == null)
                {
                    NavLogin.Visible = true;
                    NavSignup.Visible = true;
                    NavBecomeTutor.Visible = false;
                    NavProfile.Visible = false;
                    NavAdmin.Visible = false;
                }
                else // Registered User
                {
                    NavLogin.Visible = false;
                    NavSignup.Visible = false;


                    if (Session["Role"].ToString().Equals("Student")) // Student
                    {
                        NavBecomeTutor.Visible = true;
                        NavProfile.Visible = true;
                        NavAdmin.Visible = false;
                        imgBtnProfile.Visible = true;
                        LoginLink.Visible = false;
                        SignupLink.Visible = false;
                        imgBtnProfile.ImageUrl = ResolveUrl(Session["ProfileImgPath"].ToString());
                    }
                    else if (Session["Role"].ToString().Equals("Tutor"))// Tutor or Admin
                    {
                        NavBecomeTutor.Visible = false;
                        NavProfile.Visible = true;
                        LoginLink.Visible = false;
                        SignupLink.Visible = false;
                        BecomeTutorLink.Visible = false;
                        NavAdmin.Visible = false;
                        imgBtnProfile.Visible = true;
                        imgBtnProfile.ImageUrl = ResolveUrl(Session["ProfileImgPath"].ToString());
                    }
                    else if (Session["Role"].ToString().Equals("Admin"))
                    {
                        NavBecomeTutor.Visible = false;
                        LearnUs.Visible = false;
                        NavProfile.Visible = true;
                        NavAdmin.Visible = true;
                        imgBtnProfile.Visible = false;
                    }
                }
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_AlgoLabMaster.txt");

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

        protected void btnNavSearch_Click(object sender, EventArgs e)
        {
            // Get the search keyword from the textbox
            string searchCourse = txtNavSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchCourse))
            {
                // Construct the URL with the query string
                string redirectUrl = "~/Course.aspx?searchCourse=" + Server.UrlEncode(searchCourse) + "#courseCatalog";

                // Redirect to Course.aspx with the search keyword
                Response.Redirect(redirectUrl);
            }
        }


        // Load Course Tab Dropdown Content on Navigation Bar
        protected void LoadNavCourseContent()
        {
            // Load Popular Courses
            LoadPopularCourse();
            // Load Free Courses
            LoadFreeCourse();
            // Load New Courses
            LoadNewCourse();
        }

        // Load Popular Courses
        private void LoadPopularCourse()
        {
            SqlCommand cmdPopular = new SqlCommand(
                "SELECT DISTINCT Course.* " +
                "FROM (" +
                "      SELECT TOP 4 Course.CourseID " +
                "      FROM Course " +
                "      LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
                "      GROUP BY Course.CourseID " +
                "      ORDER BY COUNT(CourseEnrolment.CourseRegNo) DESC" +
                ") AS TopCourses " +
                "INNER JOIN Course ON Course.CourseID = TopCourses.CourseID;", con);

            SqlDataReader readerPopular = cmdPopular.ExecuteReader();
            if (readerPopular.HasRows)
            {
                NavPopularCourse.Visible = true;
                DisplayPopularCourse(readerPopular);
            }
            else
            {
                NavPopularCourse.Visible = false;
            }
            readerPopular.Close();
        }

        // Load Free Courses
        private void LoadFreeCourse()
        {
            SqlCommand cmdFree = new SqlCommand(
                "SELECT DISTINCT Course.* " +
                "FROM (" +
                "      SELECT TOP 4 CourseID " +
                "      FROM Course " +
                "      WHERE CourseFee = 0 " +
                "      ORDER BY CourseID" +
                ") AS TopFreeCourses " +
                "INNER JOIN Course ON TopFreeCourses.CourseID = Course.CourseID " +
                "ORDER BY Course.CourseID;", con);

            SqlDataReader readerFree = cmdFree.ExecuteReader();
            if (readerFree.HasRows)
            {
                NavFreeCourse.Visible = true;
                DisplayFreeCourse(readerFree);
            }
            else
            {
                NavFreeCourse.Visible = false;
            }
            readerFree.Close();
        }

        // Load New Courses
        private void LoadNewCourse()
        {
            SqlCommand cmdNew = new SqlCommand(
                "SELECT DISTINCT Course.* " +
                "FROM (" +
                "      SELECT TOP 4 CourseID " +
                "      FROM Course " +
                "      ORDER BY CourseCreationDatetime DESC" +
                ") AS TopNewCourses " +
                "INNER JOIN Course ON TopNewCourses.CourseID = Course.CourseID " +
                "ORDER BY Course.CourseCreationDatetime DESC;", con);

            SqlDataReader readerNew = cmdNew.ExecuteReader();
            if (readerNew.HasRows)
            {
                NavNewCourse.Visible = true;
                DisplayNewCourse(readerNew);
            }
            else
            {
                NavNewCourse.Visible = false;
            }
            readerNew.Close();
        }




        // Display Popular Courses
        protected void DisplayPopularCourse(SqlDataReader readerPopular)
        {
            string popularCourseContent = string.Empty;

            while (readerPopular.Read())
            {
                popularCourseContent += $@"
                        <div class='nav_course_card {CourseLevel_ToClassName(readerPopular["CourseLevel"].ToString())}'>
                            <a href='{Page.ResolveUrl($"~/CourseDetails.aspx?courseId={readerPopular["CourseID"].ToString()}")}' >
                                <div class='nav_course_cardImg'>
                                    <img src='{Page.ResolveUrl(readerPopular["CourseIconPath"].ToString())}' alt='{readerPopular["CourseName"].ToString()}' />
                                </div>
                                <div class='nav_course_card_descWrapper'>
                                    <p class='nav_course_card_title'>{readerPopular["CourseName"].ToString()}</p>
                                    <p class='nav_course_card_desc'>{readerPopular["CourseShortDesc"].ToString()}</p>
                                </div>
                            </a>
                        </div>";
            }

            NavPopularCourse.InnerHtml = popularCourseContent;
        }
        // Display Free Courses
        protected void DisplayFreeCourse(SqlDataReader readerFree)
        {
            string freeCourseContent = string.Empty;

            while (readerFree.Read())
            {
                freeCourseContent += $@"
                        <div class='nav_course_card {CourseLevel_ToClassName(readerFree["CourseLevel"].ToString())} course_free'>
                            <a href='{Page.ResolveUrl($"~/CourseDetails.aspx?courseId={readerFree["CourseID"].ToString()}")}' >
                                <div class='nav_course_cardImg'>
                                    <img src='{Page.ResolveUrl(readerFree["CourseIconPath"].ToString())}' alt='{readerFree["CourseName"].ToString()}' />
                                </div>
                                <div class='nav_course_card_descWrapper'>
                                    <p class='nav_course_card_title'>{readerFree["CourseName"].ToString()}</p>
                                    <p class='nav_course_card_desc'>{readerFree["CourseShortDesc"].ToString()}</p>
                                </div>
                            </a>
                        </div>";
            }

            NavFreeCourse.InnerHtml = freeCourseContent;
        }
        // Display New Courses
        protected void DisplayNewCourse(SqlDataReader readerNew)
        {
            string newCourseContent = string.Empty;

            while (readerNew.Read())
            {
                newCourseContent += $@"
                        <div class='nav_course_card {CourseLevel_ToClassName(readerNew["CourseLevel"].ToString())} course_new'>
                            <a href='{Page.ResolveUrl($"~/CourseDetails.aspx?courseId={readerNew["CourseID"].ToString()}")}' >
                                <div class='nav_course_cardImg'>
                                    <img src='{Page.ResolveUrl(readerNew["CourseIconPath"].ToString())}' alt='{readerNew["CourseName"].ToString()}' />
                                </div>
                                <div class='nav_course_card_descWrapper'>
                                    <p class='nav_course_card_title'>{readerNew["CourseName"].ToString()}</p>
                                    <p class='nav_course_card_desc'>{readerNew["CourseShortDesc"].ToString()}</p>
                                </div>
                            </a>
                        </div>";
            }

            NavNewCourse.InnerHtml = newCourseContent;

        }


        // Convert Level Code To Full Text
        public static String CourseLevel_ToClassName(String levelCode)
        {
            switch (levelCode)
            {
                case "BEG":
                    return "course_beginner";
                case "ITM":
                    return "course_intermediate";
                case "ADV":
                    return "course_advanced";
                default:
                    return "";
            }
        }

        protected void imgBtnProfile_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/User/Profile.aspx");
        }

        protected void btnLogout_Click1(object sender, EventArgs e)
        {
            // Clear the session
            Session.Clear();

            // Abandon the session to ensure all session data is removed
            Session.Abandon();

            // Redirect to the login page or homepage
            Response.Redirect("~/AccountSelect.aspx");
        }
    }
}