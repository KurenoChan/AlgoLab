function startCountdown(seconds, redirectUrl) {
    // Create an overlay to block interaction
    const overlay = document.createElement("div");
    overlay.id = "countdownOverlay";
    overlay.style.position = "fixed";
    overlay.style.top = "0";
    overlay.style.left = "0";
    overlay.style.width = "100%";
    overlay.style.height = "100%";
    overlay.style.backgroundColor = "rgba(0, 0, 0, 0.5)";
    overlay.style.zIndex = "9999";
    overlay.style.display = "flex";
    overlay.style.justifyContent = "center";
    overlay.style.alignItems = "center";
    overlay.style.color = "white";
    overlay.style.fontSize = "24px";
    overlay.style.textAlign = "center";
    document.body.appendChild(overlay);

    // Display countdown text
    const countdownText = document.createElement("div");
    countdownText.id = "countdownText";
    overlay.appendChild(countdownText);

    // Start the countdown
    function updateCountdown() {
        if (seconds > 0) {
            countdownText.textContent = `Back to Home Page in ${seconds} seconds...`;
            seconds--;
            setTimeout(updateCountdown, 1000);
        } else {
            window.location.href = redirectUrl; // Redirect to the specified URL
        }
    }
    updateCountdown();
}