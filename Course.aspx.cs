using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AlgoLab
{
    public partial class Course : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Bind the CheckBoxList
                cblLevels.DataBind();

                // Loop through each item and convert the text using CourseLevel_ToText
                foreach (ListItem item in cblLevels.Items)
                {
                    item.Text = CourseLevel_ToText(item.Value);
                }


                // Check if the search keyword is in the query string
                string searchCourse = Request.QueryString["searchCourse"];
                if (!string.IsNullOrEmpty(searchCourse))
                {
                    // Set the search text box with the value passed via query string
                    txtCourseSearch.Text = searchCourse;

                    // Load the custom content based on the search keyword
                    Load_CustomContent();
                }
                else
                {
                    // Load default content if no search keyword is found
                    Load_DefaultContent();
                }
            }
            else
            {
                // Reload custom content on postback
                Load_CustomContent();
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_Course.txt");

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

        // ==========================================
        // LOAD DEFAULT COURSE CARDS
        // ==========================================
        protected void Load_DefaultContent()
        {
            DefaultCourseWrapper.Visible = true;
            CustomCourseWrapper.Visible = false;

            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            Load_FeaturedCourse();
            Load_DefaultFreeCourse();
            Load_DefaultNewCourse();
            // Close the connection if done
            con.Close();
        }

        // Load Top 3 Featured Courses
        protected void Load_FeaturedCourse()
        {
            SqlCommand cmd = new SqlCommand(
                    "SELECT " +
                        "Course.*, Tag.*, " +
                        "ISNULL(COUNT(CourseEnrolment.CourseRegNo), 0) AS EnrolmentCount " +
                    "FROM " +
                        "(SELECT TOP 3 Course.CourseID " +
                        "FROM Course " +
                        "LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
                        "GROUP BY Course.CourseID " +
                        "HAVING COUNT(CourseEnrolment.CourseRegNo) > 0 " +
                        "ORDER BY COUNT(CourseEnrolment.CourseRegNo) DESC " +
                    ") AS TopCourses " +
                    "INNER JOIN Course ON Course.CourseID = TopCourses.CourseID " +
                    "LEFT JOIN CourseTag ON Course.CourseID = CourseTag.CourseID " +
                    "INNER JOIN Tag ON Tag.TagID = CourseTag.TagID " +
                    "INNER JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
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
                        "Course.CourseIconPath, " +
                        "Tag.TagID, " +
                        "Tag.TagName " +
                    "HAVING COUNT(CourseEnrolment.CourseRegNo) > 0;", con);


            // Execute the query and process the results
            SqlDataReader reader = cmd.ExecuteReader();
            ReadWrite_Course(reader, PlaceHolder_DefaultFeaturedCard);
            reader.Close();
        }

        // Load 3 Free Courses (Default)
        protected void Load_DefaultFreeCourse()
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT Course.*, Tag.*, COUNT(CourseEnrolment.CourseRegNo) AS EnrolmentCount " +
                "FROM ( " +
                    "SELECT CourseID " +
                    "FROM Course " +
                    "WHERE CourseFee = 0 " +
                ") AS TopFreeCourses " +
                "INNER JOIN Course ON TopFreeCourses.CourseID = Course.CourseID " +
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
                    "Course.CourseIconPath, " +
                    "Tag.TagID, " +
                    "Tag.TagName " +
                "ORDER BY Course.CourseID;", con);


            // Execute the query and process the results
            SqlDataReader reader = cmd.ExecuteReader();
            ReadWrite_Course(reader, PlaceHolder_DefaultFreeCard);
            reader.Close();
        }

        // Load 3 New Courses (Default)
        protected void Load_DefaultNewCourse()
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT Course.*, Tag.*, COUNT(CourseEnrolment.CourseRegNo) AS EnrolmentCount " +
                "FROM ( " +
                    "SELECT TOP 3 CourseID " +
                    "FROM Course " +
                    "ORDER BY CourseCreationDatetime DESC " +
                ") AS TopLatestCourses " +
                "INNER JOIN Course ON TopLatestCourses.CourseID = Course.CourseID " +
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
                    "Course.CourseIconPath, " +
                    "Tag.TagID, " +
                    "Tag.TagName " +
                "ORDER BY Course.CourseCreationDatetime DESC;", con);

            // Execute the query and process the results
            SqlDataReader reader = cmd.ExecuteReader();
            ReadWrite_Course(reader, PlaceHolder_DefaultNewCard);
            reader.Close();
        }


        // ==========================================
        // LOAD CUSTOM COURSE CARDS
        // ==========================================
        protected void Load_CustomContent()
        {
            DefaultCourseWrapper.Visible = false;
            CustomCourseWrapper.Visible = true;
            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            Load_CustomCourse();

            // Close the connection if done
            con.Close();

        }

        protected void Load_CustomCourse()
        {
            // Get selected values for filters
            string[] selectedCategory = cblCategory.Items
                .Cast<ListItem>()
                .Where(item => item.Selected)
                .Select(item => item.Value)
                .ToArray();
            string[] selectedLevels = cblLevels.Items
                .Cast<ListItem>()
                .Where(item => item.Selected)
                .Select(item => item.Value)
                .ToArray();
            List<string> selectedTags = cblTags.Items
                .Cast<ListItem>()
                .Where(item => item.Selected)
                .Select(item => item.Value)
                .ToList();
            string searchKeyword = txtCourseSearch.Text.Trim();

            // Get total course count
            int totalCourses = 0;
            using (SqlCommand totalCmd = new SqlCommand("SELECT COUNT(*) FROM Course", con))
            {
                totalCourses = (int)totalCmd.ExecuteScalar();
            }

            // Base query for filtering
            StringBuilder query = new StringBuilder(
                "SELECT Course.*, Tag.*, ISNULL(EnrolmentData.EnrolmentCount, 0) AS EnrolmentCount " +
                "FROM ( " +
                "   SELECT Course.CourseID, COUNT(CourseEnrolment.CourseRegNo) AS EnrolmentCount " +
                "   FROM Course " +
                "   LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
                "   GROUP BY Course.CourseID " +
                ") AS EnrolmentData " +
                "INNER JOIN Course ON EnrolmentData.CourseID = Course.CourseID " +
                "LEFT JOIN CourseCategory ON Course.CourseID = CourseCategory.CourseID " +  // Added CourseCategory join
                "LEFT JOIN Category ON CourseCategory.CategoryID = Category.CategoryID " + // Added Category join
                "LEFT JOIN CourseTag ON Course.CourseID = CourseTag.CourseID " +  // Added CourseTag join
                "INNER JOIN Tag ON Tag.TagID = CourseTag.TagID "  // Tag join
            );

            // List of SQL parameters
            List<SqlParameter> parameters = new List<SqlParameter>();

            // Add Category filter if selected
            if (selectedCategory.Length > 0)
            {
                query.Append(" WHERE CourseCategory.CategoryID IN (");
                for (int i = 0; i < selectedCategory.Length; i++)
                {
                    query.Append($"@Category{i},");
                    parameters.Add(new SqlParameter($"@Category{i}", selectedCategory[i]));
                }
                query.Length--; // Remove the trailing comma
                query.Append(")");
            }

            // Add Levels filter if selected
            if (selectedLevels.Length > 0)
            {
                query.Append(" AND Course.CourseLevel IN (");
                for (int i = 0; i < selectedLevels.Length; i++)
                {
                    query.Append($"@Level{i},");
                    parameters.Add(new SqlParameter($"@Level{i}", selectedLevels[i]));
                }
                query.Length--; // Remove the trailing comma
                query.Append(")");
            }

            // Add Tags filter if selected
            if (selectedTags.Count > 0)
            {
                query.Append(" AND Tag.TagID IN (");
                for (int i = 0; i < selectedTags.Count; i++)
                {
                    query.Append($"@Tag{i},");
                    parameters.Add(new SqlParameter($"@Tag{i}", selectedTags[i]));
                }
                query.Length--; // Remove the trailing comma
                query.Append(")");
            }

            // Add search keyword if present
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query.Append(" AND (Course.CourseName LIKE @SearchKeyword");
                query.Append(" OR Course.CourseShortDesc LIKE @SearchKeyword");
                query.Append(" OR Course.CourseDesc LIKE @SearchKeyword");
                query.Append(" OR Course.CourseObj LIKE @SearchKeyword)");
                parameters.Add(new SqlParameter("@SearchKeyword", $"%{searchKeyword}%"));
            }

            // Continue with the final query
            query.Append(" ORDER BY Course.CourseID;");

            // Execute the query
            SqlCommand cmd = new SqlCommand(query.ToString(), con);
            cmd.Parameters.AddRange(parameters.ToArray());

            SqlDataReader reader = cmd.ExecuteReader();

            int filteredCourses = 0;
            if (reader.HasRows)
            {
                NoCourseWrapper.Visible = false;
                lblCourseCount.Visible = true;

                // Count matching courses and render them
                filteredCourses = ReadWrite_Course(reader, PlaceHolder_CustomCourseCard);

                // Display the count of matching courses
                lblCourseCount.Text = $"Showing {filteredCourses} course(s) out of {totalCourses} available.";
            }
            else
            {
                NoCourseWrapper.Visible = true;
                lblCourseCount.Visible = false;
            }

            reader.Close();
        }


        // Button to show default menu
        protected void btnTopPicks_Click(object sender, EventArgs e)
        {
            txtCourseSearch.Text = "";
            DefaultCourseWrapper.Visible = true;
            CustomCourseWrapper.Visible = false;
            NoCourseWrapper.Visible = false;
            lblCourseCount.Visible = false;
            Load_DefaultContent();
        }

        // Button to show all courses
        protected void btnAllCourses_Click(object sender, EventArgs e)
        {
            txtCourseSearch.Text = "";
            DefaultCourseWrapper.Visible = false;
            CustomCourseWrapper.Visible = true;
            NoCourseWrapper.Visible = false;
            lblCourseCount.Visible = true;

            // Ensure the connection is open before executing commands
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open(); // Open the connection if it's not open
            }

            // Get total course count
            int totalCourses = 0;
            using (SqlCommand totalCmd = new SqlCommand("SELECT COUNT(*) FROM Course", con))
            {
                totalCourses = (int)totalCmd.ExecuteScalar();
            }

            SqlCommand cmd = new SqlCommand(
                "SELECT Course.*, Tag.*, COUNT(CourseEnrolment.CourseRegNo) AS EnrolmentCount " +
                "FROM Course " +
                "LEFT JOIN CourseTag ON Course.CourseID = CourseTag.CourseID " +
                "LEFT JOIN Tag ON CourseTag.TagID = Tag.TagID " +
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
                    "Course.CourseIconPath, " +
                    "Tag.TagID, " +
                    "Tag.TagName " +
                "ORDER BY Course.CourseID;", con);

            // Execute the query and process the results
            SqlDataReader reader = cmd.ExecuteReader();
            int filteredCourses = 0;
            if (reader.HasRows)
            {
                NoCourseWrapper.Visible = false;
                lblCourseCount.Visible = true;

                // Count matching courses and render them
                filteredCourses = ReadWrite_Course(reader, PlaceHolder_CustomCourseCard);

                // Display the count of matching courses
                lblCourseCount.Text = $"Showing {filteredCourses} course(s) out of {totalCourses} available.";
            }
            else
            {
                NoCourseWrapper.Visible = true;
                lblCourseCount.Visible = false;
            }

            reader.Close();

            con.Close();
        }


        // Convert Level Code To Full Text
        public static String CourseLevel_ToText(String levelCode)
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
        public static String CourseLang_ToText(String langCode)
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

        // Read Available Course
        private int ReadWrite_Course(SqlDataReader reader, PlaceHolder placeholder)
        {
            int courseCount = 0;
            if (reader.HasRows)
            {
                bool hasNextRow = reader.Read();
                while (hasNextRow) // TRUE: When there's another row/course to read (until when there are no course left)
                {
                    // Read data from the database
                    String courseID = reader["CourseID"].ToString();
                    String courseName = reader["CourseName"].ToString();
                    String courseShortDesc = reader["CourseShortDesc"].ToString();
                    String courseImgPath = reader["CourseImgPath"].ToString();
                    String courseLevel = CourseLevel_ToText(reader["CourseLevel"].ToString());
                    String courseLang = CourseLang_ToText(reader["CourseLang"].ToString());
                    double courseFee = Convert.ToDouble(reader["CourseFee"]);
                    String courseEnrolCount;
                    courseCount++;
                    if (reader["EnrolmentCount"] != DBNull.Value)
                    {
                        courseEnrolCount = reader["EnrolmentCount"].ToString();
                    }
                    else
                    {
                        courseEnrolCount = "None";
                    }

                    // Collect tags for the course [ArrayList is used for capture dynamic number of course tags to be read]
                    List<String> courseTags = new List<String>();

                    if (reader["TagID"] != DBNull.Value)
                    {
                        do
                        {
                            courseTags.Add(reader["TagName"].ToString());
                            hasNextRow = reader.Read();
                            // Check if the next row has the same CourseID and tag, so we can collect all tags for that course
                        } while (hasNextRow && reader["CourseID"].ToString() == courseID);
                    }
                    else
                    {
                        hasNextRow = reader.Read();
                    }

                    // Call the method to create the course card and add it to the Placeholder
                    placeholder.Controls.Add(
                       Create_CourseCard(courseID, courseName, courseShortDesc, courseLevel, courseLang, courseFee, courseImgPath, courseTags.ToArray(), courseEnrolCount)
                    );
                }

            }

            return courseCount;
        }

        // Create Course Card
        protected Literal Create_CourseCard(String courseID, String courseTitle, String courseShortDesc, String courseLevel, String courseLang, double courseFee, String courseImgPath, String[] courseTag, String courseEnrolCount)
        {
            StringBuilder courseCard = new StringBuilder();

            // Add the outer course card container
            courseCard.Append("<div class='course_card course_" + courseLevel.ToLower() + "'>");

            // Create the link to the course details page with a query string
            courseCard.Append("<a href='CourseDetails.aspx?courseId=" + courseID + "'>");

            // Course card wrapper
            courseCard.Append("<div class='course_card_wrapper'>");

            // Course image with dynamic background image URL
            courseCard.Append("<div class='course_card_img' style='background-image: url(\"" + Page.ResolveUrl(courseImgPath) + "\")'></div>");

            // Course content section
            courseCard.Append("<div class='course_card_content'>");

            if (courseTag != null)
            {
                // Course tags section
                courseCard.Append("<div class='course_card_tag_wrapper'>");
                foreach (string tag in courseTag)
                {
                    courseCard.Append("<div class='course_tag'>" + tag + "</div>");
                }
                courseCard.Append("</div>");
            }

            // Course title
            courseCard.Append("<h3 class='course_card_title'>" + courseTitle + "</h3>");

            // Course short description
            courseCard.Append("<p class='course_card_shortDesc'>" + courseShortDesc + "</p>");

            // Course language and price section
            courseCard.Append("<div class='course_card_langPrice_wrapper'>");
            courseCard.Append("<div class='course_card_lang'>");
            courseCard.Append("<i class='fa fa-language'></i>");
            courseCard.Append("<p>" + courseLang + "</p>");
            courseCard.Append("</div>");

            // Course price
            courseCard.Append("<div class='course_card_price'>");
            courseCard.Append("<i class='fa fa-money'></i>");
            String courseFeeStr = "$" + courseFee.ToString("F2");
            if (courseFee == 0)
            {
                courseFeeStr = "Free";
            }
            courseCard.Append("<p>" + courseFeeStr + "</p>");
            courseCard.Append("</div>");

            // Course Enrolment
            courseCard.Append("<div class='course_card_enrol'>");
            courseCard.Append("<i class='fa fa-users'></i>");
            courseCard.Append("<p>" + courseEnrolCount + "</p>");
            courseCard.Append("</div>");

            // Close the langPrice wrapper and content wrapper
            courseCard.Append("</div>");
            courseCard.Append("</div>");

            // Close the wrapper and link
            courseCard.Append("</div>");
            courseCard.Append("</a>");
            courseCard.Append("</div>");

            // Create a Literal control dynamically
            Literal courseCardLiteral = new Literal();
            courseCardLiteral.Text = courseCard.ToString();

            return courseCardLiteral;
        }
    }
}