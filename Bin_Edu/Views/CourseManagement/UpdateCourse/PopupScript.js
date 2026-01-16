
let updateCourseId = "";


async function fetchSubjects() {

    const courseSubjectSelect = document.getElementsByName("UpdateCourseSubject")[0];

    try {

        const response = await axios.get(`/subjects`);

        const subjects = response.data;
        

        courseSubjectSelect.innerHTML = `<option value="" selected>Select subject</option>`;
        subjects.forEach(subject => {
            const option = document.createElement("option");
            option.value = subject.id;
            option.text = subject.subjectName;
            courseSubjectSelect.appendChild(option);
        })

    }
    catch (ex) {
        console.log(ex);
    }
}

async function initUpdateModal(id) {

    const courseTitleInput = document.getElementsByName("UpdateCourseTitle")[0];
    const courseDescriptionInput = document.getElementsByName("UpdateCourseDescription")[0];
    const courseSubjectInput = document.getElementsByName("UpdateCourseSubject")[0];
    const coursePriceInput = document.getElementsByName("UpdateCoursePrice")[0];
    const teachingTeacherNameInput = document.getElementsByName("UpdateTeachingTeacherName")[0];
    const openingDateInput = document.getElementsByName("UpdateOpeningDate")[0];
    const endDateInput = document.getElementsByName("UpdateEndDate")[0];
    const thumbnailPreview = document.getElementById("updateThumbnailPreview");


    try {
        
        const response = await axios.get(`/admin/dashboard/course-management/get-course/${id}`)
        const responseData = response.data.data;
        

        courseTitleInput.value = responseData.courseTitle;
        courseDescriptionInput.value = responseData.courseDescription;
        courseSubjectInput.value = responseData.courseSubjectId;
        coursePriceInput.value = responseData.coursePrice;
        teachingTeacherNameInput.value = responseData.teachingTeacherName;
        openingDateInput.value = responseData.openingDate;
        endDateInput.value = responseData.endDate;
        if (responseData.thumbNail) {
            thumbnailPreview.src = `/CourseImages/${responseData.thumbNail}`;
        } else {
            thumbnailPreview.src = `https://placehold.co/600x400?text=No+Image`;
        }

        updateCourseId = responseData.id;        


        const updateCourseModal = new bootstrap.Modal(document.getElementById('updateCourseModal'));
        updateCourseModal.show();

    }
    catch (ex) {
        console.log(ex);
        
    }

}

function previewUpdateThumbnail(input) {
    const preview = document.getElementById("updateThumbnailPreview");

    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = e => preview.src = e.target.result;
        reader.readAsDataURL(input.files[0]);
    }
}


async function updateCourse() {
    

    const alertBox = document.querySelector(".update-alert-danger");
    alertBox.classList.add("d-none");

    try {

        const updateCourseForm = document.getElementById("updateCourseForm");

        const formData = new FormData(updateCourseForm);

        
        const response = await axios.put(`/admin/dashboard/course-management/update-course/${updateCourseId}`, formData)
        

        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        if (ex.response && ex.response.data.message) {

            const errorMessage = ex.response.data.message;

            const createCourseErrorMessage = document.getElementById("updateCourseErrorMessage");

            createCourseErrorMessage.innerHTML = `<strong>Validation Error:</strong> ${errorMessage}`;

            
            alertBox.classList.remove("d-none");

        }
    }
}



// CALL FUNCTIONS
fetchSubjects();