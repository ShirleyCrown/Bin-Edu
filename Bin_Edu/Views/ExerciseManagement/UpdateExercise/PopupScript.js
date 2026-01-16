
let updateExerciseId = "";

async function initUpdateModal(id) {

    const exerciseNameInput = document.getElementsByName("UpdateExerciseName")[0];
    const exerciseDescriptionInput = document.getElementsByName("UpdateExerciseDescription")[0];
    const exerciseSubmitDeadlineInput = document.getElementsByName("UpdateExerciseSubmitDeadline")[0];


    try {
        
        const response = await axios.get(`/admin/dashboard/exercise-management/get-course-exercise/${id}`)
        const responseData = response.data.data;
        

        exerciseNameInput.value = responseData.exerciseName;
        exerciseDescriptionInput.value = responseData.exerciseDescription;
        exerciseSubmitDeadlineInput.value = responseData.exerciseSubmitDeadline;

        updateExerciseId = responseData.id;        


        const updateExerciseModal = new bootstrap.Modal(document.getElementById('updateExerciseModal'));
        updateExerciseModal.show();
    }
    catch (ex) {
        console.log(ex);
        
    }

}


async function updateExercise() {
    

    const alertBox = document.querySelector(".update-alert-danger");
    alertBox.classList.add("d-none");

    try {
        const updateCourseForm = document.getElementById("updateExerciseForm");

        const formData = new FormData(updateCourseForm);

        console.log("Form Data:", ...formData);
        const response = await axios.put(`/admin/dashboard/exercise-management/update-course-exercise/${updateExerciseId}`, formData)

        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        if (ex.response && ex.response.data.message) {

            const errorMessage = ex.response.data.message;

            const updateExerciseErrorMessage = document.getElementById("updateExerciseErrorMessage");

            updateExerciseErrorMessage.innerHTML = `<strong>Validation Error:</strong> ${errorMessage}`;

            
            alertBox.classList.remove("d-none");

        }
    }
}