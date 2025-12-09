
async function createStudent() {
    

    const alertBox = document.querySelector(".alert-danger");
    alertBox.classList.add("d-none");

    try {

        const createStudentForm = document.getElementById("createStudentForm");

        const formData = new FormData(createStudentForm);

        console.log(...formData.entries());
        
        const response = await axios.post(`/admin/dashboard/student-management/create-student`, formData)
        

        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        if (ex.response && ex.response.data.message) {

            const errorMessage = ex.response.data.message;

            const createStudentErrorMessage = document.getElementById("createStudentErrorMessage");

            createStudentErrorMessage.innerHTML = `<strong>Validation Error:</strong> ${errorMessage}`;

            


            alertBox.classList.remove("d-none");

        }
    }
}