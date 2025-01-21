function showDetailWindow(tutorInfoID) {
    var detailWindow = document.getElementById('Detail-' + tutorInfoID);
    var viewButton = document.querySelector(`#TutorInfo-${tutorInfoID} .ViewDetailBtn`);

    // Toggle the 'collapsed' class
    if (detailWindow.classList.contains('collapsed')) {
        detailWindow.classList.remove('collapsed');
        detailWindow.classList.add('expanded');
        viewButton.textContent = 'Close Detail'; // Change button text to 'Close Detail'
    } else {
        detailWindow.classList.remove('expanded');
        detailWindow.classList.add('collapsed');
        viewButton.textContent = 'View Detail'; // Change button text back to 'View Detail'
    }
}
function handleAction(tutorInfoID, action) {
    const method = action === "Approve" ? "ApproveRequest" : "RejectRequest";
    const payload = { tutorInfoID };

    // For "Approve" action, include additional data
    if (action === "Approve") {
        const BonusRateInput = document.getElementById(`BonusRate-${tutorInfoID}`);
        const commissionInput = document.getElementById(`Commission-${tutorInfoID}`);

        // Include only if user entered values
        if (BonusRateInput && BonusRateInput.value.trim() !== "") {
            payload.newBonusRate = parseFloat(BonusRateInput.value.trim());
        }
        if (commissionInput && commissionInput.value.trim() !== "") {
            payload.newCommission = parseFloat(commissionInput.value.trim());
        }
    }

    document.body.style.cursor = "wait"; // Show loading cursor

    fetch(`ApproveTutor.aspx/${method}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
    })
        .then(response => response.json())
        .then(data => {
            alert(data.d); // Show success or error message
            location.reload(); // Reload the page to update the list
        })
        .catch(error => console.error("Error:", error))
        .finally(() => {
            document.body.style.cursor = "default"; // Reset cursor
        });
}

