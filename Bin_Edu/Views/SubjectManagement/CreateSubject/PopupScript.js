async function createSubject() {
    const alertBox = document.querySelector("#create-subject-alert");
    alertBox.classList.add("d-none");

    try {
        const createForm = document.getElementById("createSubjectForm");
        const formData = new FormData(createForm);

        const response = await axios.post(`/admin/dashboard/subject-management/create-subject`, formData);

        const redirectUrl = response.data.data;
        window.location.href = `/${redirectUrl}`;
    }
    catch (ex) {
        if (ex.response && ex.response.data.message) {
            const errorMessage = ex.response.data.message;
            const createSubjectErrorMessage = document.getElementById("createSubjectErrorMessage");

            createSubjectErrorMessage.innerHTML = `<strong>Validation Error:</strong> ${errorMessage}`;
            alertBox.classList.remove("d-none");
        }
    }
}

function openCreateSubjectModal() {
    var modal = new bootstrap.Modal(document.getElementById('createSubjectModal'));
    modal.show();
}