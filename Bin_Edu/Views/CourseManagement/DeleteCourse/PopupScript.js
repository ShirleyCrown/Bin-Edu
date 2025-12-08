
let deleteCourseId = "";

async function initDeleteModal(id) {

    deleteCourseId = id;

    const deleteCourseModal = new bootstrap.Modal(document.getElementById('deleteCourseModal'));
    deleteCourseModal.show();

}


async function deleteCourse() {
    
    try {
        
        const response = await axios.delete(`/admin/dashboard/course-management/delete-course/${deleteCourseId}`)
        

        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        console.log(ex);
        
    }
}