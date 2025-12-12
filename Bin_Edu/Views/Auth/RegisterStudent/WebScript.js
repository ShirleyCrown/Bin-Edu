

async function registerStudent() {

    const alertBox = document.querySelector(".alert-danger");
    alertBox.classList.add("d-none");

    try {

        const registerStudentForm = document.getElementById("registerStudentForm");

        const formData = new FormData(registerStudentForm);

        const response = await axios.post('/student/register', formData);

        const redirectUrl = response.data.data;

        console.log(response);

        window.location.href = `/${redirectUrl}`;
        

    }
    catch (ex) {

        if (ex.response && ex.response.data) {

            console.log(ex);
            

            const errorMessage = ex.response.data.message;

            const registerStudentErrorMessage = document.getElementById("registerStudentErrorMessage");

            registerStudentErrorMessage.innerHTML = `<strong>Register Error:</strong> ${errorMessage}`;


            alertBox.classList.remove("d-none");

        }
        
    }

}