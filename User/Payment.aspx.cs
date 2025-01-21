using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static System.Net.WebRequestMethods;

namespace AlgoLab
{
    public partial class Payment : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            con.Open();

            // Check if user is logged in, redirect if not
            if (Session["UserID"] == null)
            {
                string returnUrl = HttpContext.Current.Request.Url.PathAndQuery; // Get the full path and query string of the current page
                Response.Redirect($"../AccountSelect.aspx?ReturnUrl={HttpUtility.UrlEncode(returnUrl)}");
            }

            // When the page is loaded for the first time (not a PostBack)
            if (!IsPostBack)
            {
                UpdatePaymentBreadcrumb(1); // Active Step: Summary

                // Initialize form page visibility
                FormPageSummary.Visible = true;
                FormPageDetails.Visible = false;
                FormPageConfirm.Visible = false;
                FormPageComplete.Visible = false;

                ViewState["OtpAttemptsLeft"] = 3;
            }




            // ==========================
            // Step 1 : Enrolment Summary
            // ==========================
            // Load Course Summary & Process Billing Summary [Invoice]
            SqlDataReader reader = LoadCourseSummary();

            // Display Course Summary Details

            ViewState["CourseName"] = string.Empty;
            ViewState["CourseShortDesc"] = string.Empty;
            ViewState["CourseLevel"] = string.Empty;
            ViewState["CourseLang"] = string.Empty;
            ViewState["CourseFee"] = 0;
            ViewState["CourseImgPath"] = string.Empty;

            // Process each row in the SqlDataReader
            while (reader.Read()) // Iterate through all rows
            {
                // Retrieve course details (only once since they're the same across rows)
                if (string.IsNullOrEmpty(ViewState["CourseName"].ToString())) // Only execute once for course details
                {
                    ViewState["CourseName"] = reader["CourseName"].ToString();
                    ViewState["CourseShortDesc"] = reader["CourseShortDesc"].ToString();
                    ViewState["CourseLevel"] = Course.CourseLevel_ToText(reader["CourseLevel"].ToString());
                    ViewState["CourseLang"] = Course.CourseLang_ToText(reader["CourseLang"].ToString());
                    ViewState["CourseFee"] = Convert.ToDouble(reader["CourseFee"]);
                    ViewState["CourseImgPath"] = reader["CourseImgPath"].ToString();

                }
            }

            reader.Close();

            string script = $"document.querySelector('main').style.backgroundImage = 'url(\"{ResolveUrl(ViewState["CourseImgPath"].ToString())}\")';";
            ClientScript.RegisterStartupScript(this.GetType(), "SetBackgroundImage", script, true);


            // Course Summary [LEFT]
            lblSummaryCourseName.Text = ViewState["CourseName"].ToString();
            lblSummaryCourseDesc.Text = ViewState["CourseShortDesc"].ToString();
            lblSummaryCourseLevel.Text = ViewState["CourseLevel"].ToString();
            lblSummaryCourseLang.Text = ViewState["CourseLang"].ToString();
            lblSummaryCoursePrice.Text = ViewState["CourseFee"].ToString();
            SummaryCourseCardImg.Style["background-image"] = "url('" + ResolveUrl(ViewState["CourseImgPath"].ToString()) + "')";

            const double taxRate = 0.06;

            // Billing Summary [Invoice] [RIGHT]
            lblBillingPrice.Text = Convert.ToDouble(ViewState["CourseFee"]).ToString("F2");
            lblBillingTax.Text = "+ " + (Convert.ToDouble(ViewState["CourseFee"]) * taxRate).ToString("F2");
            double taxPrice = Convert.ToDouble(ViewState["CourseFee"]) * taxRate;
            double discountPrice = ApplyNewYearDiscount(Convert.ToDouble(ViewState["CourseFee"]), 15); // 15% New Year Discount
            double grandTotal = Convert.ToDouble(ViewState["CourseFee"]) + taxPrice - discountPrice;
            lblBillingGrandTotal.Text = $"${grandTotal:F2}";

            ViewState["TaxPrice"] = taxPrice;
            ViewState["DiscountPrice"] = discountPrice;
            ViewState["GrandTotal"] = grandTotal;

            // ===================================
            // Step 2 : Payment Method and Details
            // ===================================
            lblPayAmt.Text = "$" + Convert.ToDouble(ViewState["GrandTotal"]).ToString("F2");


            // ===========================
            // Step 3 : Email Confirmation
            // ===========================

            // The full email address
            string email = Session["Email"].ToString();

            // Format the email
            string formattedEmail = FormatEmail(email);

            // Set the formatted email to the label
            lblCustEmail.Text = formattedEmail;

            if (Convert.ToInt32(ViewState["OtpAttemptsLeft"]) == 3)
            {
                warningDiv.Visible = false;
                btnResendOTP.Visible = true;
                btnCancelCode.Visible = false;
            }
            else if (Convert.ToInt32(ViewState["OtpAttemptsLeft"]) == 0)
            {
                warningDiv.Visible = true;
                btnResendOTP.Visible = false;
                btnCancelCode.Visible = true;
            }
            con.Close();
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            // Get the last error that occurred
            Exception ex = Server.GetLastError();

            // Define the folder and file paths for logging
            string logFolder = Server.MapPath("~/Logs");
            string logFile = System.IO.Path.Combine(logFolder, "PageErrorLog_Payment.txt");

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

        // --------------
        // Step 1 Buttons
        // --------------
        protected void btnCancelPay_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Course.aspx");
        }
        protected void btnNextDetails_Click(object sender, EventArgs e)
        {
            UpdatePaymentBreadcrumb(2);  // Active Step: Details

            // Update the form visibility
            FormPageSummary.Visible = false;
            FormPageDetails.Visible = true;
            FormPageConfirm.Visible = false;
            FormPageComplete.Visible = false;
        }


        // --------------
        // Step 2 Buttons
        // --------------
        protected void btnPrevSummary_Click(object sender, EventArgs e)
        {
            UpdatePaymentBreadcrumb(1);  // Active Step: Details

            // Update the form visibility
            FormPageSummary.Visible = true;
            FormPageDetails.Visible = false;
            FormPageConfirm.Visible = false;
            FormPageComplete.Visible = false;
        }

        protected void btnNextConfirm_Click(object sender, EventArgs e)
        {
            // Get the selected payment method
            string selectedPaymentMethod = rblPaymentMeth.SelectedValue;

            // Check if a payment method has been selected
            if (string.IsNullOrEmpty(selectedPaymentMethod))
            {
                // Show alert and stop further execution
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a payment method');", true);
                return; // Stop further execution after showing the alert
            }

            // 1. Credit Card Validation
            if (selectedPaymentMethod == "Credit Card")
            {
                // Validate Credit Card details
                if (ValidateCreditCard(txtCardNo.Text.ToString(), txtCardValidThru.Text.ToString(), txtCardCVV.Text.ToString()))
                {
                    UpdatePaymentBreadcrumb(3);
                    FormPageSummary.Visible = false;
                    FormPageDetails.Visible = false;
                    FormPageConfirm.Visible = true;
                    FormPageComplete.Visible = false;
                    ViewState["PaymentMeth"] = selectedPaymentMethod;
                    try
                    {
                        SendEmail();
                        return;
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                    }

                }
                else
                {
                    // Show error, keep the current form visible
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid Credit Card details');", true);
                    return;
                }
            }

            // 2. PayPal Validation
            else if (selectedPaymentMethod == "PayPal")
            {
                // Validate PayPal details
                if (ValidatePayPal(txtPaypalUsername.Text.ToString(), txtPaypalPassword.Text.ToString()))
                {
                    UpdatePaymentBreadcrumb(3);
                    FormPageSummary.Visible = false;
                    FormPageDetails.Visible = false;
                    FormPageConfirm.Visible = true;
                    FormPageComplete.Visible = false;
                    ViewState["PaymentMeth"] = selectedPaymentMethod;

                    try
                    {
                        SendEmail();
                        return;
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                    }

                }
                else
                {
                    // Show error, keep the current form visible
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid PayPal details');", true);
                    return;
                }
            }
        }


        // --------------
        // Step 3 Buttons
        // --------------
        protected void btnResendOTP_Click(object sender, EventArgs e)
        {
            const int maxOtpAttempts = 3;
            int attemptsLeft = Convert.ToInt32(ViewState["OtpAttemptsLeft"] ?? 0);

            warningDiv.Visible = true;
            lblOtpLeftAttempt.Text = ViewState["OtpAttemptsLeft"].ToString();
            lblOtpTotalAttempt.Text = maxOtpAttempts.ToString();

            if (attemptsLeft > 0)
            {
                try
                {
                    SendEmail();
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                }
            }
            else
            {
                btnResendOTP.Visible = false;
                btnCancelCode.Visible = true;
            }
        }

        protected void btnSubmitCode_Click(object sender, EventArgs e)
        {
            string otpString = ViewState["OtpCode"].ToString();
            string inputOtpString = $"{txtOTP1.Text.Trim()}{txtOTP2.Text.Trim()}{txtOTP3.Text.Trim()}{txtOTP4.Text.Trim()}{txtOTP5.Text.Trim()}{txtOTP6.Text.Trim()}";

            // Validate input
            if (string.IsNullOrEmpty(inputOtpString) || inputOtpString.Length != 6)
            {
                txtOTP1.Text = "";
                txtOTP2.Text = "";
                txtOTP3.Text = "";
                txtOTP4.Text = "";
                txtOTP5.Text = "";
                txtOTP6.Text = "";
                txtOTP1.Focus();

                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter the complete 6-digit OTP.');", true);
                return;
            }

            if (!inputOtpString.Equals(otpString))
            {
                txtOTP1.Text = "";
                txtOTP2.Text = "";
                txtOTP3.Text = "";
                txtOTP4.Text = "";
                txtOTP5.Text = "";
                txtOTP6.Text = "";
                txtOTP1.Focus();
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid OTP. Please try again.');", true);
            }
            else
            {
                txtOTP1.Text = "";
                txtOTP2.Text = "";
                txtOTP3.Text = "";
                txtOTP4.Text = "";
                txtOTP5.Text = "";
                txtOTP6.Text = "";
                txtOTP1.Focus();


                UpdatePaymentBreadcrumb(4);

                FormPageSummary.Visible = false;
                FormPageDetails.Visible = false;
                FormPageConfirm.Visible = false;
                FormPageComplete.Visible = true;
                ViewState["PaymentSuccess"] = true;
                PaymentSuccessForm.Visible = true;
                PaymentFailedForm.Visible = false;

                string script = "document.querySelector('.paymentStatus_img').style.backgroundImage = 'url(\"../Assets/Images/icons/icon_success.png\")';";
                ClientScript.RegisterStartupScript(this.GetType(), "SetPaymentStatusImg", script, true);

                ProcessPayment();
            }

        }

        protected void btnCancelCode_Click(object sender, EventArgs e)
        {
            UpdatePaymentBreadcrumb(4);

            FormPageSummary.Visible = false;
            FormPageDetails.Visible = false;
            FormPageConfirm.Visible = false;
            FormPageComplete.Visible = true;
            ViewState["PaymentSuccess"] = false;
            PaymentSuccessForm.Visible = false;
            PaymentFailedForm.Visible = true;

            string script = "document.querySelector('.paymentStatus_img').style.backgroundImage = 'url(\"../Assets/Images/icons/icon_error.png\")';";
            ClientScript.RegisterStartupScript(this.GetType(), "SetPaymentStatusImg", script, true);

            ProcessPayment();
        }


        // --------------
        // Step 4 Buttons
        // --------------
        // 1. Payment Success
        protected void btnAccessCourse_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/User/Lesson.aspx?courseId={Request.QueryString["courseId"].ToString()}");
        }

        // 2. Payment Failed
        protected void btnCancelPayment_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Home.aspx");
        }

        protected void btnRetryPayment_Click(object sender, EventArgs e)
        {
            UpdatePaymentBreadcrumb(1); // Active Step: Summary

            // Initialize form page visibility
            FormPageSummary.Visible = true;
            FormPageDetails.Visible = false;
            FormPageConfirm.Visible = false;
            FormPageComplete.Visible = false;

            ViewState["OtpAttemptsLeft"] = 3;

            rblPaymentMeth.ClearSelection();
            txtCardNo.Text = "";
            txtCardValidThru.Text = "";
            txtCardCVV.Text = "";
            txtPaypalUsername.Text = "";
            txtPaypalPassword.Text = "";
            txtOTP1.Text = "";
            txtOTP2.Text = "";
            txtOTP3.Text = "";
            txtOTP4.Text = "";
            txtOTP5.Text = "";
            txtOTP6.Text = "";
        }

        // =====================================================================================
        // STEP 1 : LOADING COURSE SUMMARY DETAILS
        // =====================================================================================
        protected SqlDataReader LoadCourseSummary()
        {
            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            string courseID = Request.QueryString["courseId"].ToString();
            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Course Where CourseID = @CourseID", con);
            cmd.Parameters.AddWithValue("@CourseID", courseID);

            //SqlDataReader reader = cmd.ExecuteReader();

            return cmd.ExecuteReader();
        }

        // New Year Discount
        private double ApplyNewYearDiscount(double originalPrice, double discountPercentage)
        {
            // Define the New Year discount period
            DateTime newYearStart = new DateTime(2024, 12, 20);
            DateTime newYearEnd = new DateTime(2025, 1, 20);
            DateTime currentDate = DateTime.Now;

            double discountedPrice = originalPrice;
            double discountAmount = 0;

            // Check if the current date is within the New Year discount period
            if (currentDate >= newYearStart && currentDate <= newYearEnd)
            {
                // Update the discount label for New Year
                lblBillingDiscountLabel.Text = $"New Year Discount ({discountPercentage}%)";

                // Calculate the discounted price
                discountAmount = originalPrice * (discountPercentage / 100);
                discountedPrice = originalPrice - discountAmount;

                // Set the discount amount in the label
                lblBillingDiscount.Text = $"- ${discountAmount:F2}";
            }
            else
            {
                // Set default values for non-discount period
                lblBillingDiscountLabel.Text = "Discount";
                lblBillingDiscount.Text = "- $0.00";
            }

            // Update the grand total label
            lblBillingGrandTotal.Text = $"${discountedPrice:F2}";

            // Return the discounted price
            return discountAmount;
        }

        // =====================================================================================
        // STEP 2 : VALIDATION ON PAYMENT DETAILS
        // =====================================================================================
        private bool ValidateCreditCard(string cardNumber, string cardValidThru, string cardCVV)
        {
            // 1.1 Credit Card Number
            if (!IsValidCreditCardNo(cardNumber))
                return false;

            // 1.2 Credit Card Valid Thru
            if (!IsValidCreditCardValidThru(cardValidThru))
                return false;

            // 1.3 Credit Card CVV
            if (!IsValidCreditCardCVV(cardCVV))
                return false;

            return true;
        }

        // Credit Card No Local Validation [Luhn Algorithm] & Check if it is only digit
        private bool IsValidCreditCardNo(string cardNumber)
        {
            // Check if the card number is not empty or null and is 16 digits long
            if (string.IsNullOrEmpty(cardNumber))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Card number cannot be empty');", true);
                return false;
            }

            if (cardNumber.Length != 16)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Card number must be 16 digits long');", true);
                return false;
            }

            // Check if the card number is numeric
            if (!cardNumber.All(char.IsDigit))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Card number must be numeric');", true);
                return false;
            }

            // Luhn Algorithm for validating the credit card number
            int sum = 0;
            bool isSecondDigit = false;

            // Loop through the card number digits from right to left
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                // Get the digit as an integer
                int digit = cardNumber[i] - '0';

                if (isSecondDigit)
                {
                    digit *= 2;

                    // If doubling results in a number greater than 9, subtract 9
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                isSecondDigit = !isSecondDigit;
            }

            // If the total sum is a multiple of 10, the card number is valid
            if (sum % 10 == 0)
            {
                return true;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid card number');", true);
                return false;
            }
        }

        // Expiration Date Validation [MMYY Format]
        private bool IsValidCreditCardValidThru(string cardValidThru)
        {
            // Check if the expiration date is not empty
            if (string.IsNullOrEmpty(cardValidThru))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Expiration date cannot be empty');", true);
                return false;
            }

            // Ensure the expiration date is exactly 4 digits long
            if (cardValidThru.Length != 4)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Expiration date must be in MMYY format');", true);
                return false;
            }

            // Check if the expiration date contains only numbers
            if (!cardValidThru.All(char.IsDigit))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Expiration date must contain only numbers');", true);
                return false;
            }

            // Extract month and year from the expiration date
            int month = int.Parse(cardValidThru.Substring(0, 2)); // First two digits are the month
            int year = int.Parse(cardValidThru.Substring(2, 2));  // Last two digits are the year

            // Check if the month is valid
            if (month < 1 || month > 12)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Month must be between 01 and 12');", true);
                return false;
            }

            // Get current date
            DateTime currentDate = DateTime.Now;

            // Ensure the expiration year is greater than or equal to the current year
            // If the year is the same, check if the month is in the future
            int currentYearLastTwoDigits = currentDate.Year % 100; // Get last two digits of the current year
            if (year < currentYearLastTwoDigits || (year == currentYearLastTwoDigits && month < currentDate.Month))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Expiration date is in the past');", true);
                return false;
            }

            return true;
        }


        private bool IsValidCreditCardCVV(string cardCVV)
        {
            // Check if the CVV is not empty
            if (string.IsNullOrEmpty(cardCVV))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('CVV cannot be empty');", true);
                return false;
            }

            // Check if the CVV is numeric
            if (!cardCVV.All(char.IsDigit))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('CVV must be numeric');", true);
                return false;
            }

            // Check if the CVV length is exactly 3 digits
            if (cardCVV.Length != 3)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('CVV must be 3 digits');", true);
                return false;
            }

            // If all checks passed, return true (valid CVV)
            return true;
        }



        private bool ValidatePayPal(string paypalUsername, string paypalPassword)
        {
            // 1. PayPal Username
            if (!IsValidPayPalUsername(paypalUsername))
                return false;
            // 2. PayPal Password
            if (!IsValidPayPalPassword(paypalPassword))
                return false;

            return true;
        }

        private bool IsValidPayPalUsername(string paypalUsername)
        {
            // Check if the entered username is null or empty
            if (string.IsNullOrEmpty(paypalUsername))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('PayPal username cannot be empty');", true);
                return false;
            }

            // Check if the entered username matches the session value
            if (paypalUsername != Session["Username"]?.ToString())
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid PayPal username!');", true);
                return false;
            }

            // If valid, return true
            return true;
        }

        private bool IsValidPayPalPassword(string paypalPassword)
        {
            // Check if the entered password is null or empty
            if (string.IsNullOrEmpty(paypalPassword))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('PayPal password cannot be empty');", true);
                return false;
            }

            // Check if the entered password matches the session value
            if (paypalPassword != Session["Password"]?.ToString())
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid PayPal password!');", true);
                return false;
            }

            // If valid, return true
            return true;
        }


        // =====================================================================================
        // STEP 3 : EMAIL VERIFICATION
        // =====================================================================================
        private string FormatEmail(string email)
        {
            // Split the email into local and domain parts
            var emailParts = email.Split('@');

            if (emailParts.Length == 2)
            {
                string localPart = emailParts[0];
                string domainPart = emailParts[1];

                // Show the first and last characters of the local part, replace the middle with '*'
                if (localPart.Length > 2)
                {
                    string formattedLocal = localPart.Substring(0, 1) + "***" + localPart.Substring(localPart.Length - 1);
                    return formattedLocal + "@" + domainPart;
                }
                else
                {
                    return email; // If the local part is too short, show it as it is
                }
            }

            return email; // Return the original email if format is invalid
        }

        private void SendEmail()
        {
            try
            {
                // Generate a 6-digit OTP (including leading zeros if needed)
                Random random = new Random();
                int otp = random.Next(0, 1000000); // Generates a random number between 0 and 999999

                // Format the OTP as a 6-digit string with leading zeros if necessary
                string otpString = otp.ToString("D6"); // "D6" ensures the number is always 6 digits (pads with leading zeros if needed)
                char digit1 = otpString[0];
                char digit2 = otpString[1];
                char digit3 = otpString[2];
                char digit4 = otpString[3];
                char digit5 = otpString[4];
                char digit6 = otpString[5];

                // SMTP client settings
                string smtpUserName = ConfigurationManager.AppSettings["smtpUserName"];
                string smtpPassword = ConfigurationManager.AppSettings["smtpPassword"];
                string smtpClient = ConfigurationManager.AppSettings["smtpClient"];
                int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
                bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]);

                // Create the email message
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(smtpUserName, "AlgoLab"),
                    Subject = "Verify Your Payment with OTP",
                    Body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #333; background-color: #f4f4f4; padding: 20px;'>
                        <div style='max-width: 600px; margin: 0 auto; background-color: rgb(0,0,0,0.8); padding: 20px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
                            <p style='text-align: center; font-size: 18px; margin-bottom: 20px; color: #fff;'>Greetings {Session["Username"].ToString()},</p>
                            <p style='font-size: 16px; line-height: 1.6; text-align: center; color: #fff;'>Thank you for choosing AlgoLab! Your AlgoLab OTP Code is:</p>
                            <div style='text-align: center; padding: 20px; font-size: 30px; font-weight: bold; color: #fff; border-radius: 4px; margin: 20px auto; display: flex; width: 80%;'>
                                <div style='
                                    border: 0.1vw solid rgba(255,255,255,0.8); 
                                    border-radius: 0.5vw; 
                                    background-color: rgba(255,255,255,0.2); 
                                    width: 50px; 
                                    position: relative; 
                                    margin: 0 auto; 
                                    '>
                                    <p style='
                                        margin: auto; text-align: center;'>
                                        {digit1}
                                    </p>
                                </div>
                                <div style='
                                    border: 0.1vw solid rgba(255,255,255,0.8); 
                                    border-radius: 0.5vw; 
                                    background-color: rgba(255,255,255,0.2); 
                                    width: 50px; 
                                    position: relative; 
                                    margin: 0 auto; 
                                    '>
                                    <p style='
                                        margin: auto; text-align: center;'>
                                        {digit2}
                                    </p>
                                </div>
                                <div style='
                                    border: 0.1vw solid rgba(255,255,255,0.8); 
                                    border-radius: 0.5vw; 
                                    background-color: rgba(255,255,255,0.2); 
                                    width: 50px; 
                                    position: relative; 
                                    margin: 0 auto; 
                                    '>
                                    <p style='
                                        margin: auto; text-align: center;'>
                                        {digit3}
                                    </p>
                                </div>
                                <div style='
                                    border: 0.1vw solid rgba(255,255,255,0.8); 
                                    border-radius: 0.5vw; 
                                    background-color: rgba(255,255,255,0.2); 
                                    width: 50px; 
                                    position: relative; 
                                    margin: 0 auto; 
                                    '>
                                    <p style='
                                        margin: auto; text-align: center;'>
                                        {digit4}
                                    </p>
                                </div>
                                <div style='
                                    border: 0.1vw solid rgba(255,255,255,0.8); 
                                    border-radius: 0.5vw; 
                                    background-color: rgba(255,255,255,0.2); 
                                    width: 50px; 
                                    position: relative; 
                                    margin: 0 auto; 
                                    '>
                                    <p style='
                                        margin: auto; text-align: center;'>
                                        {digit5}
                                    </p>
                                </div>
                                <div style='
                                    border: 0.1vw solid rgba(255,255,255,0.8); 
                                    border-radius: 0.5vw; 
                                    background-color: rgba(255,255,255,0.2); 
                                    width: 50px; 
                                    position: relative; 
                                    margin: 0 auto; 
                                    '>
                                    <p style='
                                        margin: auto; text-align: center;'>
                                        {digit6}
                                    </p>
                                </div>
                            </div>
                            <div style='text-align: center;'>
                                <p style='font-size: 14px; color: #888;'>Need help? Our support team is just a message away!</p>
                                <p style='color: #777;'><a href='mailto:algolab17@gmail.com' style='color: #aaa;'>Email us</a> or <a href='tel:+1072335546' style='color: #aaa;'>Contact Us</a> here</p>
                            </div>
                        </div>
                    </body>
                </html>",
                    IsBodyHtml = true // Setting the body as HTML to render the styled content
                };

                // Add the recipient
                mail.To.Add(Session["Email"].ToString()); 

                // Configure the SMTP client
                SmtpClient smtp_Client = new SmtpClient(smtpClient, smtpPort)
                {
                    Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword),
                    EnableSsl = enableSSL
                };

                // Send the email
                smtp_Client.Send(mail);

                ViewState["OtpCode"] = otpString;

                if (ViewState["OtpAttemptsLeft"] != null)
                {
                    int attemptsLeft = Convert.ToInt32(ViewState["OtpAttemptsLeft"] ?? 0);
                    attemptsLeft--;
                    ViewState["OtpAttemptsLeft"] = attemptsLeft;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in sending email: {ex.Message}");
            }
        }


        // =====================================================================================
        // STEP 4 : PAYMENT PROCESSING & RECEIPT GENERATION
        // =====================================================================================
        // Load into Payment Table for the transaction details & Enrolment Information into CourseEnrolment Table
        private void ProcessPayment()
        {
            // =============
            // PAYMENT TABLE
            // =============
            DateTime transactionEnrolmentDatetime = DateTime.Now;
            string transactionNo = GenerateUniqueTransactionNo(transactionEnrolmentDatetime);
            string courseName = ViewState["CourseName"].ToString();
            string courseFee = Convert.ToDouble(ViewState["CourseFee"]).ToString("F2");
            string taxPrice = Convert.ToDouble(ViewState["TaxPrice"]).ToString("F2");
            string discountPrice = Convert.ToDouble(ViewState["DiscountPrice"]).ToString("F2");
            string grandTotal = Convert.ToDouble(ViewState["GrandTotal"]).ToString("F2");

            string paymentMeth = ViewState["PaymentMeth"].ToString();
            string custID = Session["UserID"].ToString();

            string transactionStatus = string.Empty;

            // ======================
            // COURSE ENROLMENT TABLE
            // ======================
            string courseRegNo = GenerateUniqueCourseRegNo(transactionEnrolmentDatetime);
            string enrolmentStatus = string.Empty;
            string courseID = Request.QueryString["courseId"].ToString();


            if (ViewState["PaymentSuccess"] != null && (bool)ViewState["PaymentSuccess"])
            {
                transactionStatus = "Completed";
                enrolmentStatus = "Active";
            }
            else
            {
                transactionStatus = "Failed";
                enrolmentStatus = "Failed";
            }


            // ----------------------------------------------
            // 1. Save Transaction Details into Payment Table
            // Open the connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            SqlCommand cmdPayment = new SqlCommand(
                "INSERT INTO Payment (TransactionNo, TransactionDatetime, TransactionAmount, PaymentMethod, TransactionStatus, CustID) " +
                "VALUES (@TransactionNo, @TransactionDatetime, @TransactionAmount, @PaymentMethod, @TransactionStatus, @CustID);", con);

            cmdPayment.Parameters.AddWithValue("@TransactionNo", transactionNo);
            cmdPayment.Parameters.AddWithValue("@TransactionDatetime", transactionEnrolmentDatetime);
            cmdPayment.Parameters.AddWithValue("@TransactionAmount", grandTotal);
            cmdPayment.Parameters.AddWithValue("@PaymentMethod", paymentMeth);
            cmdPayment.Parameters.AddWithValue("@TransactionStatus", transactionStatus);
            cmdPayment.Parameters.AddWithValue("@CustID", custID);

            try
            {
                cmdPayment.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Transaction Insertion Failed", $"alert('Error in saving payment records: {ex}')", true);
            }

            // ----------------------------------------------------
            // 2. Save/Update Enrolment Details into CourseEnrolment Table
            // 1. Check if the record already exists
            string checkQuery = "SELECT COUNT(1) FROM CourseEnrolment WHERE CourseID = @CourseID AND CustID = @CustID";

            // Use the existing connection (con)
            SqlCommand checkCmd = new SqlCommand(checkQuery, con);
            checkCmd.Parameters.AddWithValue("@CourseID", courseID);
            checkCmd.Parameters.AddWithValue("@CustID", custID);

            int recordCount = (int)checkCmd.ExecuteScalar();  // ExecuteScalar returns the first column of the first row

            if (recordCount > 0)  // Record exists, perform update
            {
                // Define the query to update the record
                string updateQuery = "UPDATE CourseEnrolment " +
                                     "SET EnrolmentStatus = @EnrolmentStatus, EnrolmentDate = @EnrolmentDate " +
                                     "WHERE CourseID = @CourseID AND CustID = @CustID";

                SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                updateCmd.Parameters.AddWithValue("@EnrolmentStatus", enrolmentStatus);
                updateCmd.Parameters.AddWithValue("@EnrolmentDate", transactionEnrolmentDatetime);
                updateCmd.Parameters.AddWithValue("@CourseID", courseID);
                updateCmd.Parameters.AddWithValue("@CustID", custID);

                try
                {
                    updateCmd.ExecuteNonQuery();  // Perform the update
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Transaction Update Failed",
                        $"alert('Error in updating course enrolment records: {ex}')", true);
                }
            }
            else  // Record does not exist, perform insert
            {
                // Define the query to insert a new record
                string insertQuery = "INSERT INTO CourseEnrolment (CourseRegNo, EnrolmentStatus, EnrolmentDate, CustID, CourseID) " +
                                     "VALUES (@CourseRegNo, @EnrolmentStatus, @EnrolmentDate, @CustID, @CourseID)";

                SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@CourseRegNo", courseRegNo);
                insertCmd.Parameters.AddWithValue("@EnrolmentStatus", enrolmentStatus);
                insertCmd.Parameters.AddWithValue("@EnrolmentDate", transactionEnrolmentDatetime);
                insertCmd.Parameters.AddWithValue("@CustID", custID);
                insertCmd.Parameters.AddWithValue("@CourseID", courseID);

                try
                {
                    insertCmd.ExecuteNonQuery();  // Perform the insert
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Transaction Insertion Failed",
                        $"alert('Error in saving course enrolment records: {ex}')", true);
                }
            }

            // Ensure the connection is closed properly after the operation
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }


            // 2. Display Payment Completion Message [Success/Failed]
            if (ViewState["PaymentSuccess"] != null && (bool)ViewState["PaymentSuccess"])
            {
                lblCourseEnrolPaid.Text = courseName;
                lblReceiptNo.Text = transactionNo;
                lblTransDatetime.Text = transactionEnrolmentDatetime.ToString("M/d/yyyy h:mm:ss tt");
                lblEnrolCourseName.Text = courseName;
                lblEnrolCoursePrice.Text = courseFee;
                lblPaidSubtoalAmt.Text = courseFee;
                lblPaidTaxAmt.Text = taxPrice;
                lblPaidDiscountAmt.Text = discountPrice;
                lblPaidGrandTotalAmt.Text = grandTotal;
            }

            ViewState.Clear();

        }


        // Generate a unique TransactionNos
        private string GenerateUniqueTransactionNo(DateTime transactionDatetime)
        {
            string prefix = "TRS-";
            string timestamp = transactionDatetime.ToString("yyyyMMddHHmmss");  // Use the passed transactionDatetime for the timestamp
            string newTransactionNo = $"{prefix}{timestamp}-0001"; // Start with the default sequence number (0001)

            // Get all existing TransactionNos from the database and check for uniqueness
            List<string> existingTransactionNos = GetAllExistingTransactionNos();

            // Keep generating a new ID until it's unique
            while (existingTransactionNos.Contains(newTransactionNo))
            {
                // Increment the sequence number to avoid duplication
                string lastSequence = newTransactionNo.Split('-')[1]; // Get the sequence part (after the second '-')
                int nextSeq = int.Parse(lastSequence) + 1; // Increment the sequence number
                newTransactionNo = $"{prefix}{timestamp}-{nextSeq:D4}"; // Ensure the sequence is always 4 digits (e.g., 0001, 0002)
            }

            return newTransactionNo;
        }

        // Get all existing TransactioNos from the database
        private List<string> GetAllExistingTransactionNos()
        {
            List<string> transactionNos = new List<string>();

            // Query to get all TransactioNos
            SqlCommand cmd = new SqlCommand("SELECT TransactionNo FROM Payment", con);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string transactionNo = reader["TransactionNo"].ToString();
                    transactionNos.Add(transactionNo); // Add each ID to the list
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

            return transactionNos;
        }

        // Generate a unique CourseRegNos
        private string GenerateUniqueCourseRegNo(DateTime enrolmentDatetime)
        {
            string prefix = "CREG-";
            string timestamp = enrolmentDatetime.ToString("yyyyMMddHHmmss");  // Use the passed transactionDatetime for the timestamp
            string newCourseRegNo = $"{prefix}{timestamp}-0001"; // Start with the default sequence number (0001)

            // Get all existing TransactionNos from the database and check for uniqueness
            List<string> existingCourseRegNos = GetAllExistingCourseRegNos();

            // Keep generating a new ID until it's unique
            while (existingCourseRegNos.Contains(newCourseRegNo))
            {
                // Increment the sequence number to avoid duplication
                string lastSequence = newCourseRegNo.Split('-')[1]; // Get the sequence part (after the second '-')
                int nextSeq = int.Parse(lastSequence) + 1; // Increment the sequence number
                newCourseRegNo = $"{prefix}{timestamp}-{nextSeq:D4}"; // Ensure the sequence is always 4 digits (e.g., 0001, 0002)
            }

            return newCourseRegNo;
        }

        // Get all existing CourseRegNos from the database
        private List<string> GetAllExistingCourseRegNos()
        {
            List<string> courseRegNos = new List<string>();

            // Query to get all TransactioNos
            SqlCommand cmd = new SqlCommand("SELECT CourseRegNo FROM CourseEnrolment", con);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string courseRegNo = reader["CourseRegNo"].ToString();
                    courseRegNos.Add(courseRegNo); // Add each ID to the list
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

            return courseRegNos;
        }


        // =====================================================================================
        // BREADCRUMBS STEPS UPDATE
        // =====================================================================================
        private void UpdatePaymentBreadcrumb(int currentStep)
        {
            // List of steps, with each step containing the elements we need to modify (step, bullet, check icon, stepIndex)
            var steps = new List<(HtmlGenericControl step, HtmlGenericControl bullet, HtmlGenericControl checkIcon, int stepIndex)>
    {
        (StepSummary, BulletSummary, StepCheckSummary, 1), // Summary - Step 1
        (StepDetails, BulletDetails, StepCheckDetails, 2), // Details - Step 2
        (StepConfirm, BulletConfirm, StepCheckConfirm, 3), // Confirm - Step 3
        (StepComplete, BulletComplete, StepCheckComplete, 4) // Complete - Step 4
    };

            // Helper function to update each step's state
            void SetStepState(HtmlGenericControl step, HtmlGenericControl bullet, HtmlGenericControl checkIcon, string stepState)
            {
                // Reset classes for all elements
                step.Attributes["class"] = step.Attributes["class"].Replace("active", "").Replace("inactive", "").Trim();
                bullet.Attributes["class"] = bullet.Attributes["class"].Replace("active", "").Replace("passed", "").Trim();
                checkIcon.Attributes["class"] = checkIcon.Attributes["class"].Replace("active", "").Trim();

                // Set classes based on the step's state
                switch (stepState)
                {
                    case "active":
                        step.Attributes["class"] += " active";  // Activate the step
                        break;

                    case "passed":
                        bullet.Attributes["class"] += " passed";  // Mark the bullet as passed
                        checkIcon.Attributes["class"] += " active";  // Activate the check icon
                        break;

                    case "inactive":
                        step.Attributes["class"] += " inactive";  // Mark the step as inactive
                        break;

                    default:
                        // For any null or invalid states, leave the element as is
                        break;
                }
            }

            // Reset all steps initially based on the currentStep
            foreach (var step in steps)
            {
                string stepState = "inactive"; // Default state is inactive for all steps

                // Set the stepState based on the current step
                if (step.stepIndex == currentStep)
                {
                    stepState = "active"; // Current step should be active
                }
                else if (step.stepIndex < currentStep)
                {
                    stepState = "passed"; // Passed steps should be marked as passed
                }

                // Apply the state to the step, bullet, and check icon
                SetStepState(step.step, step.bullet, step.checkIcon, stepState);
            }
        }
    }
}