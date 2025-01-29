document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("form1");
    const username = document.getElementById("username");
    const email = document.getElementById("email");
    const password = document.getElementById("password");
    const confirmPassword = document.getElementById("confirmPassword");
    const dateOfBirth = document.getElementById("dateOfBirth");
    const gender = document.getElementsByName("Gender");
    const courseStreamSelect = document.getElementById("courseStream");
    const profilePicture = document.getElementById("profilePicture");

    const validateUsername = () => {
        if (username.value.trim() === "") {
            username.setCustomValidity("Username is required.");
        } else if (username.value.length > 50) {
            username.setCustomValidity("Username cannot exceed 50 characters.");
        } else {
            username.setCustomValidity("");
        }
    };

    const validateEmail = () => {
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (email.value.trim() === "") {
            email.setCustomValidity("Email is required.");
        } else if (!emailPattern.test(email.value)) {
            email.setCustomValidity("Please enter a valid email address.");
        } else {
            email.setCustomValidity("");
        }
    };

    const validatePassword = () => {
        if (password.value.trim() === "") {
            password.setCustomValidity("Password is required.");
        } else if (password.value.length < 6 || password.value.length > 100) {
            password.setCustomValidity("Password must be between 6 and 100 characters.");
        } else {
            password.setCustomValidity("");
        }
    };

    const validateConfirmPassword = () => {
        if (confirmPassword.value.trim() === "") {
            confirmPassword.setCustomValidity("Confirmation of password is required.");
        } else if (confirmPassword.value !== password.value) {
            confirmPassword.setCustomValidity("The password and confirmation password do not match.");
        } else {
            confirmPassword.setCustomValidity("");
        }
    };

    const validateDateOfBirth = () => {
        if (dateOfBirth.value.trim() === "") {
            dateOfBirth.setCustomValidity("Date of birth is required.");
        } else {
            dateOfBirth.setCustomValidity("");
        }
    };

    const validateGender = () => {
        let isGenderSelected = false;
        for (let i = 0; i < gender.length; i++) {
            if (gender[i].checked) {
                isGenderSelected = true;
                break;
            }
        }
        if (!isGenderSelected) {
            gender[0].setCustomValidity("Gender is required.");
        } else {
            gender[0].setCustomValidity("");
        }
    };

    const validateCourseStream = () => {
        if (courseStreamSelect.value === "") {
            courseStreamSelect.setCustomValidity("Course stream is required.");
        } else {
            courseStreamSelect.setCustomValidity("");
        }
    };

    form.addEventListener("submit", function (event) {
        validateUsername();
        validateEmail();
        validatePassword();
        validateConfirmPassword();
        validateDateOfBirth();
        validateGender();
        validateCourseStream();

        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        } else {
            courseStreamSelect.disabled = true; 
        }

        form.classList.add('was-validated');
    }, false);

    username.addEventListener("input", validateUsername);
    email.addEventListener("input", validateEmail);
    password.addEventListener("input", validatePassword);
    confirmPassword.addEventListener("input", validateConfirmPassword);
    dateOfBirth.addEventListener("input", validateDateOfBirth);
    for (let i = 0; i < gender.length; i++) {
        gender[i].addEventListener("change", validateGender);
    }
    courseStreamSelect.addEventListener("change", validateCourseStream);
});

function previewImage(event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('profilePicturePreview');
        output.src = reader.result;
    };
    reader.readAsDataURL(event.target.files[0]);
}
