using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace AlgoLab
{
    public partial class CourseDetails : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            con.Open();
            // Get Course ID from the query string
            string courseID = Request.QueryString["courseId"];
            LoadCourseDetails(courseID);

            // Check if ProfileImgPath is available in the session
            string profileImgPath = Session["ProfileImgPath"]?.ToString();

            // If the ProfileImgPath is not set or is empty, use the default image
            if (string.IsNullOrEmpty(profileImgPath))
            {
                profileImgPath = "~/Assets/Images/customer/default_profile.jpg"; // Default image path
            }

            // Set the background-image dynamically
            commentProfileImg.Style["background-image"] = "url('" + ResolveUrl(profileImgPath) + "')";

            con.Close();

            prerequisitePopup.Visible = false;
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_CourseDetails.txt");

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
        // ==============================
        // LOAD COURSE DETAILS
        // ==============================

        private void LoadCourseDetails(string courseID)
        {

            // Read and Display Course General Info
            LoadCourseGeneralDetails(courseID);

            // Read and Display Course Tags
            bool isTagID = false;
            string courseTagName = LoadCourseTags(courseID, isTagID);
            ReadWrite_CourseTags(courseTagName);

            // Read and Display Course Feedback
            LoadCourseFeedback(courseID);

            // Read and Display Prerequisite Courses (if any)
            LoadCoursePrerequisite(courseID);

            // Read and Display Recommended/Related Course Info
            isTagID = true;
            string recordRecommendTags = LoadCourseTags(courseID, isTagID);
            string recordRecommendCategory = LoadCourseCategory(courseID);
            LoadCourseRecommend(recordRecommendTags, recordRecommendCategory);

            if (Session["UserID"] != null)
            {
                SetEnrolmentStatus(courseID, Session["UserID"].ToString());
            }
            else
            {
                lblCourseEnrolStatus.Text = "Not Started";
                btnEnrol.Visible = true;
                btnViewCourse.Visible = false;
            }

        }

        private void LoadCourseGeneralDetails(string courseID)
        {
            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            SqlCommand cmdGeneral = new SqlCommand(
                "SELECT " +
                "       Course.*, " +
                "       Customer.CustUsername, " +
                "       Customer.CustProfileImgPath, " +
                "       TutorInfo.JobTitle, " +
                "       TutorInfo.Company, " +
                "       COUNT(CourseEnrolment.CourseRegNo) AS EnrolmentCount " +
                "FROM " +
                "   Course " +
                "INNER JOIN CourseAssignment ON Course.CourseID = CourseAssignment.CourseID " +
                "LEFT JOIN Customer ON CourseAssignment.TutorID = Customer.CustID " +
                "INNER JOIN TutorInfo ON Customer.CustID = TutorInfo.TutorID " +
                "LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
                "WHERE Course.CourseID = @CourseID " +
                "GROUP BY " +
                "   Course.CourseID, Course.CourseName, Course.CourseShortDesc, " +
                "   Course.CourseDesc, Course.CourseObj, Course.CourseLevel, " +
                "   Course.CourseLang, Course.CourseFee, Course.CourseCreationDatetime, " +
                "   Course.CourseImgPath, Course.CourseIconPath, " +
                "   Customer.CustID, Customer.CustUsername, Customer.CustProfileImgPath, " +
                "   TutorInfo.JobTitle, TutorInfo.Company;", con);

            cmdGeneral.Parameters.AddWithValue("@CourseID", courseID);

            // Execute the query and process the results
            SqlDataReader readerGeneral = cmdGeneral.ExecuteReader();

            ReadWrite_CourseGeneral(readerGeneral);
            readerGeneral.Close();
        }

        private void LoadCourseFeedback(string courseID)
        {
            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            SqlCommand cmdFeedback = new SqlCommand(
                "SELECT " +
                "       CourseFeedback.*, " +
                "       Customer.CustUsername, " +
                "       Customer.CustProfileImgPath " +
                "FROM " +
                "   CourseFeedback " +
                "INNER JOIN Course ON CourseFeedback.CourseID = Course.CourseID " +
                "INNER JOIN Customer ON CourseFeedback.CustID = Customer.CustID " +
                "WHERE CourseFeedback.CourseID = @CourseID;", con);

            cmdFeedback.Parameters.AddWithValue("@CourseID", courseID);

            // Execute the query and process the results
            SqlDataReader readerFeedback = cmdFeedback.ExecuteReader();

            ReadWrite_CourseFeedback(readerFeedback);
            readerFeedback.Close();
        }

        private void LoadCoursePrerequisite(string mainCourseID)
        {
            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            SqlCommand cmdPrerequisite = new SqlCommand(
                "SELECT DISTINCT " +
                "   PrerequisiteCourse.CourseID, " +
                "   PrerequisiteCourse.CourseName, " +
                "   PrerequisiteCourse.CourseImgPath " +
                "FROM " +
                "   Course AS PrerequisiteCourse " +
                "INNER JOIN CoursePrerequisite ON PrerequisiteCourse.CourseID = CoursePrerequisite.PrerequisiteCourseID " +
                "INNER JOIN Course AS MainCourse ON CoursePrerequisite.CourseID = MainCourse.CourseID " +
                "WHERE MainCourse.CourseID = @MainCourseID;", con);

            cmdPrerequisite.Parameters.AddWithValue("@MainCourseID", mainCourseID);

            SqlDataReader readerPrerequisite = cmdPrerequisite.ExecuteReader();

            // If there is at least one prerequisite course
            if (readerPrerequisite.HasRows)
            {
                PrerequisiteSlider.Visible = true;
                ReadWrite_CoursePrerequisite(readerPrerequisite);
            }
            else
            {
                PrerequisiteSlider.Visible = false;
            }
            readerPrerequisite.Close();
        }

        private string LoadCourseTags(string courseID, bool isTagID)
        {
            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            SqlCommand cmdTags = new SqlCommand(
                "SELECT DISTINCT Tag.* " +
                "FROM " +
                "   Course " +
                "LEFT JOIN CourseTag ON Course.CourseID = CourseTag.CourseID " +
                "LEFT JOIN Tag ON CourseTag.TagID = Tag.TagID " +
                "WHERE Course.CourseID = @CourseID;", con);

            cmdTags.Parameters.AddWithValue("@CourseID", courseID);

            // Execute the query and process the results
            SqlDataReader readerTags = cmdTags.ExecuteReader();

            // List to hold tag IDs
            List<string> courseTag = new List<string>();

            while (readerTags.Read())
            {
                if (isTagID)
                {
                    // Assuming TagID is a string, so directly fetching TagID value
                    string tagId = readerTags["TagID"].ToString();
                    if (!string.IsNullOrEmpty(tagId))
                    {
                        courseTag.Add(tagId);
                    }
                }
                else
                {
                    // Assuming TagID is a string, so directly fetching TagID value
                    string tagName = readerTags["TagName"].ToString();
                    if (!string.IsNullOrEmpty(tagName))
                    {
                        courseTag.Add(tagName);
                    }
                }
            }

            // Close the reader
            readerTags.Close();

            // Join the tag IDs into a single comma-separated string with each tag ID wrapped in single quotes
            string tagsString = string.Join("', '", courseTag);

            // Return the string wrapped with single quotes on both ends
            return $"'{tagsString}'";
        }

        private string LoadCourseCategory(string courseID)
        {
            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            SqlCommand cmdCategory = new SqlCommand(
                "SELECT DISTINCT Category.* " +
                "FROM " +
                "   Course " +
                "LEFT JOIN CourseCategory ON Course.CourseID = CourseCategory.CourseID " +
                "LEFT JOIN Category ON CourseCategory.CategoryID = Category.CategoryID " +
                "WHERE Course.CourseID = @CourseID;", con);

            cmdCategory.Parameters.AddWithValue("@CourseID", courseID);

            // Execute the query and process the results
            SqlDataReader readerCategory = cmdCategory.ExecuteReader();
            // List to hold category IDs
            List<string> categoryIds = new List<string>();

            while (readerCategory.Read())
            {
                // Assuming CategoryID is a string, so directly fetching CategoryID value
                string tagId = readerCategory["CategoryID"].ToString();
                if (!string.IsNullOrEmpty(tagId))
                {
                    categoryIds.Add(tagId);
                }
            }

            // Close the reader
            readerCategory.Close();

            // Join the category IDs into a single comma-separated string with each category ID wrapped in single quotes
            string categoryString = string.Join("', '", categoryIds);

            // Return the string wrapped with single quotes on both ends
            return $"'{categoryString}'";
        }
        private void LoadCourseRecommend(string recordRecommendTags, string recordRecommendCategory)
        {
            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            // Build the SQL query using parameterized queries for TagIDs and CategoryIDs
            SqlCommand cmdRecommend = new SqlCommand(
                        @"
                    SELECT DISTINCT
                        RecommendedCourse.CourseID,
                        RecommendedCourse.CourseName,
                        RecommendedCourse.CourseImgPath
                    FROM Course AS RecommendedCourse
                    LEFT JOIN CourseTag ON RecommendedCourse.CourseID = CourseTag.CourseID
                    LEFT JOIN Tag ON CourseTag.TagID = Tag.TagID
                    LEFT JOIN CourseCategory ON RecommendedCourse.CourseID = CourseCategory.CourseID
                    LEFT JOIN Category ON CourseCategory.CategoryID = Category.CategoryID
                    WHERE CourseTag.TagID IN (@Tags)
                    OR CourseCategory.CategoryID IN (@Categories);
                ", con);

            // Add parameters for tags and categories
            cmdRecommend.Parameters.AddWithValue("@Tags", recordRecommendTags);
            cmdRecommend.Parameters.AddWithValue("@Categories", recordRecommendCategory);

            // Execute the query and process the results
            SqlDataReader readerRecommend = cmdRecommend.ExecuteReader();

            if (readerRecommend.HasRows)
            {
                ReadWrite_CourseRecommend(readerRecommend);
                readerRecommend.Close();
            }
            else
            {
                readerRecommend.Close();
                ReadWrite_CourseRecommend(LoadCourseRecommendFeatured());
            }
        }

        private SqlDataReader LoadCourseRecommendFeatured()
        {
            SqlCommand cmdRecommendFeatured = new SqlCommand(
                "SELECT " +
                    "Course.*, " +
                    "ISNULL(COUNT(CourseEnrolment.CourseRegNo), 0) AS EnrolmentCount " +
                "FROM " +
                    "(SELECT TOP 5 Course.CourseID " +
                    " FROM Course " +
                    " LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
                    " GROUP BY Course.CourseID " +
                    " HAVING COUNT(CourseEnrolment.CourseRegNo) > 0) AS TopCourses " +
                "INNER JOIN Course ON Course.CourseID = TopCourses.CourseID " +
                "LEFT JOIN CourseTag ON Course.CourseID = CourseTag.CourseID " +
                "INNER JOIN Tag ON Tag.TagID = CourseTag.TagID " +
                "LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
                "GROUP BY " +
                    "Course.CourseID, " +
                    "Course.CourseName, " +
                    "Course.CourseShortDesc, " +
                    "Course.CourseDesc, " +
                    "Course.CourseObj, " +
                    "Course.CourseLevel, " +
                    "Course.CourseLang, " +
                    "Course.CourseFee, " +
                    "Course.CourseCreationDatetime, " +
                    "Course.CourseImgPath, " +
                    "Course.CourseIconPath " +
                "ORDER BY Course.CourseID;", con);


            return cmdRecommendFeatured.ExecuteReader();
        }

        private void SetEnrolmentStatus(string courseID, string custID)
        {
            // Open the connection if it's not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            string enrolmentStatus;

            using (SqlCommand cmd = new SqlCommand(
                "SELECT EnrolmentStatus " +
                "FROM CourseEnrolment " +
                "WHERE CourseID = @CourseID " +
                "AND CustID = @CustID " +
                "AND EnrolmentStatus != 'Failed';", con))
            {
                // Add parameters with explicit types
                cmd.Parameters.Add("@CourseID", SqlDbType.VarChar, 50).Value = courseID;
                cmd.Parameters.Add("@CustID", SqlDbType.VarChar, 50).Value = custID;


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Check if there's a row
                    {
                        enrolmentStatus = reader["EnrolmentStatus"] != DBNull.Value
                            ? reader["EnrolmentStatus"].ToString()
                            : null;
                    }
                    else
                    {
                        enrolmentStatus = null; // Handle no results
                    }
                }
            }

            if (enrolmentStatus != null)
            {
                Console.WriteLine($"Enrolment Status: {enrolmentStatus}");
            }
            else
            {
                Console.WriteLine("No enrolment found for the given CourseID and CustID.");
            }

            if (!string.IsNullOrEmpty(enrolmentStatus))
            {
                lblCourseEnrolStatus.Text = enrolmentStatus;
                btnEnrol.Visible = false;
                btnViewCourse.Visible = true;
            }
            else
            {
                lblCourseEnrolStatus.Text = "Not Started";
                btnEnrol.Visible = true;
                btnViewCourse.Visible = false;
            }

        }


        // ==============================
        // DISPLAY COURSE DETAILS
        // ==============================

        // Process Course General Details
        protected void ReadWrite_CourseGeneral(SqlDataReader readerGeneral)
        {
            // Initialize variables for course details
            string courseName = string.Empty;
            string courseDesc = string.Empty;
            string courseObj = string.Empty;
            string courseLevel = string.Empty;
            string courseLang = string.Empty;
            double courseFee = 0;
            string courseImgPath = string.Empty;
            string courseEnrolCount = "None";
            string tutorHtml = string.Empty; // To store dynamically generated HTML for tutors

            // Process each row in the SqlDataReader
            while (readerGeneral.Read()) // Iterate through all rows
            {
                // Retrieve course details (only once since they're the same across rows)
                if (string.IsNullOrEmpty(courseName)) // Only execute once for course details
                {
                    courseName = readerGeneral["CourseName"].ToString();
                    courseDesc = readerGeneral["CourseDesc"].ToString();
                    courseObj = readerGeneral["CourseObj"].ToString();
                    courseLevel = CourseLevel_ToText(readerGeneral["CourseLevel"].ToString());
                    courseLang = CourseLang_ToText(readerGeneral["CourseLang"].ToString());
                    courseFee = Convert.ToDouble(readerGeneral["CourseFee"]);
                    courseImgPath = readerGeneral["CourseImgPath"].ToString();

                    // Display number of enrolments
                    if (readerGeneral["EnrolmentCount"] != DBNull.Value && readerGeneral["EnrolmentCount"].ToString() != "0")
                    {
                        courseEnrolCount = readerGeneral["EnrolmentCount"].ToString() + " Student(s)";
                    }
                }

                // Add each tutor's details to the HTML
                string tutorName = readerGeneral["CustUsername"].ToString();
                string tutorProfileImg = readerGeneral["CustProfileImgPath"].ToString();
                string tutorJobTitle = readerGeneral["JobTitle"].ToString();
                string tutorCompany = readerGeneral["Company"].ToString();

                tutorHtml += $@"
                                <div class='courseTutor_wrapper'>
                                    <div class='courseTutorImg_wrapper' style='background-image: url(""{Page.ResolveUrl(tutorProfileImg)}"")'></div>
                                    <div class='courseTutorNameRole_wrapper'>
                                        <h4 class='courseTutorName'>{tutorName}</h4>
                                        <p class='courseTutorRole'>{tutorJobTitle} at {tutorCompany}</p>
                                    </div>
                                </div>";
            }


            // Display course details
            Set_PageTitle(courseName);

            // Set Course Image
            if (!string.IsNullOrEmpty(courseImgPath))
            {
                // Using ClientScript to inject JavaScript
                string script = $"document.getElementById('courseImgWrapper').style.backgroundImage = \"url('{Page.ResolveUrl(courseImgPath)}')\";";
                ClientScript.RegisterStartupScript(this.GetType(), "changeBackgroundImage", script, true);

            }

            lblCourseName.Text = courseName;
            lblCourseDesc.Text = courseDesc;
            lblCourseObj.Text = courseObj;
            lblCourseLevel.Text = courseLevel;
            lblCourseLang.Text = courseLang;
            lblCourseEnrolCount.Text = courseEnrolCount;

            // Check on course fee: if fee = 0, mark it as a free course
            if (courseFee != 0)
            {
                lblCourseFeeLabel.Visible = true;
                lblCourseFee.Text = courseFee.ToString();
                lblCourseFee.CssClass = "courseFee"; // Retain default CSS class
            }
            else // Free course
            {
                lblCourseFeeLabel.Visible = false;
                lblCourseFee.Text = "FREE";
                lblCourseFee.CssClass = "courseFee freeCourse"; // Add the 'freeCourse' class
            }

            // Inject tutor details into the courseTutor_container div
            if (!string.IsNullOrEmpty(tutorHtml))
            {
                courseTutorContainer.InnerHtml = tutorHtml; // Dynamically set HTML content
            }
            else
            {
                courseTutorContainer.InnerHtml = "<p>No tutors assigned.</p>"; // Default message if no tutors
            }
        }

        // Process Course Tags Details
        protected void ReadWrite_CourseTags(string courseTags)
        {
            // Check if courseTags is not empty
            if (!string.IsNullOrEmpty(courseTags))
            {
                // Split the courseTags string into individual tag IDs
                string[] tagNames = courseTags.Split(new[] { "', '" }, StringSplitOptions.RemoveEmptyEntries);

                // Loop through each tag ID and dynamically add tags to the wrapper div
                foreach (string tagName in tagNames)
                {
                    // Sanitize the tag name (if necessary)
                    string sanitizedTag = tagName.Trim('\'');

                    // Create a new <div> element for the tag
                    HtmlGenericControl tagDiv = new HtmlGenericControl("div");
                    tagDiv.Attributes["class"] = "courseTag";
                    tagDiv.InnerText = sanitizedTag;

                    // Append the tag to the courseTag_wrapper
                    courseTag_wrapper.Controls.Add(tagDiv);
                }
            }
            else
            {
                // Handle the case where no tags are available
                HtmlGenericControl noTagDiv = new HtmlGenericControl("div");
                noTagDiv.Attributes["class"] = "courseTag";
                noTagDiv.InnerText = "No Tags Available";

                // Append the no-tags div to the wrapper
                courseTag_wrapper.Controls.Add(noTagDiv);
            }
        }

        // Process Course Feedbacks (Comment + Rating)
        protected void ReadWrite_CourseFeedback(SqlDataReader readerFeedback)
        {
            double totalRating = 0;
            int feedbackCount = 0;
            int commentCount = 0;
            double courseRating = 0;

            while (readerFeedback.Read())
            {
                // Extract feedback details
                string courseFeedback = readerFeedback["CourseFeedback"].ToString();
                if (readerFeedback["CourseFeedbackRating"] != DBNull.Value)
                {
                    courseRating = Convert.ToDouble(readerFeedback["CourseFeedbackRating"]);
                }
                DateTime feedbackDatetime = Convert.ToDateTime(readerFeedback["CourseFeedbackDatetime"]);
                string feedbackCustUsername = readerFeedback["CustUsername"].ToString();
                string feedbackCustProfileImgPath = readerFeedback["CustProfileImgPath"].ToString();

                // Calculate relative time
                string relativeTime = GetRelativeTime(feedbackDatetime);

                if (!string.IsNullOrEmpty(courseFeedback))
                {
                    // Generate HTML for this comment
                    string commentHtml = $@"
                    <div class='commentBox'>
                        <div class='commentProfileImg' style='background-image: url({Page.ResolveUrl(feedbackCustProfileImgPath)})'></div>
                        <div class='commentNameDateMsg'>
                            <div class='commentNameDate'>
                                <h3 class='commentName'>{feedbackCustUsername}</h3>
                                <p class='commentDate'>{relativeTime}</p>
                            </div>
                            <div class='commentMsg'>
                                <p>{courseFeedback}</p>
                            </div>
                        </div>
                    </div>";

                    // Append to the commentHistory_wrapper
                    commentHistory_wrapper.Controls.Add(new LiteralControl(commentHtml));
                    commentCount++;
                }

                if (readerFeedback["CourseFeedbackRating"] != DBNull.Value)
                {
                    // Add to rating calculations
                    totalRating += courseRating;
                    feedbackCount++;
                }
            }

            // Calculate the average rating and set it
            double averageRating = feedbackCount > 0 ? totalRating / feedbackCount : 0;
            lblCourseRating.Text = averageRating.ToString("F1");

            lblCommentCount.Text = "(" + commentCount.ToString() + ")";
        }

        // Process Prerequisite Courses (if any)
        protected void ReadWrite_CoursePrerequisite(SqlDataReader readerPrerequisite)
        {
            // Initialize the prerequisite slider with the heading and viewport structure
            string prerequisiteSliderContent = @"
                    <h3 class='prerequisitetitle'>Prerequisite(s)</h3>
                    <div class='slideBar_viewport'>
                        <div class='slideBar_wrapper'>
                ";

            // Initialize a list to store the prerequisite course IDs
            List<string> prerequisiteCourseIds = new List<string>();

            // Loop through the prerequisite courses and add them as course cards
            while (readerPrerequisite.Read())
            {
                string prerequisiteCourseID = readerPrerequisite["CourseID"].ToString().Trim();
                string prerequisiteCourseName = readerPrerequisite["CourseName"].ToString().Trim();
                string prerequisiteCourseImgPath = readerPrerequisite["CourseImgPath"].ToString().Trim();
                prerequisiteCourseIds.Add(prerequisiteCourseID);

                // Add a course card for each prerequisite course
                prerequisiteSliderContent += $@"
                            <div class='courseCard'>
                                <a href='courseDetails.aspx?courseId={prerequisiteCourseID}'>
                                    <div class='courseImg' style='background-image: url(""{Page.ResolveUrl(prerequisiteCourseImgPath)}"");'></div>
                                    <div class='courseDetails_wrapper'>
                                        <h3 class='courseSlideBarTitle'>{prerequisiteCourseName}</h3>
                                    </div>
                                </a>
                            </div>";
            }

            // Store the list of prerequisite course IDs in the ViewState
            ViewState["PrerequisiteCourseIds"] = prerequisiteCourseIds;

            // Close the slideBar_wrapper div
            prerequisiteSliderContent += @"
                        </div>
                    </div>
                ";

            // Add the slider buttons at the end
            prerequisiteSliderContent += @"
                    <div class='slideBarBtn_wrapper'>
                        <div id='preSlideLeftBtn' onclick='slide(this);'>
                            <i class='fa fa-caret-left'></i>
                        </div>
                        <div id='preSlideRightBtn' onclick='slide(this);'>
                            <i class='fa fa-caret-right'></i>
                        </div>
                    </div>
                ";

            // Set the content to the PrerequisiteSlider div
            PrerequisiteSlider.InnerHtml = prerequisiteSliderContent;
        }

        // Process Recommended Courses (if any)
        protected void ReadWrite_CourseRecommend(SqlDataReader readerRecommend)
        {
            // Initialize the recommend slider with the heading and viewport structure
            string recommendSliderContent = @"
                    <h3 class='recommendTitle'>You may also like</h3>
                    <div class='slideBar_viewport'>
                        <div class='slideBar_wrapper'>
                ";

            // Loop through the recommended courses and add them as course cards
            while (readerRecommend.Read())
            {
                string recommendCourseID = readerRecommend["CourseID"].ToString();
                string recommendCourseName = readerRecommend["CourseName"].ToString();
                string recommendCourseImgPath = readerRecommend["CourseImgPath"].ToString();

                // Add a course card for each recommended course
                recommendSliderContent += $@"
                            <div class='courseCard'>
                                <a href='courseDetails.aspx?courseId={recommendCourseID}'>
                                    <div class='courseImg' style='background-image: url(""{Page.ResolveUrl(recommendCourseImgPath)}"");'></div>
                                    <div class='courseDetails_wrapper'>
                                        <h3 class='courseSlideBarTitle'>{recommendCourseName}</h3>
                                    </div>
                                </a>
                            </div>";
            }

            readerRecommend.Close();

            // Close the slideBar_wrapper div
            recommendSliderContent += @"
                        </div>
                    </div>
                ";

            // Add the slider buttons at the end
            recommendSliderContent += @"
                    <div class='slideBarBtn_wrapper'>
                        <div id='recommendSlideLeftBtn' onclick='slide(this);'>
                            <i class='fa fa-caret-left'></i>
                        </div>
                        <div id='recommendSlideRightBtn' onclick='slide(this);'>
                            <i class='fa fa-caret-right'></i>
                        </div>
                    </div>
                ";

            // Set the content to the RecommendationSlider div
            RecommendationSlider.InnerHtml = recommendSliderContent;
        }

        // Display Datetime in text format (e.g. 2 days ago)
        private string GetRelativeTime(DateTime feedbackDateTime)
        {
            TimeSpan timeDifference = DateTime.Now - feedbackDateTime;

            if (timeDifference.TotalSeconds < 60)
                return $"{(int)timeDifference.TotalSeconds} second{(timeDifference.TotalSeconds < 2 ? "" : "s")} ago";
            if (timeDifference.TotalMinutes < 60)
                return $"{(int)timeDifference.TotalMinutes} minute{(timeDifference.TotalMinutes < 2 ? "" : "s")} ago";
            if (timeDifference.TotalHours < 24)
                return $"{(int)timeDifference.TotalHours} hour{(timeDifference.TotalHours < 2 ? "" : "s")} ago";
            if (timeDifference.TotalDays < 7)
                return $"{(int)timeDifference.TotalDays} day{(timeDifference.TotalDays < 2 ? "" : "s")} ago";
            if (timeDifference.TotalDays < 30)
                return $"{(int)(timeDifference.TotalDays / 7)} week{(timeDifference.TotalDays / 7 < 2 ? "" : "s")} ago";
            if (timeDifference.TotalDays < 365)
                return $"{(int)(timeDifference.TotalDays / 30)} month{(timeDifference.TotalDays / 30 < 2 ? "" : "s")} ago";
            if (timeDifference.TotalDays < 3650)
                return $"{(int)(timeDifference.TotalDays / 365)} year{(timeDifference.TotalDays / 365 < 2 ? "" : "s")} ago";

            // If more than 10 years ago, display the original date in a specific format
            return feedbackDateTime.ToString("yyyy-MM-dd");
        }



        // Convert Level Code To Full Text
        protected String CourseLevel_ToText(String levelCode)
        {
            switch (levelCode)
            {
                case "BEG":
                    return "Beginner";
                case "ITM":
                    return "Intermediate";
                case "ADV":
                    return "Advanced";
                default:
                    return "";
            }
        }

        // Convert Language Code To Full Text
        protected String CourseLang_ToText(String langCode)
        {
            switch (langCode)
            {
                case "EN":
                    return "English";
                case "CN":
                    return "Chinese";
                default:
                    return "";
            }
        }

        protected void Set_PageTitle(String courseName)
        {
            // Dynamically set the title
            ContentPlaceHolder titlePlaceholder = (ContentPlaceHolder)Master.FindControl("ContentPlaceHolder_Title");
            if (titlePlaceholder != null)
            {
                titlePlaceholder.Controls.Clear();
                titlePlaceholder.Controls.Add(new Literal { Text = courseName });
            }
        }

        protected void btnEnrol_Click(object sender, EventArgs e)
        {
            string courseID = Request.QueryString["courseId"]?.ToString();

            // Check if UserID exists in the session
            if (Session["UserID"] != null && !string.IsNullOrEmpty(courseID))
            {
                try
                {
                    // Open the connection if it's not already open
                    if (con.State != System.Data.ConnectionState.Open)
                    {
                        con.Open();
                    }

                    // Query to check if the current user is a tutor for this course
                    SqlCommand cmd = new SqlCommand(
                        "SELECT CourseAssignment.TutorID " +
                        "FROM CourseAssignment " +
                        "WHERE CourseAssignment.CourseID = @CourseID;", con);

                    cmd.Parameters.AddWithValue("@CourseID", courseID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Check if the current user is a tutor for the course
                            if (reader["TutorID"].ToString().Equals(Session["UserID"].ToString()))
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "BlockedEnrollment",
                                    "alert('Tutors cannot enroll in courses they are assigned to.');", true);

                                reader.Close(); // Ensure the reader is closed
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                    ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert",
                        $"alert('An error occurred: {HttpUtility.JavaScriptStringEncode(ex.Message)}');", true);
                }
                finally
                {
                    // Ensure the connection is closed
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                    }
                }

                // Checking for prerequisites
                if (ViewState["PrerequisiteCourseIds"] != null)
                {
                    // Retrieve the list of prerequisite course IDs from ViewState
                    List<string> prerequisiteCourseIds = (List<string>)ViewState["PrerequisiteCourseIds"];
                    SqlDataReader readerUnenrolledCourse = GetUnenrolledPrerequisites(prerequisiteCourseIds);

                    // Check if there are unenrolled courses
                    if (readerUnenrolledCourse.HasRows)
                    {
                        // Initialize an empty string to store the generated HTML
                        StringBuilder courseCardsHtml = new StringBuilder();

                        // Loop through the results and generate the course cards dynamically
                        while (readerUnenrolledCourse.Read())
                        {
                            // Extract course details from the reader
                            string courseId = readerUnenrolledCourse["CourseID"].ToString();
                            string courseName = readerUnenrolledCourse["CourseName"].ToString();
                            string courseImgPath = readerUnenrolledCourse["CourseImgPath"].ToString();

                            string courseImgSize = "background-size: cover;";

                            if (courseImgPath != null && courseImgPath.Equals("~/Assets/Images/course/courseImg/default_courseImg.png"))
                            {
                                courseImgSize = "background-size: contain;";
                            }

                            // Construct the HTML for the course card using string interpolation
                            courseCardsHtml.Append($@"
                                <div class='prerequisiteCourseCard' style='background-image: url(""{Page.ResolveUrl(courseImgPath)}""); {courseImgSize}'>
                                    <a href='courseDetails.aspx?courseId={courseId}'>
                                        <p class='prerequisiteCourseCard_title'>{courseName}</p>
                                    </a>
                                </div>");
                        }

                        prerequisitePopup.Visible = true;
                        // Assign the dynamically generated HTML to the course wrapper's InnerHtml
                        PrerequisitePopupCourseWrapper.InnerHtml = courseCardsHtml.ToString();

                        readerUnenrolledCourse.Close();
                        return;
                    }
                    readerUnenrolledCourse.Close();
                }

                else
                {
                    prerequisitePopup.Visible = false;
                    // Redirect to Payment.aspx, where the actual validation occurs
                }
                Response.Redirect($"User/Payment.aspx?courseId={HttpUtility.UrlEncode(courseID)}");

            }
            else
            {
                // Redirect to login page with return URL pointing back to this page
                string returnUrl = HttpUtility.UrlEncode(Request.Url.PathAndQuery);
                Response.Redirect($"AccountSelect.aspx?ReturnUrl={returnUrl}");
                return;
            }
        }

        protected SqlDataReader GetUnenrolledPrerequisites(List<string> prerequisiteCourseIds)
        {
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            // Build the SQL query dynamically with placeholders for parameters
            var placeholders = new List<string>();
            var parameters = new List<SqlParameter>();

            for (int i = 0; i < prerequisiteCourseIds.Count; i++)
            {
                string parameterName = $"@CourseId{i}"; // e.g., @CourseId0, @CourseId1, etc.
                placeholders.Add(parameterName);
                parameters.Add(new SqlParameter(parameterName, prerequisiteCourseIds[i]));
            }

            string query = $@"
        SELECT Course.CourseID, Course.CourseName, Course.CourseImgPath
        FROM Course
        LEFT JOIN CourseEnrolment 
            ON Course.CourseID = CourseEnrolment.CourseID 
            AND CourseEnrolment.CustID = @CustID
        LEFT JOIN CourseAssignment ca
            ON Course.CourseID = ca.CourseID
            AND ca.TutorID = @CustID
        WHERE Course.CourseID IN ({string.Join(",", placeholders)})
        AND (
            -- Case 1: Exclude the course if the user is a tutor for it
            NOT EXISTS (
                SELECT 1
                FROM CourseAssignment ca
                WHERE ca.TutorID = @CustID 
                AND ca.CourseID = Course.CourseID
            )
            AND
            (
                -- Case 2: The user is not enrolled in the prerequisite course
                CourseEnrolment.CourseID IS NULL
                OR
                -- Case 3: The user is enrolled but hasn't completed the prerequisite
                CourseEnrolment.EnrolmentStatus != 'Completed'
            )
        );
    ";

            SqlCommand cmd = new SqlCommand(query, con);

            // Add CustID parameter
            cmd.Parameters.AddWithValue("@CustID", Session["UserID"].ToString());

            // Add CourseID parameters
            foreach (var param in parameters)
            {
                cmd.Parameters.Add(param);
            }

            // Execute the query and return the result
            return cmd.ExecuteReader();
        }



        protected void btnViewCourse_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/User/Lesson.aspx?courseId={Request.QueryString["courseId"].ToString()}");
        }

        protected void btnSendComment_Click(object sender, EventArgs e)
        {
            SendComment();
            // Reload the page after posting the comment
            Response.Redirect(Request.Url.ToString(), true);

        }

        protected void txtComment_TextChanged(object sender, EventArgs e)
        {
            SendComment();
            // Reload the page after posting the comment
            Response.Redirect(Request.Url.ToString(), true);
        }

        protected void SendComment()
        {
            // Check if the user is anonymous (not logged in)
            if (Session["Role"] == null)
            {
                string returnUrl = HttpContext.Current.Request.Url.PathAndQuery; // Get the current URL and query string
                Response.Redirect($"AccountSelect.aspx?ReturnUrl={HttpUtility.UrlEncode(returnUrl)}");
            }
            else if (!Session["Role"].ToString().Equals("Admin") && (!string.IsNullOrEmpty(txtComment.Text) || !string.IsNullOrEmpty(hfSliderValue.Value)))
            {
                // Generate CourseFeedbackID and ensure it's unique
                DateTime courseFeedbackDatetime = DateTime.Now;
                string courseFeedbackID = GenerateUniqueCourseFeedbackID(courseFeedbackDatetime);

                // Retrieve CourseID and CustID from session or request (ensure these values are set in the session)
                string courseID = Request.QueryString["courseId"].ToString();
                string custID = Session["UserID"]?.ToString();
                string courseFeedbackRating = null; // Set to null by default

                double ratingValue = 0; // Default to 0 if no value is given

                // Access the value directly from the hidden field
                if (!string.IsNullOrEmpty(hfSliderValue.Value))
                {
                    ratingValue = Convert.ToDouble(hfSliderValue.Value); // Parse the hidden field value
                    courseFeedbackRating = (ratingValue / 10.0).ToString("F2"); // Assign the value to the rating
                }

                // Insert the feedback into the database using the existing connection (con)
                SqlCommand cmdComment = new SqlCommand(
                    "INSERT INTO CourseFeedback (CourseFeedbackID, CourseFeedback, CourseFeedbackRating, CourseFeedbackDatetime, CourseID, CustID) " +
                    "VALUES (@CourseFeedbackID, @CourseFeedback, @CourseFeedbackRating, @CourseFeedbackDatetime, @CourseID, @CustID)", con);

                cmdComment.Parameters.AddWithValue("@CourseFeedbackID", courseFeedbackID);
                cmdComment.Parameters.AddWithValue("@CourseFeedback", txtComment.Text);

                // If no rating was provided, pass DBNull.Value (null) to the database
                if (courseFeedbackRating == null)
                {
                    cmdComment.Parameters.AddWithValue("@CourseFeedbackRating", DBNull.Value);
                }
                else
                {
                    cmdComment.Parameters.AddWithValue("@CourseFeedbackRating", courseFeedbackRating); // Use the rating
                }

                cmdComment.Parameters.AddWithValue("@CourseFeedbackDatetime", courseFeedbackDatetime);
                cmdComment.Parameters.AddWithValue("@CourseID", courseID);
                cmdComment.Parameters.AddWithValue("@CustID", custID);

                try
                {
                    if (con.State != System.Data.ConnectionState.Open)
                    {
                        con.Open();
                    }
                    cmdComment.ExecuteNonQuery();
                    // Log successful comment insertion
                    string script = "alert('Comment posted successfully!');";
                    ClientScript.RegisterStartupScript(this.GetType(), "SuccessAlert", script, true);
                }
                catch (Exception ex)
                {
                    // Display the exception message in a JavaScript alert
                    string script = $"alert('An error occurred: {ex.Message}');";
                    ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", script, true);
                }
                finally
                {
                    txtComment.Text = "";
                    con.Close();
                    hfSliderValue.Value = ""; // Reset hidden field after processing (empty for no rating)
                }
            }
        }




        // Generate a unique CourseFeedbackID
        private string GenerateUniqueCourseFeedbackID(DateTime courseFeedbackDatetime)
        {
            string prefix = "CRSFDB-";
            string timestamp = courseFeedbackDatetime.ToString("yyyyMMddHHmmss"); // Get current timestamp
            string newFeedbackID = $"{prefix}{timestamp}-0001"; // Start with the default sequence number (0001)

            // Get all existing CourseFeedbackIDs from the database and check for uniqueness
            List<string> existingFeedbackIDs = GetAllExistingCourseFeedbackIDs();

            // Keep generating a new ID until it's unique
            while (existingFeedbackIDs.Contains(newFeedbackID))
            {
                // Increment the sequence number to avoid duplication
                string lastSequence = newFeedbackID.Split('-')[1]; // Get the sequence part (after the second '-')
                int nextSeq = int.Parse(lastSequence) + 1; // Increment the sequence number
                newFeedbackID = $"{prefix}{timestamp}-{nextSeq:D4}"; // Ensure the sequence is always 4 digits (e.g., 0001, 0002)
            }

            return newFeedbackID;
        }

        // Get all existing CourseFeedbackIDs from the database
        private List<string> GetAllExistingCourseFeedbackIDs()
        {
            List<string> feedbackIDs = new List<string>();

            // Query to get all CourseFeedbackIDs
            SqlCommand cmd = new SqlCommand("SELECT CourseFeedbackID FROM CourseFeedback", con);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string courseFeedbackID = reader["CourseFeedbackID"].ToString();
                    feedbackIDs.Add(courseFeedbackID); // Add each ID to the list
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

            return feedbackIDs;
        }
    }
}