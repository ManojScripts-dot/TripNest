document.getElementById('contact-form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const name = document.getElementById("name").value.trim();
    const email = document.getElementById("email").value.trim();
    const message = document.getElementById("message").value.trim();
    const feedback = document.getElementById("form-feedback");

    const response = await fetch(`${window.location.origin}/Contact/Submit`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Name: name,
            Email: email,
            Message: message
        })
    });

    const text = await response.text();
    if (response.ok) {
        feedback.textContent = text || "Message sent successfully!";
        feedback.className = "text-success";
        document.getElementById("contact-form").reset();
    } else {
        feedback.textContent = text || "Something went wrong!";
        feedback.className = "text-danger";
    }
});
