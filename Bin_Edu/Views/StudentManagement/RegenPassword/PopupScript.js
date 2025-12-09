
let regenStudentId = "";

async function initRegenPasswordModal(id) {

    regenStudentId = id;

    const regenStudentModal = new bootstrap.Modal(document.getElementById('regenStudentPasswordModal'));
    regenStudentModal.show();

}


async function regenPassword() {
    
    try {
        
        const response = await axios.post(`/admin/dashboard/student-management/regen-student-password/${regenStudentId}`)
        

        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        console.log(ex);
        
    }
}