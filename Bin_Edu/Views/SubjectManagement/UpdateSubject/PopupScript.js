let updateSubjectId = "";

async function initUpdateModal(id) {
    const input = document.getElementsByName("UpdateSubjectName")[0];

    try {
        const response = await axios.get(`/admin/dashboard/subject-management/get-subject/${id}`);
        const data = response.data.data;

        input.value = data.subjectName;
        updateSubjectId = data.id;

        const modal = new bootstrap.Modal(document.getElementById('updateSubjectModal'));
        modal.show();
    }
    catch (ex) {
        console.log(ex);
    }
}

async function updateSubject() {
    const alertBox = document.querySelector(".update-subject-alert");
    alertBox.classList.add("d-none");

    try {
        const form = document.getElementById("updateSubjectForm");
        const formData = new FormData(form);

        const response = await axios.put(`/admin/dashboard/subject-management/update-subject/${updateSubjectId}`, formData);

        const redirectUrl = response.data.data;
        window.location.href = `/${redirectUrl}`;
    }
    catch (ex) {
        if (ex.response && ex.response.data.message) {
            const errorMessage = ex.response.data.message;
            const el = document.getElementById("updateSubjectErrorMessage");
            el.innerHTML = `<strong>Validation Error:</strong> ${errorMessage}`;
            alertBox.classList.remove("d-none");
        }
    }
}