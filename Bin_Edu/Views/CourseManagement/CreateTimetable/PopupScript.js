
let createTimetableCourseId = "";


function initCreateTimetableModal(id) {

    createTimetableCourseId = id;

    const createTimetableModal = new bootstrap.Modal(document.getElementById('createTimetableModal'));
    createTimetableModal.show();

}


async function createTimetable() {

    const alertBox = document.querySelector(".create-timetable-alert-danger");
    alertBox.classList.add("d-none");

    try {

        const createTimetableForm = document.getElementById("createTimetableForm");

        const formData = new FormData(createTimetableForm);

        const response = await axios.post(`/admin/dashboard/course-management/create-timetable/${createTimetableCourseId}`, formData);


        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;
        
    }
    catch (ex) {
        
        if (ex.response && ex.response.data.message) {

            const errorMessage = ex.response.data.message;

            const createTimetableErrorMessage = document.getElementById("createTimetableErrorMessage");

            createTimetableErrorMessage.innerHTML = `<strong>Validation Error:</strong> ${errorMessage}`;


            alertBox.classList.remove("d-none");

        }
        
    }

}