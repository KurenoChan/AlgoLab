document.addEventListener("DOMContentLoaded", function () {
    const otpInputs = document.querySelectorAll(".otpBox"); // Select all OTP input boxes
    otpInputs.forEach((input, index) => {
        input.addEventListener("keyup", (e) => {
            // Validate input
            const otpRegex = /^[0-9]*$/; // Allow only numbers
            if (!otpRegex.test(input.value)) {
                input.value = input.value.replace(/\D/g, ''); // Remove non-numeric characters
            }

            // Move focus
            if (input.value.length === 1) {
                if (index < otpInputs.length - 1) {
                    otpInputs[index + 1].focus(); // Move to the next input
                }
            } else if (e.key === "Backspace" && index > 0) {
                otpInputs[index - 1].focus(); // Move to the previous input on Backspace
            }
        });
    });
});

//let maxAttempts = 3;
//let attemptsLeft = maxAttempts;

//function handleResend() {
//    const warningDiv = document.getElementById('warningDiv');

//    // Handle button visibility
//    const resendBtn = document.querySelector('.paymentConfirmResendBtn');
//    const submitBtn = document.querySelector('.paymentConfirmSubmitCode'); // Ensure this matches the generated client ID
//    const cancelBtn = document.querySelector('.paymentConfirmCancel'); // Ensure this matches the generated client ID

//    if (attemptsLeft > 0) {
//        // Show warning div
//        warningDiv.style.display = 'block';

//        // Update the number of attempts left
//        const lblOtpLeftAttempt = document.getElementById('lblOtpLeftAttempt');
//        lblOtpLeftAttempt.innerText = attemptsLeft;

//        // Decrement attempts left
//        attemptsLeft--;
//    } else {
//        // Show warning div
//        warningDiv.style.display = 'none';

//        // Disable resend and submit buttons
//        resendBtn.style.display = 'none';
//        submitBtn.style.display = 'none';

//        // Show cancel button
//        cancelBtn.style.display = 'block';
//    }
//}
