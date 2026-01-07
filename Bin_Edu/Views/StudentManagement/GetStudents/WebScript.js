
// async function fetchStudents(page = 0) {
//     const courseTableBody = document.getElementById("studentTableBody");

//     try {
//         const response = await axios.get(`/admin/dashboard/student-management/get-students`, {
//             params: {
//                 "page": page
//             },
//         });
//         const responseData = response.data.data;

//         // Clear old rows before re-rendering
//         courseTableBody.innerHTML = "";

//         for (let i = 0; i < responseData.students.length; i++) {
//             courseTableBody.innerHTML += `
//                 <tr>
//                     <td>${responseData.students[i].fullName}</td>
//                     <td>${responseData.students[i].email}</td>
//                     <td>${responseData.students[i].grade}</td>
//                     <td>${responseData.students[i].phoneNumber}</td>
//                     <td>${responseData.students[i].school}</td>
//                     <td>${responseData.students[i].dob}</td>
//                     <td>
//                         <button class="btn btn-sm btn-light text-primary" onclick="initRegenPasswordModal('${responseData.students[i].id}')"><i class="bi bi-arrow-repeat"></i></button>
//                         <button class="btn btn-sm btn-light text-danger" onclick="initDeleteModal('${responseData.students[i].id}')"><i class="bi bi-trash"></i></button>
//                     </td>
//                 </tr>
//             `;
//         }

//         generatePagination(responseData.totalPages, page);

//     } catch (ex) {
//         console.log(ex);
//     }
// }

async function fetchStudents(page = 0) {
    const studentTableBody = document.getElementById("studentTableBody");
    const keyword = document.getElementById("studentKeyword").value;

    try {
        const response = await axios.get(
            "/admin/dashboard/student-management/get-students",
            {
                params: {
                    page: page,
                    keyword: keyword
                }
            }
        );

        const data = response.data.data;

        studentTableBody.innerHTML = "";

        data.students.forEach(s => {
            studentTableBody.innerHTML += `
                <tr>
                    <td>${s.fullName}</td>
                    <td>${s.email}</td>
                    <td>${s.grade}</td>
                    <td>${s.phoneNumber}</td>
                    <td>${s.school}</td>
                    <td>${s.dob}</td>
                    <td>
                        <button class="btn btn-sm btn-light text-primary"
                            onclick="initRegenPasswordModal('${s.id}')">
                            <i class="bi bi-arrow-repeat"></i>
                        </button>
                        <button class="btn btn-sm btn-light text-danger"
                            onclick="initDeleteModal('${s.id}')">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
        });

        generatePagination(data.totalPages, page);

    } catch (ex) {
        console.log(ex);
    }
}

document
  .getElementById("studentKeyword")
  .addEventListener("input", () => fetchStudents(0));

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