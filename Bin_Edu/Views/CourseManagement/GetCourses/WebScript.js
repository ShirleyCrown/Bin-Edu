
async function fetchCourses(page = 0) {
    const courseTableBody = document.getElementById("courseTableBody");

    try {
        const response = await axios.get(`/admin/dashboard/course-management/get-courses`, {
            params: {
                "page": page
            },
        });
        const responseData = response.data.data;

        console.log(response);
        

        // Clear old rows before re-rendering
        courseTableBody.innerHTML = "";

        for (let i = 0; i < responseData.courses.length; i++) {
            courseTableBody.innerHTML += `
                <tr>
                    <td>${responseData.courses[i].courseTitle}</td>
                    <td>${responseData.courses[i].courseSubject}</td>
                    <td>${responseData.courses[i].teachingTeacherName}</td>
                    <td>${responseData.courses[i].coursePrice}</td>
                    <td>${responseData.courses[i].weekDuration}</td>
                    <td>${responseData.courses[i].numberOfStudents}</td>
                    <td>
                        <button class="btn btn-sm btn-light" onclick="initUpdateModal(${responseData.courses[i].id})"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-sm btn-light text-danger" onclick="initDeleteModal(${responseData.courses[i].id})"><i class="bi bi-trash"></i></button>
                    </td>
                </tr>
            `;
        }

        generatePagination(responseData.totalPages, page);

    } catch (ex) {
        console.log(ex);
    }
}


function generatePagination(totalPages, currentPage) {
    const pagination = document.getElementById("pagination");
    pagination.innerHTML = "";

    // ---- « PREVIOUS BUTTON ----
    const prevLi = document.createElement("li");
    prevLi.className = "page-item " + (currentPage === 0 ? "disabled" : "");
    prevLi.innerHTML = `<a class="page-link" href="#">«</a>`;
    prevLi.onclick = () => {
        if (currentPage > 0) fetchCourses(0);
    };
    pagination.appendChild(prevLi);

    // ---- PAGE NUMBERS ----
    for (let i = 0; i < totalPages; i++) {
        const li = document.createElement("li");
        li.className = "page-item " + (i === currentPage ? "active" : "");
        li.innerHTML = `<a class="page-link" href="#">${i + 1}</a>`;

        li.onclick = () => fetchCourses(i);

        pagination.appendChild(li);
    }

    // ---- » NEXT BUTTON ----
    const nextLi = document.createElement("li");
    nextLi.className = "page-item " + (currentPage === totalPages ? "disabled" : "");
    nextLi.innerHTML = `<a class="page-link" href="#">»</a>`;
    nextLi.onclick = () => {
        if (currentPage < totalPages) fetchCourses(totalPages - 1);
    };
    pagination.appendChild(nextLi);
}





// CALL FUNCTIONS
fetchCourses()