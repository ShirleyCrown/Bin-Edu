
async function fetchStudents(page = 0) {
    const parts = window.location.pathname.split("/");
    const course_id = parts[parts.length - 1];
    const courseTableBody = document.getElementById("exerciseTableBody");

    try {
        const response = await axios.get(`/admin/dashboard/exercise-management/${course_id}/get-exercises`, {
            params: {
                "page": page
            },
        });
        const responseData = response.data.data;

        // Clear old rows before re-rendering
        courseTableBody.innerHTML = "";

        for (let i = 0; i < responseData.exercises.length; i++) {
            courseTableBody.innerHTML += `
                <tr>
                    <td>${responseData.exercises[i].name}</td>
                    <td>${responseData.exercises[i].description}</td>
                    <td>${responseData.exercises[i].submitDeadline}</td>
                    <td>${responseData.exercises[i].createdAt}</td>
                    <td>
                        <a class="btn btn-sm btn-light text-primary" href="/admin/dashboard/exercise/${responseData.exercises[i].id}/exercise-submission"><i class="bi bi-journal-check"></i></a>

                        <button class="btn btn-sm btn-light" onclick="initUpdateModal(${responseData.exercises[i].id})"><i class="bi bi-pencil"></i></button>

                        <button class="btn btn-sm btn-light text-danger" onclick="initDeleteModal('${responseData.exercises[i].id}')"><i class="bi bi-trash"></i></button>
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
    prevLi.innerHTML = `<a class="page-link" href="#">&laquo;</a>`;
    prevLi.onclick = () => {
        if (currentPage > 0) fetchStudents(0);
    };
    pagination.appendChild(prevLi);

    // ---- PAGE NUMBERS ----
    for (let i = 0; i < totalPages; i++) {
        const li = document.createElement("li");
        li.className = "page-item " + (i === currentPage ? "active" : "");
        li.innerHTML = `<a class="page-link" href="#">${i + 1}</a>`;

        li.onclick = () => fetchStudents(i);

        pagination.appendChild(li);
    }

    // ---- » NEXT BUTTON ----
    const nextLi = document.createElement("li");
    nextLi.className = "page-item " + (currentPage === totalPages ? "disabled" : "");
    nextLi.innerHTML = `<a class="page-link" href="#">&raquo;</a>`;
    nextLi.onclick = () => {
        if (currentPage < totalPages) fetchStudents(totalPages - 1);
    };
    pagination.appendChild(nextLi);
}





// CALL FUNCTIONS
fetchStudents()