

async function fetchMyCourses(page = 0) {
    const courseGrid = document.getElementById("courseGrid");

    try {
        const response = await axios.get(`/get-my-courses`, {
            params: {
                "page": page
            },
        });
        const responseData = response.data.data;

        // Clear old rows before re-rendering
        courseGrid.innerHTML = "";

        for (let i = 0; i < responseData.myCourses.length; i++) {

            const courseDayOfWeekText = responseData.myCourses[i].timetables.map(t => t.dayOfWeek).join(" & ");


            courseGrid.innerHTML += 
            `
                <div class="col-md-6 col-lg-4">
                    <div class="card shadow-sm border-0">
                        <div class="position-relative">
                            ${
                                responseData.myCourses[i].courseSubject == "Math" ? 
                                ` <img 
                                    src="https://www.shutterstock.com/shutterstock/photos/1859813464/display_1500/stock-vector-math-horizontal-banner-presentation-website-isolated-lettering-typography-idea-with-icons-1859813464.jpg"
                                    class="card-img-top" style="height: 16rem" alt="">` 
                                : 
                                responseData.myCourses[i].courseSubject == "Literature" ? 
                                ` <img 
                                    src="https://www.shutterstock.com/image-photo/image-latin-american-continent-on-260nw-2640131997.jpg"
                                    class="card-img-top" style="height: 16rem" alt="">` 
                                : 
                                ` <img 
                                    src="https://www.shutterstock.com/image-vector/english-language-learning-concept-vector-260nw-1827529367.jpg"
                                    class="card-img-top" style="height: 16rem" alt="">` 
                            }

                            <span class="badge bg-success position-absolute top-0 end-0 m-3">
                                ${responseData.myCourses[i].courseSubject}
                            </span>
                        </div>

                        <div class="card-body">
                            <h5 class="card-title fw-bold">
                                ${responseData.myCourses[i].courseTitle}
                            </h5>

                            <p class="text-muted mb-1">
                                <span class="material-symbols-outlined fs-6 align-middle">person</span>
                                ${responseData.myCourses[i].teachingTeacherName}
                            </p>

                            <p class="text-muted">
                                <span class="material-symbols-outlined fs-6 align-middle">calendar_month</span>
                                ${responseData.myCourses[i].weekDuration} Weeks • 
                                ${courseDayOfWeekText} ${responseData.myCourses[i].timetables[0].startTime} - ${responseData.myCourses[i].timetables[0].endTime}
                            </p>

                        
                            <div class="gap-2 d-flex mt-3">
                                <a href="/my-courses/${responseData.myCourses[i].id}/timetable" class="btn btn-outline-secondary w-100">
                                    <span class="material-symbols-outlined fs-6 align-middle">calendar_today</span>
                                    Timetable
                                </a>
                                <button class="btn btn-primary w-100">
                                    Details
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
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

    if (totalPages > 1) {
        // ---- « PREVIOUS BUTTON ----
        const prevLi = document.createElement("li");
        prevLi.className = "page-item " + (currentPage === 0 ? "disabled" : "");
        prevLi.innerHTML = `<a class="page-link" href="#">«</a>`;
        prevLi.onclick = () => {
            if (currentPage > 0) fetchMyCourses(0);
        };
        pagination.appendChild(prevLi);
    
        // ---- PAGE NUMBERS ----
        for (let i = 0; i < totalPages; i++) {
            const li = document.createElement("li");
            li.className = "page-item " + (i === currentPage ? "active" : "");
            li.innerHTML = `<a class="page-link" href="#">${i + 1}</a>`;
    
            li.onclick = () => fetchMyCourses(i);
    
            pagination.appendChild(li);
        }
    
        // ---- » NEXT BUTTON ----
        const nextLi = document.createElement("li");
        nextLi.className = "page-item " + (currentPage === totalPages ? "disabled" : "");
        nextLi.innerHTML = `<a class="page-link" href="#">»</a>`;
        nextLi.onclick = () => {
            if (currentPage < totalPages) fetchMyCourses(totalPages - 1);
        };
        pagination.appendChild(nextLi);
    }
}





// CALL FUNCTIONS
fetchMyCourses()