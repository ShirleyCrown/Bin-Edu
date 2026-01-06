
async function createExercise() {
    

    const alertBox = document.querySelector(".alert-danger");
    alertBox.classList.add("d-none");

    const parts = window.location.pathname.split("/");
    const courseId = parts[parts.length - 1];

    try {

        const createExerciseForm = document.getElementById("createExerciseForm");

        const formData = new FormData(createExerciseForm);

        console.log("Creating exercise for course ID:", courseId);
        
        const response = await axios.post(`/admin/dashboard/exercise-management/${courseId}/create-exercise`, formData)
        

        const redirectUrl = response.data.data;
        console.log("Redirecting to:", redirectUrl);
        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        if (ex.response && ex.response.data.message) {

            const errorMessage = ex.response.data.message;

            const createExerciseErrorMessage = document.getElementById("createExerciseErrorMessage");

            createExerciseErrorMessage.innerHTML = `<strong>Validation Error:</strong> ${errorMessage}`;

        
            alertBox.classList.remove("d-none");

        }
    }
}