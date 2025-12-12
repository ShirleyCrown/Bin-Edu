async function logoutStudent() {
    try {
        const response = await axios.post('/student/logout');

        window.location.href = response.data.data;

    } catch (error) {
        console.error('Error during logout:', error);
    }
}


async function fetchCourses(page = 0) {
    const courseGrid = document.getElementById("courseGrid");

    try {
        const response = await axios.get(`/get-courses`, {
            params: {
                "page": page
            },
        });
        const responseData = response.data.data;

        // Clear old rows before re-rendering
        courseGrid.innerHTML = "";

        for (let i = 0; i < responseData.courses.length; i++) {
            courseGrid.innerHTML += `
                <div class="col-md-6 col-lg-4">
                    <div class="card course-card shadow-sm h-100"> 
                        ${
                            responseData.courses[i].courseSubject == "Math" ? 
                            ` <img 
                                src="https://www.shutterstock.com/shutterstock/photos/1859813464/display_1500/stock-vector-math-horizontal-banner-presentation-website-isolated-lettering-typography-idea-with-icons-1859813464.jpg"
                                class="card-img-top" alt="">` 
                            : 
                            responseData.courses[i].courseSubject == "Literature" ? 
                            ` <img 
                                src="https://www.shutterstock.com/image-photo/image-latin-american-continent-on-260nw-2640131997.jpg"
                                class="card-img-top" alt="">` 
                            : 
                            ` <img 
                                src="https://www.shutterstock.com/image-vector/english-language-learning-concept-vector-260nw-1827529367.jpg"
                                class="card-img-top" alt="">` 
                        }
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title">${responseData.courses[i].courseTitle}</h5>
                            <p class="card-text small text-muted">${responseData.courses[i].courseDescription}</p>
                            <p class="small text-muted" style="display: flex; align-items: center;"> 
                                <span class="material-symbols-outlined mx-2"> book_4 </span> ${responseData.courses[i].courseSubject} 
                                | 
                                <span class="material-symbols-outlined mx-2"> person </span> <span>${responseData.courses[i].teachingTeacherName} 
                            </p>
                            <p class="small text-muted" style="display: flex; align-items: center;"> 
                                <span class="material-symbols-outlined mx-2"> clock_loader_10 </span> ${responseData.courses[i].weekDuration} 
                                |
                                <span class="material-symbols-outlined mx-2"> price_change </span> ${responseData.courses[i].coursePrice} VND
                            </p>
                            <a href="/course-detail/${responseData.courses[i].id}" class="btn btn-outline-primary mt-auto">View Details</a>
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