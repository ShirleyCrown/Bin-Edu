
async function createCourse() {
    

    const alertBox = document.querySelector("#create-alert");
    alertBox.classList.add("d-none");

    try {

        const createCourseForm = document.getElementById("createCourseForm");

        const formData = new FormData(createCourseForm);

        
        const response = await axios.post(`/admin/dashboard/course-management/create-course`, formData)
        

        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        if (ex.response && ex.response.data.message) {

            const errorMessage = ex.response.data.message;            

            const createCourseErrorMessage = document.getElementById("createCourseErrorMessage");

            createCourseErrorMessage.innerHTML = `<strong>Validation Error:</strong> ${errorMessage}`;


            alertBox.classList.remove("d-none");

        }
    }
}