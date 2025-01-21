using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace AlgoLab
{
    public partial class Lesson : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Session["Role"]?.ToString();  // Assuming role is stored in session

            // Dynamically set the title
            ContentPlaceHolder titlePlaceholder = (ContentPlaceHolder)Master.FindControl("ContentPlaceHolder_Title");
            if (titlePlaceholder != null)
            {
                titlePlaceholder.Controls.Clear();
                if (!string.IsNullOrEmpty(role) && role.Equals("Student"))
                {
                    titlePlaceholder.Controls.Add(new Literal { Text = "Lesson" });
                }
                else if (!string.IsNullOrEmpty(role) && role.Equals("Tutor"))
                {
                    titlePlaceholder.Controls.Add(new Literal { Text = "Lesson Management" });
                }
            }


            // Query to check if any courses are available
            string courseID = Request.QueryString["courseId"]; // Assuming courseId is passed in the query string
            bool hasCourses = CheckForAvailableCourses(courseID);  // Check if courses are available for this course ID

            // If there are no courses, display "No Content" message
            no_content.Visible = !hasCourses;

            // If there are courses, check the user's role
            if (hasCourses)
            {

                if (role == "student")  // Hide the button for students
                {
                    CompleteCourseBtn.Visible = true;
                }

            }
            else
            {
                student_Layout.Visible = false; // Hide student layout if no courses available
            }



            // Register the jQuery mapping
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery-3.6.0.min.js", // Replace with your actual jQuery file path
                DebugPath = "~/Scripts/jquery-3.6.0.js", // Path for the debug version
                CdnPath = "https://code.jquery.com/jquery-3.6.0.min.js", // Optional: Use a CDN version
                CdnDebugPath = "https://code.jquery.com/jquery-3.6.0.js",
                CdnSupportsSecureConnection = true
            });



            if (!IsPostBack)
            {
                // Retrieve the courseId from the query string
                string courseId = Request.QueryString["courseId"];
                Load_Lesson(role, courseId);

            }


        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_Lesson.txt");

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

        protected void Load_Lesson(string role, string courseId)
        {


            if (role == "Tutor")
            {

                TutorLayout.Visible = true;




                string query = @"
SELECT 
    l.LessonID,
    l.LessonTitle,
    l.LessonDesc,
    
    m.MaterialName,
    m.MaterialPath
FROM 
    CourseLesson l
LEFT JOIN 
    CourseMaterial m ON l.LessonID = m.LessonID
WHERE 
    l.CourseID = @CourseID
ORDER BY 
    l.LessonID, m.MaterialName";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CourseID", courseId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                var lessonMaterials = new List<dynamic>();

                while (reader.Read())
                {
                    lessonMaterials.Add(new
                    {
                        LessonID = reader["LessonID"],
                        LessonTitle = reader["LessonTitle"],
                        LessonDesc = reader["LessonDesc"],

                        MaterialName = reader["MaterialName"] != DBNull.Value ? reader["MaterialName"].ToString() : null,
                        MaterialPath = reader["MaterialPath"] != DBNull.Value && !string.IsNullOrEmpty(reader["MaterialPath"].ToString())
                                        ? Page.ResolveUrl(reader["MaterialPath"].ToString())
                                        : null
                    });
                }

                con.Close();

                var lessons = lessonMaterials
                    .GroupBy(l => new { l.LessonID, l.LessonTitle, l.LessonDesc, })
                    .Select(g => new
                    {
                        LessonID = g.Key.LessonID,
                        LessonTitle = g.Key.LessonTitle,
                        LessonDesc = g.Key.LessonDesc,

                        Materials = g
                            .Where(m => m.MaterialName != null)
                            .Select(m => new
                            {
                                MaterialName = m.MaterialName,
                                MaterialPath = m.MaterialPath
                            }).ToList()
                    }).ToList();

                RepeaterLessons.DataSource = lessons;
                RepeaterLessons.DataBind();
            }


            else if (role == "Student")
            {
                student_Layout.Visible = true;
                string query = @"
SELECT 
    l.LessonID,
    l.LessonTitle,
    l.LessonDesc,
    
    m.MaterialName,
    m.MaterialPath
FROM 
    CourseLesson l
LEFT JOIN 
    CourseMaterial m ON l.LessonID = m.LessonID
WHERE 
    l.CourseID = @CourseID
ORDER BY 
    l.LessonID, m.MaterialName";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CourseID", courseId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                var lessonMaterials = new List<dynamic>();

                while (reader.Read())
                {
                    lessonMaterials.Add(new
                    {
                        LessonID = reader["LessonID"],
                        LessonTitle = reader["LessonTitle"],
                        LessonDesc = reader["LessonDesc"],

                        MaterialName = reader["MaterialName"] != DBNull.Value ? reader["MaterialName"].ToString() : null,
                        MaterialPath = Page.ResolveUrl(reader["MaterialPath"] != DBNull.Value ? reader["MaterialPath"].ToString() : null)
                    });
                }

                con.Close();

                var lessons = lessonMaterials
                    .GroupBy(l => new { l.LessonID, l.LessonTitle, l.LessonDesc, })
                    .Select(g => new
                    {
                        LessonID = g.Key.LessonID,
                        LessonTitle = g.Key.LessonTitle,
                        LessonDesc = g.Key.LessonDesc,

                        Materials = g
                            .Where(m => m.MaterialName != null)
                            .Select(m => new
                            {
                                MaterialName = m.MaterialName,
                                MaterialPath = m.MaterialPath
                            }).ToList()
                    }).ToList();

                RepeaterLessons.DataSource = lessons;
                RepeaterLessons.DataBind();
            }
        }


        public void UploadMaterial()
        {

            string courseId = Request.QueryString["courseId"];
            string lessonId = Request.Form["lessonId"];
            string materialName = MaterialName.Text;  // Material name from the TextBox
            string filePath = string.Empty;
            string filePaths = string.Empty;

            if (!MaterialPath.HasFile)
            {
                // No file selected
                throw new InvalidOperationException("Please select a file to upload.");
            }

            // Define the folder where files will be saved

            string uploadFolder = Server.MapPath("~/Assets/Images/course/courseMaterial/" + courseId + "/" + lessonId);
            string savematerial = $"~/Assets/Images/course/courseMaterial/{courseId}/{lessonId}";



            // Create the folder if it doesn't exist
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Get the file extension and ensure it's a valid file type
            string fileExtension = Path.GetExtension(MaterialPath.FileName).ToLower();
            string[] allowedExtensions = { ".pdf", ".docx", ".pptx", ".txt" };

            if (!allowedExtensions.Contains(fileExtension))
            {
                // Invalid file type
                throw new InvalidOperationException("Invalid file type. Please upload a PDF, DOCX, PPTX, or TXT file.");
            }

            // Define the file name and path
            string fileName = $"{Guid.NewGuid()}{fileExtension}"; // Unique name using GUID
            filePath = Path.Combine(uploadFolder, fileName);
            filePaths = Path.Combine(savematerial, fileName);

            // Save the file to the server
            MaterialPath.SaveAs(filePath);

            // Save the material information to the database
            SaveMaterialToDatabase(lessonId, materialName, filePaths);
        }



        private void SaveMaterialToDatabase(string lessonId, string fileName, string filePath)
        {

            string materialQuery = "INSERT INTO [dbo].[CourseMaterial] ([MaterialID], [MaterialName], [MaterialPath], [LessonID]) " +
                                     "VALUES (@MaterialID, @MaterialName, @MaterialPath, @LessonID)";

            using (SqlCommand cmd = new SqlCommand(materialQuery, con))
            {
                cmd.Parameters.AddWithValue("@MaterialID", GenerateMaterialID());
                cmd.Parameters.AddWithValue("@MaterialName", fileName);
                cmd.Parameters.AddWithValue("@MaterialPath", filePath);
                cmd.Parameters.AddWithValue("@LessonID", lessonId);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }


        }

        public string GenerateMaterialID()
        {
            // Use the current date and time for the middle part
            string middlePart = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Generate the last 4 random digits
            Random random = new Random();
            string lastPart = random.Next(0, 9999).ToString("D4");

            // Combine the parts into the required format
            return $"MAT-{middlePart}-{lastPart}";
        }



        protected void upload_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Call the UploadMaterial method
                UploadMaterial();

                // Reload lessons to reflect new materials (if required)
                string role = Session["Role"]?.ToString();
                string courseId = Request.QueryString["courseId"];
                Load_Lesson(role, courseId);

                // Display a success message
                ClientScript.RegisterStartupScript(this.GetType(), "UploadSuccess", "alert('Material uploaded successfully.');", true);
            }
            catch (InvalidOperationException ex)
            {
                // Handle validation errors (e.g., no file selected, invalid file type)
                ClientScript.RegisterStartupScript(this.GetType(), "ValidationError", $"alert('{ex.Message}');", true);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                ClientScript.RegisterStartupScript(this.GetType(), "UploadError", $"alert('An error occurred: {ex.Message}');", true);
            }
        }



        protected void CompleteCourse_Click(object sender, EventArgs e)
        {
            // Get the CourseID from the CommandArgument of the button
            string courseID = Request.QueryString["courseId"];


            // Get the CustID of the logged-in user (adjust based on your user management)
            string custID = Session["UserID"].ToString(); // Example: Assuming you store UserID in session



            string query = "UPDATE CourseEnrolment " +
                           "SET EnrolmentStatus = 'Completed' " +
                           "WHERE CourseID = @CourseID AND CustID = @CustID";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@CourseID", courseID);
            cmd.Parameters.AddWithValue("@CustID", custID);

            try
            {
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    // Optionally, you can notify the user that the status has been updated.
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Course status updated to Completed');", true);
                    // Redirect to course.aspx after successful update
                    Response.Redirect("~/Course.aspx");  // Redirects to the course page
                }
                else
                {
                    // Handle case if no rows were updated (e.g., invalid CourseID or CustID).
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error updating course status.');", true);
                }
            }
            catch (Exception ex)
            {
                // Handle error (e.g., log it)
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
            }







        }
        protected void RepeaterLessons_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Check if the item is a data item (not a header/footer)
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Find the Panel (uploadmaterial) inside the repeater item
                Panel uploadButton = (Panel)e.Item.FindControl("uploadmaterial");

                // Get user role from session
                string userRole = Session["Role"]?.ToString(); // Assuming "Role" is the correct session variable name

                // If user is a tutor, make the panel visible
                if (userRole == "Tutor" && uploadButton != null)
                {
                    uploadButton.Style["display"] = "block"; // Make the upload button visible
                }
                else
                {
                    uploadButton.Style["display"] = "none"; // Hide the upload button for others
                }
            }
        }



        protected void createLesson_Btn_Click(object sender, EventArgs e)
        {

            Random random = new Random();
            // Generate the LessonID in the format LID-00000000000000-0000
            string datePart = DateTime.Now.ToString("yyyyMMddHHmmss"); // Get date and time part (14 digits: yyyyMMddHHmmss)
            string idPart1 = datePart.PadLeft(14, '0'); // Ensure 14 digits, padding with leading zeros
            // Generate the four digits
            string idPart2 = random.Next(1000, 9999).ToString();

            string lessonID = "LID-" + idPart1 + "-" + idPart2;

            string lessonTitle = LessonTitle.Text;
            string lessonDesc = LessonDesc.Text;
            string courseID = Request.QueryString["courseId"]; // Get the CourseID from the query string

            // Insert the new lesson into the database
            string query = "INSERT INTO [dbo].[CourseLesson] ([LessonID], [LessonTitle], [LessonDesc], [CourseID]) " +
                           "VALUES (@LessonID, @LessonTitle, @LessonDesc, @CourseID)";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@LessonID", lessonID);
                cmd.Parameters.AddWithValue("@LessonTitle", lessonTitle);
                cmd.Parameters.AddWithValue("@LessonDesc", lessonDesc);
                cmd.Parameters.AddWithValue("@CourseID", courseID);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();



                    // Reload the lessons to reflect the new addition (optional)
                    string role = Session["Role"]?.ToString();
                    Load_Lesson(role, courseID); // Refresh lesson list after creating a new one
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur
                    ClientScript.RegisterStartupScript(this.GetType(), "LessonCreationError", $"alert('An error occurred: {ex.Message}');", true);
                }
            }
            UploadMaterial2(courseID, lessonID);
        }


        public void UploadMaterial2(string courseId, string lessonId)
        {



            string materialName = MaterialName1.Text;  // Material name from the TextBox
            string filePath = string.Empty;
            string filePaths = string.Empty;

            if (!MaterialPath1.HasFile)
            {
                // No file selected
                throw new InvalidOperationException("Please select a file to upload.");
            }

            // Define the folder where files will be saved

            string uploadFolder = Server.MapPath("~/Assets/Images/course/courseMaterial/" + courseId + "/" + lessonId);
            string savematerial = $"/Assets/Images/course/courseMaterial/{courseId}/{lessonId}";



            // Create the folder if it doesn't exist
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Get the file extension and ensure it's a valid file type
            string fileExtension = Path.GetExtension(MaterialPath1.FileName).ToLower();
            string[] allowedExtensions = { ".pdf", ".docx", ".pptx", ".txt" };

            if (!allowedExtensions.Contains(fileExtension))
            {
                // Invalid file type
                throw new InvalidOperationException("Invalid file type. Please upload a PDF, DOCX, PPTX, or TXT file.");
            }

            // Define the file name and path
            string fileName = $"{Guid.NewGuid()}{fileExtension}"; // Unique name using GUID
            filePath = Path.Combine(uploadFolder, fileName);
            filePaths = Path.Combine(savematerial, fileName);

            // Save the file to the server
            MaterialPath1.SaveAs(filePath);

            // Save the material information to the database
            SaveMaterialToDatabase(lessonId, materialName, filePaths);
            string script = @"
    alert('Lesson created successfully!');
    window.location.href = window.location.href;
";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessMessage", script, true);

        }


        private bool CheckForAvailableCourses(string courseID)
        {

            int courseCount = 0;

            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [dbo].[CourseLesson] WHERE CourseID = @CourseID", con))
            {
                cmd.Parameters.AddWithValue("@CourseID", courseID);
                con.Open();
                courseCount = (int)cmd.ExecuteScalar();
                con.Close();
            }

            return courseCount > 0;  // Return true if there are courses, otherwise false
        }




    }
}
