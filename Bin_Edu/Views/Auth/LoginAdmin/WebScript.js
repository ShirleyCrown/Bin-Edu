

async function loginAdmin() {

    const alertBox = document.querySelector(".alert-danger");
    alertBox.classList.add("d-none");

    try {

        const loginAdminForm = document.getElementById("loginAdminForm");

        const formData = new FormData(loginAdminForm);

        const response = await axios.post('/admin/login', formData);

        const redirectUrl = response.data.data;

        console.log(response);

        window.location.href = `/${redirectUrl}`;
        

    }
    catch (ex) {

        if (ex.response && ex.response.data) {

            console.log(ex);
            

            const errorMessage = ex.response.data.message;

            const loginAdminErrorMessage = document.getElementById("loginAdminErrorMessage");

            loginAdminErrorMessage.innerHTML = `<strong>Login Error:</strong> ${errorMessage}`;


            alertBox.classList.remove("d-none");

        }
        
    }

}