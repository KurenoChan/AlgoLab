using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlgoLab
{
    public partial class CourseManagement : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Bind the CheckBoxList with tags from the database, only during initial page load.
                BindTagCheckboxList();
                BindCategoryDropdown();
            }



            if (Session["UserID"] == null)
            {
                string returnUrl = HttpContext.Current.Request.Url.PathAndQuery; // Get the full path and query string of the current page
                Response.Redirect($"../AccountSelect.aspx?ReturnUrl={HttpUtility.UrlEncode(returnUrl)}");
            }

            string userID = Session["UserID"]?.ToString();
            string role = Session["Role"]?.ToString();

            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            Load_MyCourse(userID, role);


        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_CourseManagement.txt");

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

        protected void Load_MyCourse(string userID, string userRole)
        {


            if (userRole == "Tutor")
            {
                TutorMyCourseCard.Visible = true;
                StudentMyCourseCard.Visible = false;

                SqlCommand cmdTutor;
                create.Visible = true;
                cmdTutor = new SqlCommand(

                    "SELECT " +
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
                "Tag.TagName, " +
                "COUNT(CourseEnrolment.CourseRegNo) AS EnrolmentCount " +
            "FROM " +
                "CourseAssignment ca " +
            "INNER JOIN Course ON ca.CourseID = Course.CourseID " +
            "LEFT JOIN CourseTag ON Course.CourseID = CourseTag.CourseID " +
            "INNER JOIN Tag ON Tag.TagID = CourseTag.TagID " +
            "LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
            "WHERE ca.TutorID = @UserID " +
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

                // Add the UserID parameter to the SQL command
                cmdTutor.Parameters.AddWithValue("@UserID", userID);
                // Execute the query and process the results
                SqlDataReader reader = cmdTutor.ExecuteReader();

                // Check if any courses were returned by the query
                if (reader.HasRows)
                {
                    ReadWrite_Course(reader, PlaceHolder_MyCourseCard_Tutor);
                }
                else
                {
                    // Handle invalid user role or show a message
                    no_content.Visible = true;
                }
                reader.Close();
                return;
            }
            else if (userRole == "Student")
            {
                TutorMyCourseCard.Visible = false;
                StudentMyCourseCard.Visible = true;

                SqlCommand cmdActive;

                cmdActive = new SqlCommand(
                    "SELECT " +
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
                    "Tag.TagName, " +
                    "COUNT(CourseEnrolment.CourseRegNo) AS EnrolmentCount " +
                    "FROM " +
                    "CourseEnrolment ce " +
                    "INNER JOIN Course ON ce.CourseID = Course.CourseID " +
                    "LEFT JOIN CourseTag ON Course.CourseID = CourseTag.CourseID " +
                    "INNER JOIN Tag ON Tag.TagID = CourseTag.TagID " +
                    "LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
                    "WHERE ce.CustID = @UserID " +
                    "AND ce.EnrolmentStatus = 'Active' " + // Only active courses
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
                    "HAVING COUNT(CourseEnrolment.CourseRegNo) > 0 " + // Ensure enrollments exist
                    "ORDER BY Course.CourseID;", con);

                // Add the UserID parameter to the SQL command
                cmdActive.Parameters.AddWithValue("@UserID", userID);
                // Execute the query and process the results
                SqlDataReader readerActive = cmdActive.ExecuteReader();

                // Check if any courses were returned by the query
                if (readerActive.HasRows)
                {
                    ReadWrite_Course(readerActive, PlaceHolder_MyCourseCard_Active);
                }
                else
                {
                    // Handle invalid user role or show a message
                    no_content.Visible = true;
                }

                readerActive.Close();

                SqlCommand cmdCompleted;

                cmdCompleted = new SqlCommand(
                    "SELECT " +
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
                    "Tag.TagName, " +
                    "COUNT(CourseEnrolment.CourseRegNo) AS EnrolmentCount " +
                    "FROM " +
                    "CourseEnrolment ce " +
                    "INNER JOIN Course ON ce.CourseID = Course.CourseID " +
                    "LEFT JOIN CourseTag ON Course.CourseID = CourseTag.CourseID " +
                    "INNER JOIN Tag ON Tag.TagID = CourseTag.TagID " +
                    "LEFT JOIN CourseEnrolment ON Course.CourseID = CourseEnrolment.CourseID " +
                    "WHERE ce.CustID = @UserID " +
                    "AND ce.EnrolmentStatus = 'Completed' " + // Only active courses
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
                    "HAVING COUNT(CourseEnrolment.CourseRegNo) > 0 " + // Ensure enrollments exist
                    "ORDER BY Course.CourseID;", con);

                // Add the UserID parameter to the SQL command
                cmdCompleted.Parameters.AddWithValue("@UserID", userID);
                // Execute the query and process the results
                SqlDataReader readerCompleted = cmdCompleted.ExecuteReader();

                // Check if any courses were returned by the query
                if (readerCompleted.HasRows)
                {
                    ReadWrite_Course(readerCompleted, PlaceHolder_MyCourseCard_Completed);
                }
                else
                {
                    // Handle invalid user role or show a message
                    no_content.Visible = true;
                }

                readerCompleted.Close();
                return;
            }
            else
            {
                // Handle invalid user role or show a message
                no_content.Visible = true;
                return;
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


        // Create Course Card
        protected Literal Create_CourseCard(String courseID, String courseTitle, String courseShortDesc, String courseLevel, String courseLang, double courseFee, String courseImgPath, String[] courseTag, String courseEnrolCount)
        {
            StringBuilder courseCard = new StringBuilder();

            // Add the outer course card container
            courseCard.Append("<div class='course_card course_" + courseLevel.ToLower() + "'>");

            // Create the link to the course details page with a query string
            courseCard.Append("<a href='../User/Lesson.aspx?courseId=" + courseID + "'>");

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


        protected void CreateCourse(object sender, EventArgs e)
        {
            // Reset error message visibility
            CourseFeeError.Visible = false;

            // Flag to track if the form is valid
            bool isValid = true;

            decimal courseFee = 0;  // Declare the variable to store the parsed course fee

            // Validate Course Fee
            if (string.IsNullOrWhiteSpace(CourseFee.Text))
            {
                CourseFeeError.Text = "Course Fee is required.";
                CourseFeeError.Visible = true;
                isValid = false;
            }
            else if (!decimal.TryParse(CourseFee.Text, out courseFee) || courseFee <= 0)
            {
                CourseFeeError.Text = "Course Fee must be a valid positive number.";
                CourseFeeError.Visible = true;
                isValid = false;
            }

            // Validate other required fields
            if (string.IsNullOrWhiteSpace(CourseName.Text))
            {
                // Handle CourseName error, if necessary
                isValid = false;
            }

            // Only proceed to create the course if the form is valid
            if (isValid)
            {
                string courseName = CourseName.Text;
                string courseShortDesc = CourseShortDesc.Text;
                string courseDesc = CourseDesc.Text;
                string courseObj = CourseObj.Text;
                string courseLevel = CourseLevel.SelectedValue;
                string courseLang = CourseLang.Text;


                // Generate CourseID
                string courseID = GenerateCourseID();



                string folderDirectory = Server.MapPath("~/Assets/Images/course/courseImg/");
                string courseFolderPath = Path.Combine(folderDirectory, courseID);

                if (!Directory.Exists(courseFolderPath))
                {
                    Directory.CreateDirectory(courseFolderPath);
                }


                string courseImgPath = "~/Assets/Images/course/courseImg/default_courseImg.png";
                string courseIconPath = "~/Assets/Images/course/courseImg/default_courseImg.png";


                // Save Course Image with CourseID as the file name
                if (CourseImgUpload.HasFile)
                {

                    string fileName = "Image" + Path.GetExtension(CourseImgUpload.PostedFile.FileName);
                    string savePath = Path.Combine(courseFolderPath, fileName);
                    CourseImgUpload.SaveAs(savePath);

                    courseImgPath = $"~/Assets/Images/course/courseImg/{courseID}/{fileName}";
                }

                // Save Course Icon with CourseID as the file name
                if (CourseIcon.HasFile)
                {
                    string fileName = "Icon" + Path.GetExtension(CourseIcon.PostedFile.FileName);
                    string savePath = Path.Combine(courseFolderPath, fileName);
                    CourseIcon.SaveAs(savePath);

                    courseIconPath = $"~/Assets/Images/course/courseImg/{courseID}/{fileName}";
                }


                // Database insertion logic
                string query = "INSERT INTO [dbo].[Course]([CourseID], [CourseName], [CourseShortDesc], [CourseDesc], [CourseObj], [CourseLevel], [CourseLang], [CourseFee], [CourseImgPath], [CourseIconPath]) " +
                               "VALUES (@courseID, @courseName, @courseShortDesc, @courseDesc, @courseObj, @courseLevel, @courseLang, @courseFee, @courseImgPath, @courseIconPath)";



                con.Close();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    cmd.Parameters.AddWithValue("@courseID", courseID);
                    cmd.Parameters.AddWithValue("@courseName", courseName);
                    cmd.Parameters.AddWithValue("@courseShortDesc", courseShortDesc);
                    cmd.Parameters.AddWithValue("@courseDesc", courseDesc);
                    cmd.Parameters.AddWithValue("@courseObj", courseObj);
                    cmd.Parameters.AddWithValue("@courseLevel", courseLevel);
                    cmd.Parameters.AddWithValue("@courseLang", courseLang);
                    cmd.Parameters.AddWithValue("@courseFee", courseFee);  // Use the parsed value of courseFee here
                    cmd.Parameters.AddWithValue("@courseImgPath", courseImgPath);
                    cmd.Parameters.AddWithValue("@courseIconPath", courseIconPath);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    con.Close();


                }


                updateAssignment(courseID);

                updateCategory(courseID);
                updateTag(courseID);


                string script = @"
    alert('Course created successfully!');
    window.location.href = window.location.href;
";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessMessage", script, true);


            }
            else
            {
                // If form is not valid, ensure form stays visible
                CreateCourseForm.Visible = true;
            }
        }



        private string GenerateCourseID()
        {
            Random random = new Random();

            // Generate the first two uppercase letters
            string part1 = GenerateRandomLetters(2, random);

            // Generate the four digits
            string part2 = random.Next(1000, 9999).ToString();

            // Generate the last two uppercase letters
            string part3 = "EN";

            // Combine all parts into the desired CourseID format
            return $"{part1}-{part2}-{part3}";
        }

        private string GenerateRandomLetters(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] letters = new char[length];

            for (int i = 0; i < length; i++)
            {
                letters[i] = chars[random.Next(chars.Length)];
            }

            return new string(letters);
        }


        public string GenerateAssignmentNo()
        {
            // Use the current date and time for the middle part
            string middlePart = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Generate the last 4 random digits
            Random random = new Random();
            string lastPart = random.Next(0, 9999).ToString("D4");

            // Combine the parts into the required format
            return $"CASG-{middlePart}-{lastPart}";
        }



        public void updateAssignment(string courseID)
        {
            // Generate AssignmentNo
            string assignmentNo = GenerateAssignmentNo();

            string userID = Session["UserID"]?.ToString();
            string role = Session["Role"]?.ToString();


            // Insert into CourseAssignment table
            string assignmentQuery = "INSERT INTO [dbo].[CourseAssignment] (AssignmentNo, TutorID, CourseID) " +
                                     "VALUES (@AssignmentNo, @TutorID, @CourseID)";

            using (SqlCommand cmd = new SqlCommand(assignmentQuery, con))
            {
                cmd.Parameters.AddWithValue("@AssignmentNo", assignmentNo);
                cmd.Parameters.AddWithValue("@TutorID", userID);
                cmd.Parameters.AddWithValue("@CourseID", courseID);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }


        public void updateCategory(string courseID)
        {
            // Insert into CourseCategory table
            string categoryQuery = "INSERT INTO [dbo].[CourseCategory] ([CourseID], [CategoryID]) " +
                                     "VALUES (@CourseID, @CategoryID)";

            using (SqlCommand cmd = new SqlCommand(categoryQuery, con))
            {

                string categoryID = CourseCategory.SelectedValue;
                cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                cmd.Parameters.AddWithValue("@CourseID", courseID);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }



        public void updateTag(string courseID)
        {
            // Loop through all selected items in the CheckBoxList
            foreach (ListItem items in CourseTags.Items)
            {
                // Check if the tag is selected
                if (items.Selected)
                {
                    // Query to insert into the CourseTag table
                    string TagQuery = "INSERT INTO [dbo].[CourseTag] ([CourseID], [TagID]) " +
                                      "VALUES (@CourseID, @TagID)";

                    // Create and execute the SQL command for each selected tag
                    using (SqlCommand cmd = new SqlCommand(TagQuery, con))
                    {
                        // Adding parameters to the SQL command
                        cmd.Parameters.AddWithValue("@CourseID", courseID); // CourseID is provided as a parameter
                        cmd.Parameters.AddWithValue("@TagID", items.Value); // TagID is the value of the selected checkbox item

                        con.Open(); // Open the connection
                        cmd.ExecuteNonQuery(); // Execute the command
                        con.Close(); // Close the connection after execution
                    }
                }
            }
        }




        private void BindCategoryDropdown()
        {
            SqlCommand cmd = new SqlCommand(
        "SELECT CategoryID, CategoryName FROM Category", con);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();





            CourseCategory.DataSource = reader;
            CourseCategory.DataTextField = "CategoryName";
            CourseCategory.DataValueField = "CategoryID";
            CourseCategory.DataBind();
            // If a value was selected on postback, make sure it's selected in the dropdown
            string selectedCategoryID = Request.Form[CourseCategory.UniqueID];
            if (!string.IsNullOrEmpty(selectedCategoryID))
            {
                CourseCategory.SelectedValue = selectedCategoryID;
            }


            con.Close();
        }


        private void BindTagCheckboxList()
        {
            SqlCommand cmd = new SqlCommand("SELECT TagID, TagName FROM Tag", con);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            CourseTags.DataSource = reader;
            CourseTags.DataTextField = "TagName";
            CourseTags.DataValueField = "TagID";
            CourseTags.DataBind();

            con.Close();
        }


        protected void CourseCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = CourseCategory.SelectedValue.ToString();
        }

    }
}
