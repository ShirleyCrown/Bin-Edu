

async function fetchSubjects() {

    const courseSubjectSelect = document.getElementById("CourseSubject");

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

async function createCourse() {
    

    const alertBox = document.querySelector("#create-alert");
    alertBox.classList.add("d-none");

    try {

        const createCourseForm = document.getElementById("createCourseForm");

        const formData = new FormData(createCourseForm);

        
        const response = await axios.post(`/admin/dashboard/course-management/create-course`, formData)
        

        const redirectUrl = response.data.data;

        window.location.href = `/${redirectUrl}`;

    }
    catch (ex) {        
        
        if (ex.response && ex.response.data.message) {

            const errorMessage = ex.response.data.message;            

            const createCourseErrorMessage = document.getElementById("createCourseErrorMessage");

            createCourseErrorMessage.innerHTML = `<strong>Validation Error:</strong> ${errorMessage}`;


            alertBox.classList.remove("d-none");

        }
    }
}



// CALL FUNCTIONS
fetchSubjects();