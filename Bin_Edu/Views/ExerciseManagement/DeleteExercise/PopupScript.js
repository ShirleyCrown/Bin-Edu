
let deleteExerciseId = "";

async function initDeleteModal(id) {

    deleteExerciseId = id;
    const deleteExerciseModal = new bootstrap.Modal(document.getElementById('deleteExerciseModal'));
    deleteExerciseModal.show();

}


async function deleteExercise() {
    
    try {
        
        const response = await axios.delete(`/admin/dashboard/exercise-management/delete-course-exercise/${deleteExerciseId}`)
        

        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        console.log(ex);
        
    }
}