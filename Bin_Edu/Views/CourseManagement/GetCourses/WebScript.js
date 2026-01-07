
// async function fetchCourses(page = 0) {
//     const courseTableBody = document.getElementById("courseTableBody");

//     try {
//         const response = await axios.get(`/admin/dashboard/course-management/get-courses`, {
//             params: {
//                 "page": page
//             },
//         });
//         const responseData = response.data.data;

//         console.log(response);
        

//         // Clear old rows before re-rendering
//         courseTableBody.innerHTML = "";

//         for (let i = 0; i < responseData.courses.length; i++) {
//             courseTableBody.innerHTML += `
//                 <tr>
//                     <td>${c.courseTitle}</td>
//                     <td>${c.courseSubject}</td>
//                     <td>${c.teachingTeacherName}</td>
//                     <td>${c.coursePrice}</td>
//                     <td>${c.weekDuration}</td>
//                     <td>${c.numberOfStudents}</td>
//                     <td>
//                         <button class="btn btn-sm btn-light text-warning" onclick="initCourseSessionsModal(${c.id})"><i class="bi bi-calendar-check"></i></button>
//                         <button class="btn btn-sm btn-light text-primary" onclick="initTimetablesModal(${c.id})"><i class="bi bi-calendar-range"></i></button>
//                         <button class="btn btn-sm btn-light text-primary" onclick="initCreateTimetableModal(${c.id})"><i class="bi bi-calendar-plus"></i></button>
//                         <a class="btn btn-sm btn-light text-primary" href="/admin/dashboard/exercise-management/${c.id}"><i class="bi bi-book"></i></a>
//                         <button class="btn btn-sm btn-light" onclick="initUpdateModal(${c.id})"><i class="bi bi-pencil"></i></button>
//                         <button class="btn btn-sm btn-light text-danger" onclick="initDeleteModal(${c.id})"><i class="bi bi-trash"></i></button>
//                     </td>
//                 </tr>
//             `;
//         }

//         generatePagination(responseData.totalPages, page);

//     } catch (ex) {
//         console.log(ex);
//     }
// }

async function fetchCourses(page = 0) {
    const keyword = document.getElementById("keyword").value;
    const subject = document.getElementById("subject").value;
    const minPrice = document.getElementById("minPrice").value;
    const maxPrice = document.getElementById("maxPrice").value;

    try {
        const response = await axios.get(
            "/admin/dashboard/course-management/get-courses",
            {
                params: {
                    page,
                    keyword,
                    subject,
                    minPrice,
                    maxPrice
                }
            }
        );

        const data = response.data.data;
        const courses = data.courses;

        courseTableBody.innerHTML = "";

        courses.forEach(c => {
            courseTableBody.innerHTML += `
                <tr>
                    <td>${c.courseTitle}</td>
                    <td>${c.courseSubject}</td>
                    <td>${c.teachingTeacherName}</td>
                    <td>${c.coursePrice}</td>
                    <td>${c.weekDuration}</td>
                    <td>${c.numberOfStudents}</td>
                    <td>
                        <a class="btn btn-sm btn-light text-warning" title="View Course Sessions" onclick="initCourseSessionsModal(${c.id})"><i class="bi bi-calendar-check"></i></a>
                        <a class="btn btn-sm btn-light text-primary" title="View Course Timetable" onclick="initTimetablesModal(${c.id})"><i class="bi bi-calendar-range"></i></a>
                        <a class="btn btn-sm btn-light text-primary" title="Create Timetable" onclick="initCreateTimetableModal(${c.id})"><i class="bi bi-calendar-plus"></i></a>
                        <a class="btn btn-sm btn-light text-primary" title="View Exercises" href="/admin/dashboard/exercise-management/${c.id}"><i class="bi bi-book"></i></a>
                        <a class="btn btn-sm btn-light" title="Update Course" onclick="initUpdateModal(${c.id})"><i class="bi bi-pencil"></i></a>
                        <a class="btn btn-sm btn-light text-danger" title="Delete Course" onclick="initDeleteModal(${c.id})"><i class="bi bi-trash"></i></a>
                    </td>
                </tr>
            `;
        });

        generatePagination(data.totalPages, page);

    } catch (err) {
        console.log(err);
    }
}


function generatePagination(totalPages, currentPage) {
    const pagination = document.getElementById("pagination");
    pagination.innerHTML = "";

    if (totalPages > 1) {
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
}





// CALL FUNCTIONS
fetchCourses()