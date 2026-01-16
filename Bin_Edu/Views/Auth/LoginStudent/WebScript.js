

async function loginStudent() {

    const alertBox = document.querySelector(".alert-danger");
    alertBox.classList.add("d-none");

    try {

        const loginStudentForm = document.getElementById("loginStudentForm");

        const formData = new FormData(loginStudentForm);

        const response = await axios.post('/student/login', formData);

        const redirectUrl = response.data.data;

        console.log(response);

        window.location.href = `/${redirectUrl}`;
        

    }
    catch (ex) {

        if (ex.response && ex.response.data) {

            console.log(ex);
            

            const errorMessage = ex.response.data.message;

            const loginStudentErrorMessage = document.getElementById("loginStudentErrorMessage");

            loginStudentErrorMessage.innerHTML = `<strong>Login Error:</strong> ${errorMessage}`;


            alertBox.classList.remove("d-none");

        }
        
    }

}