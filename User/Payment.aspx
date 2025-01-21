<%@ Page Language="C#" MasterPageFile="~/AlgoLab.Master" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="AlgoLab.Payment" %>

<%--Custom Page Title--%>
<asp:Content ID="PaymentTitle" ContentPlaceHolderID="ContentPlaceHolder_Title" runat="server">
    Payment
</asp:Content>

<%--Custom Style Sheet--%>
<asp:Content ID="PaymentHead" ContentPlaceHolderID="ContentPlaceHolder_CustomHead" runat="server">
    <link href="../Assets/Stylesheets/payment.css" rel="stylesheet" type="text/css" />
    <link href="../Assets/Stylesheets/printReceipt.css" rel="stylesheet" type="text/css" media="print" />
    <script type="text/javascript" async>
        document.addEventListener("DOMContentLoaded", function () {
            const rblPaymentMeth = document.querySelector(".rblPaymentMeth");
            const defaultForm = document.querySelector(".payment_DefaultForm");
            const creditCardForm = document.querySelector(".payment_CreditCardForm");
            const paypalForm = document.querySelector(".payment_PayPalForm");

            // Event listener for radio button changes
            rblPaymentMeth.addEventListener("change", function (event) {
                const selectedValue = event.target.value;

                // Hide all forms first
                defaultForm.style.display = "none";
                creditCardForm.style.display = "none";
                paypalForm.style.display = "none";

                // Display the corresponding form based on the selected value
                if (selectedValue === "Credit Card") {
                    creditCardForm.style.display = "flex";
                } else if (selectedValue === "PayPal") {
                    paypalForm.style.display = "flex";
                }
            });

            // Check if a payment method is already selected when the page loads
            const selectedValue = rblPaymentMeth.querySelector('input:checked')?.value;

            // If no payment method is selected, show the default form
            if (!selectedValue) {
                defaultForm.style.display = "block"; // Show the default form if nothing is selected
            } else if (selectedValue === "Credit Card") {
                creditCardForm.style.display = "flex"; // Show Credit Card form if selected
                defaultForm.style.display = "none";
            } else if (selectedValue === "PayPal") {
                paypalForm.style.display = "flex"; // Show PayPal form if selected
                defaultForm.style.display = "none";
            }
        });
    </script>


</asp:Content>

<%--Custom Page Body--%>
<asp:Content ID="PaymentBody" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

    <!--Section 0 : Receipt Heading-->
    <section id="printHeading">
        <div class="receiptLogo">
            <img src="../Assets/Images/Logo.png" alt="AlgoLab Logo" />
        </div>
        <div class="receiptAddress">
            <address>
                <p>- WHERE ALGORITHMS MEET INNOVATION -</p>
                <br />
                Tel No : <a href="tel:+1072335546">+07-233-5546</a><br />
                Email : <a href="mailto:algolab17@gmail.com">algolab17@gmail.com</a><br />
                Visit Us : <a href="<%= ResolveUrl("~/Home.aspx") %>">https://www.algolab.com</a><br />
            </address>
        </div>
    </section>

    <!--Section 1 : Payment Card Display-->
    <section id="paymentCard">
        <h3 class="paymendCardTitle">Payment</h3>
        <!--Part 1 : Payment Breadcrumb Steps-->
        <div id="paymentStep">
            <div class="step" id="StepSummary" runat="server">
                <p class="stepLabel">Summary</p>
                <div class="bullet" id="BulletSummary" runat="server">
                    <span>1</span>
                </div>
                <i class="fa fa-check check" id="StepCheckSummary" runat="server"></i>
            </div>
            <div class="step" id="StepDetails" runat="server">
                <p class="stepLabel">Details</p>
                <div class="bullet" id="BulletDetails" runat="server">
                    <span>2</span>
                </div>
                <i class="fa fa-check check" id="StepCheckDetails" runat="server"></i>
            </div>
            <div class="step" id="StepConfirm" runat="server">
                <p class="stepLabel">Confirmation</p>
                <div class="bullet" id="BulletConfirm" runat="server">
                    <span>3</span>
                </div>
                <i class="fa fa-check check" id="StepCheckConfirm" runat="server"></i>
            </div>
            <div class="step" id="StepComplete" runat="server">
                <p class="stepLabel">Complete</p>
                <div class="bullet" id="BulletComplete" runat="server">
                    <span>4</span>
                </div>
                <i class="fa fa-check check" id="StepCheckComplete" runat="server"></i>
            </div>
        </div>

        <!--Part 2 : Payment Form-->
        <%--        <asp:HiddenField ID="HiddenFieldCurrentStep" runat="server" />
<%--        <asp:HiddenField ID="HiddenFieldPaymentDetails" runat="server" />
        <asp:HiddenField ID="HiddenFieldPaymentConfirm" runat="server" />
        <asp:HiddenField ID="HiddenFieldPaymentComplete" runat="server" />--%>

        <div class="paymentForm_wrapper">
            <div id="paymentForm">
                <!--Step 1 : Payment Summary-->
                <div class="formPage" id="FormPageSummary" runat="server">
                    <h4 class="formPageTitle">Summary</h4>

                    <div class="formContent">
                        <div class="formContenLeft">
                            <div class="courseEnrolImgTxt">
                                <div class="courseCardEnrolImg" id="SummaryCourseCardImg" runat="server"></div>
                                <div class="courseCardEnrolNameDescPrice">
                                    <asp:Label ID="lblSummaryCourseName" runat="server" CssClass="courseCardEnrolName" Text="" />
                                    <asp:Label ID="lblSummaryCourseDesc" runat="server" CssClass="courseCardEnrolDesc" Text="" />

                                    <div class="courseCardEnrolPriceLabel_wrapper">
                                        <p class="courseCardEnrolLabel">$</p>
                                        <asp:Label ID="lblSummaryCoursePrice" runat="server" CssClass="courseCardEnrolPrice" Text="" />
                                    </div>
                                </div>
                            </div>

                            <div class="courseEnrolDetails">
                                <div class="courseEnrolDetailsBox">
                                    <p class="courseEnrolLabel">Level</p>
                                    <asp:Label ID="lblSummaryCourseLevel" runat="server" Text="" CssClass="courseEnrolContent" />
                                </div>
                                <div class="courseEnrolDetailsBox">
                                    <p class="courseEnrolLabel">Language</p>
                                    <asp:Label ID="lblSummaryCourseLang" runat="server" Text="" CssClass="courseEnrolContent" />
                                </div>
                            </div>
                        </div>

                        <div class="formContenRight billingBtn_wrapper">
                            <div class="billingSummary_wrapper">
                                <div class="billingSummary">
                                    <p class="billingLabel">Price</p>
                                    <asp:Label ID="lblBillingPrice" runat="server" Text="" CssClass="billingAmt" />

                                    <%--<p class="billingAmt">$350.00</p>--%>
                                </div>
                                <div class="billingSummary">
                                    <p class="billingLabel">Tax (6%)</p>
                                    <asp:Label ID="lblBillingTax" runat="server" Text="" CssClass="billingAmt" />
                                    <%--<p class="billingAmt">+ $12.00</p>--%>
                                </div>
                                <div class="billingSummary">
                                    <asp:Label ID="lblBillingDiscountLabel" runat="server" Text="Discount" CssClass="billingLabel" />
                                    <%--<p class="billingLabel">Discount (15%)</p> <!--New Year Eve Discount: 15%)-->--%>
                                    <asp:Label ID="lblBillingDiscount" runat="server" Text="" CssClass="billingAmt" />
                                    <%--<p class="billingAmt">- $0.00</p>--%>
                                </div>
                                <div class="billingGrandTotal">
                                    <p class="billingLabel">Grand Total</p>
                                    <asp:Label ID="lblBillingGrandTotal" runat="server" Text="" CssClass="billingAmt" />
                                    <%--<p class="billingAmt">$362.00</p>--%>
                                </div>

                            </div>

                            <div class="backNextBtn_wrapper">
                                <asp:Button ID="btnCancelPay" runat="server" Text="Cancel" CssClass="payPrevBtn" OnClick="btnCancelPay_Click" />
                                <asp:Button ID="btnNextDetails" runat="server" Text="Next" CssClass="payNextBtn" OnClick="btnNextDetails_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <!--Step 2 : Payment Details [Method + Details]-->
                <div class="formPage" id="FormPageDetails" runat="server">
                    <h4 class="formPageTitle">Details</h4>

                    <div class="formContent">
                        <div class="formContenLeft paymentMeth_wrapper">
                            <asp:RadioButtonList ID="rblPaymentMeth" runat="server" CssClass="rblPaymentMeth">
                                <asp:ListItem Value="Credit Card">Credit Card</asp:ListItem>
                                <asp:ListItem Value="PayPal">PayPal</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>

                        <div class="formContenRight paymentDetailsForm">
                            <div class="payment_TotalAmt">
                                <h4>Amount</h4>
                                <asp:Label ID="lblPayAmt" runat="server" Text="$362.00" CssClass="amountLabel"></asp:Label>
                            </div>

                            <div class="payment_DefaultForm">
                                <h3>Please select a payment method</h3>
                            </div>

                            <div class="payment_CreditCardForm">
                                <asp:TextBox ID="txtCardNo" runat="server" placeholder="Card Number" MaxLength="16" CssClass="txtPayment"></asp:TextBox>
                                <asp:TextBox ID="txtCardValidThru" runat="server" placeholder="MMYY" MaxLength="4" CssClass="txtPayment" />
                                <asp:TextBox ID="txtCardCVV" runat="server" placeholder="CVV" MaxLength="3" CssClass="txtPayment"></asp:TextBox>
                            </div>

                            <div class="payment_PayPalForm">
                                <asp:TextBox ID="txtPaypalUsername" runat="server" placeholder="PayPal Username" MaxLength="30" CssClass="txtPayment"></asp:TextBox>
                                <div class="password-box">
                                    <asp:TextBox
                                        ID="txtPaypalPassword"
                                        runat="server"
                                        placeholder="PayPal Password"
                                        MaxLength="30"
                                        TextMode="Password"
                                        CssClass="txtPayment"></asp:TextBox>
                                    <button type="button" id="togglePassword">
                                        <i class='fa fa-eye'></i>
                                    </button>
                                </div>
                                <script type="text/javascript">
                                    document.getElementById("togglePassword").addEventListener("click", function () {
                                        const passwordBox = document.getElementById('<%= txtPaypalPassword.ClientID %>');
                                        const toggleBtn = this;
                                        if (passwordBox.type === "password") {
                                            passwordBox.type = "text";
                                            toggleBtn.innerHTML = "<i class='fa fa-eye-slash'></i>"; // Change icon to "hide"
                                        } else {
                                            passwordBox.type = "password";
                                            toggleBtn.innerHTML = "<i class='fa fa-eye'></i>"; // Change icon back to "show"
                                        }
                                    });
                                </script>


                            </div>

                            <div class="backNextBtn_wrapper">
                                <asp:Button ID="btnPrevSummary" runat="server" Text="Back" CssClass="payPrevBtn" OnClick="btnPrevSummary_Click" />
                                <asp:Button ID="btnNextConfirm" runat="server" Text="Next" CssClass="payNextBtn" OnClick="btnNextConfirm_Click" />
                            </div>
                        </div>

                    </div>
                </div>

                <!--Step 3 : Payment Confirmation-->
                <div class="formPage" id="FormPageConfirm" runat="server">
                    <h4 class="formPageTitle">Confirmation</h4>

                    <div class="formContent">
                        <div class="paymentConfirm_wrapper">
                            <p>Please enter the 6-digit code sent to </p>
                            <asp:Label ID="lblCustEmail" runat="server" Text="t***4@gmail.com" CssClass="labelCustEmail"></asp:Label>

                            <div class="paymentConfirm_OTP">
                                <asp:TextBox ID="txtOTP1" runat="server" MaxLength="1" CssClass="otpBox" />
                                <asp:TextBox ID="txtOTP2" runat="server" MaxLength="1" CssClass="otpBox" />
                                <asp:TextBox ID="txtOTP3" runat="server" MaxLength="1" CssClass="otpBox" />
                                <asp:TextBox ID="txtOTP4" runat="server" MaxLength="1" CssClass="otpBox" />
                                <asp:TextBox ID="txtOTP5" runat="server" MaxLength="1" CssClass="otpBox" />
                                <asp:TextBox ID="txtOTP6" runat="server" MaxLength="1" CssClass="otpBox" />
                            </div>

                            <div class="paymentConfirm_warning" id="warningDiv" runat="server">
                                <p>
                                    Number of Attempts Left: 
                                    <asp:Label ID="lblOtpLeftAttempt" runat="server" Text=""></asp:Label>
                                    of
                                    <asp:Label ID="lblOtpTotalAttempt" runat="server" Text=""></asp:Label>
                                </p>
                            </div>


                            <div class="paymentConfirm_Btn">
                                <asp:Button ID="btnResendOTP" runat="server" Text="Resend" CssClass="paymentConfirmResendBtn" OnClick="btnResendOTP_Click" />

                                <asp:Button ID="btnCancelCode" runat="server" Text="Cancel" CssClass="paymentConfirmCancel" OnClick="btnCancelCode_Click" />

                                <asp:Button ID="btnSubmitCode" runat="server" Text="Submit" CssClass="payNextBtn paymentConfirmSubmitCode" OnClick="btnSubmitCode_Click" />
                            </div>
                        </div>
                    </div>
                </div>

                <!--Step 4.1 : Payment Complete + Receipt + [Failed]-->
                <div class="formPage receipt" id="FormPageComplete" runat="server">
                    <h4 class="formPageTitle">Complete</h4>

                    <div class="formContent">
                        <div class="paymentComplete_wrapper">
                            <div class="paymentStatus_img"></div>

                            <div class="paymentSuccess" id="PaymentSuccessForm" runat="server">
                                <p class="paymentComplete_comment">
                                    Payment Successful

                                Thanks for enrolling in
                                <br />
                                    <asp:Label ID="lblCourseEnrolPaid" runat="server" Text="" CssClass="labelPaidCourseName"></asp:Label>
                                </p>
                                <asp:Button ID="btnAccessCourse" runat="server" Text="Access Course" CssClass="accessCourseButton" OnClick="btnAccessCourse_Click" />

                                <div class="paymentComplete_receiptDetails">
                                    <h3>Registration Summary</h3>

                                    <div class="paymentComplete_receiptNoTransDatetime">
                                        <p class="receiptNo">
                                            Receipt No &nbsp;&nbsp;
                                        <asp:Label ID="lblReceiptNo" runat="server" Text="" CssClass="receiptNoDatetime"></asp:Label>
                                        </p>

                                        <p class="transactionDatetime">
                                            <asp:Label ID="lblTransDatetime" runat="server" Text="" CssClass="receiptNoDatetime"></asp:Label>
                                        </p>
                                    </div>

                                    <div class="receiptCourse">
                                        <!--Course Name & Price-->
                                        <div class="paymentComplete_receiptRow">
                                            <asp:Label ID="lblEnrolCourseName" runat="server" Text="" CssClass="labelEnrolReceipt"></asp:Label>
                                            <asp:Label ID="lblEnrolCoursePrice" runat="server" Text="" CssClass="labelEnrolReceipt"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="receiptSubtotalTax">
                                        <!--Subtotal-->
                                        <div class="paymentComplete_receiptRow">
                                            <p class="labelEnrolReceipt">Subtotal</p>
                                            <asp:Label ID="lblPaidSubtoalAmt" runat="server" Text="" CssClass="labelEnrolReceipt"></asp:Label>
                                        </div>

                                        <!--Tax-->
                                        <div class="paymentComplete_receiptRow">
                                            <p class="labelEnrolReceipt">Tax (6%)</p>
                                            <asp:Label ID="lblPaidTaxAmt" runat="server" Text="" CssClass="labelEnrolReceipt"></asp:Label>
                                        </div>

                                        <!--Discount-->
                                        <div class="paymentComplete_receiptRow">
                                            <p class="labelEnrolReceipt">Discount</p>
                                            <asp:Label ID="lblPaidDiscountAmt" runat="server" Text="" CssClass="labelEnrolReceipt"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="receiptGrandTotal">
                                        <!--Grand Total-->
                                        <div class="paymentComplete_receiptRow">
                                            <p class="labelEnrolReceipt">Grand Total</p>
                                            <asp:Label ID="lblPaidGrandTotalAmt" runat="server" Text="" CssClass="labelEnrolReceipt"></asp:Label>
                                        </div>
                                    </div>
                                </div>


                                <div class="paymentComplete_btn">
                                    <div class="receiptPrintBtn" onclick="window.print();">Print Receipt</div>
                                    <div class="browseCourseBtn" onclick="window.location.href = '../Course.aspx'">
                                        Browse Course
                                    </div>
                                </div>

                            </div>

                            <div class="paymentFailed" id="PaymentFailedForm" runat="server">
                                <h3>Whoops!</h3>
                                <p class="paymentComplete_comment">
                                    Something went wrong. We couldn&rsquo;t process your payment. Kindly contact our support for further assistance.
                                </p>

                                <div class="paymentFailed_btn">
                                    <!--Failed: May due to not receive verification code-->
                                    <asp:Button ID="btnCancelPayment" runat="server" Text="Back to Home" CssClass="cancelPaymentButton" OnClick="btnCancelPayment_Click"/>
                                    <asp:Button ID="btnRetryPayment" runat="server" Text="Try Again" CssClass="retryPaymentButton" OnClick="btnRetryPayment_Click"/>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

    </section>


    <script type="text/javascript" src="../Assets/Scripts/payment.js"></script>
</asp:Content>
