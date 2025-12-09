
let deleteStudentId = "";

async function initDeleteModal(id) {

    deleteStudentId = id;

    const deleteStudentModal = new bootstrap.Modal(document.getElementById('deleteStudentModal'));
    deleteStudentModal.show();

}


async function deleteStudent() {
    
    try {
        
        const response = await axios.delete(`/admin/dashboard/student-management/delete-student/${deleteStudentId}`)
        

        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        console.log(ex);
        
    }
}