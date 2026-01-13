let deleteSubjectId = "";

async function initDeleteModal(id) {
    deleteSubjectId = id;
    const modal = new bootstrap.Modal(document.getElementById('deleteSubjectModal'));
    modal.show();
}

async function deleteSubject() {
    try {
        const response = await axios.delete(`/admin/dashboard/subject-management/delete-subject/${deleteSubjectId}`);
        const redirectUrl = response.data.data;
        window.location.href = `/${redirectUrl}`;
    }
    catch (ex) {
        if (ex.response && ex.response.data.message) {
            console.log('Delete error:', ex.response.data.message);
        } else {
            console.log(ex);
        }
    }
}