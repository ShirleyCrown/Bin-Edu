

function initRelatedCourseSection() {
    new Splide( '.splide', {
        type    : 'loop',
        autoplay: true,
        interval: 4000,
        perPage: 2,
        fixedHeight: "35rem",
        gap: "5rem",
        arrows: false,
        pagination: false
    }).mount();
}

async function fetchCourseDetail() {

    const courseId = document.getElementsByName("CourseId")[0];

    const courseThumbnail = document.getElementById("courseThumbnail");
    const courseTitle = document.getElementById("courseTitle");
    const courseDescription = document.getElementById("courseDescription");
    const coursePrice = document.getElementById("coursePrice");
    const courseSubject = document.getElementById("courseSubject");
    const courseDurationWeek = document.getElementById("courseDurationWeek");
    const courseTimetable = document.getElementById("courseTimetable");
    const courseTeacherName = document.getElementById("courseTeacherName");


    try {

        const response = await axios.get(`/get-course-detail/${courseId.value}`)
        
        const courseDetail = response.data.data.courseDetail;
        const relatedCourses =  response.data.data.relatedCourses;

        courseThumbnail.style.backgroundImage = 
            courseDetail.courseSubject == "Math" ? 
            "url('https://www.shutterstock.com/shutterstock/photos/1859813464/display_1500/stock-vector-math-horizontal-banner-presentation-website-isolated-lettering-typography-idea-with-icons-1859813464.jpg')" 
            : 
            courseDetail.courseSubject == "Literature" ? 
            "url('https://www.shutterstock.com/image-photo/image-latin-american-continent-on-260nw-2640131997.jpg')" 
            : 
            "url('https://www.shutterstock.com/image-vector/english-language-learning-concept-vector-260nw-1827529367.jpg')";
        courseTitle.innerText = courseDetail.courseTitle;
        courseDescription.innerText = courseDetail.courseDescription;
        coursePrice.innerText = `${courseDetail.coursePrice} VND`;
        courseSubject.innerText = courseDetail.courseSubject;
        courseDurationWeek.innerText = `${courseDetail.weekDuration} weeks`;
        
        const courseDayOfWeekText = courseDetail.timetables.map(t => t.dayOfWeek).join(" & ");
        const courseStartTimeText = courseDetail.timetables[0].startTime.substring(0, 5);
        const courseEndTimetext = courseDetail.timetables[0].endTime.substring(0, 5);

        courseTimetable.innerText = `${courseDayOfWeekText}, ${courseStartTimeText} - ${courseEndTimetext}`;
        courseTeacherName.innerText = courseDetail.teachingTeacherName;


        const relatedCoursesSlider = document.getElementById("relatedCoursesSlider");
        

        for (let i = 0; i < relatedCourses.length; i++) {
            relatedCoursesSlider.innerHTML += `
                <li class="splide__slide">
                    <div class="card course-card shadow-sm h-100"> 
                        ${
                            relatedCourses[i].courseSubject == "Math" ? 
                            ` <img 
                                src="https://www.shutterstock.com/shutterstock/photos/1859813464/display_1500/stock-vector-math-horizontal-banner-presentation-website-isolated-lettering-typography-idea-with-icons-1859813464.jpg"
                                class="card-img-top" alt="">` 
                            : 
                            relatedCourses[i].courseSubject == "Literature" ? 
                            ` <img 
                                src="https://www.shutterstock.com/image-photo/image-latin-american-continent-on-260nw-2640131997.jpg"
                                class="card-img-top" alt="">` 
                            : 
                            ` <img 
                                src="https://www.shutterstock.com/image-vector/english-language-learning-concept-vector-260nw-1827529367.jpg"
                                class="card-img-top" alt="">` 
                        }
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title">${relatedCourses[i].courseTitle}</h5>
                            <p class="card-text small text-muted">${relatedCourses[i].courseDescription}</p>
                            <p class="small text-muted" style="display: flex; align-items: center;"> 
                                <span class="material-symbols-outlined mx-2"> book_4 </span> ${relatedCourses[i].courseSubject} 
                                | 
                                <span class="material-symbols-outlined mx-2"> person </span> <span>${relatedCourses[i].teachingTeacherName} 
                            </p>
                            <p class="small text-muted" style="display: flex; align-items: center;"> 
                                <span class="material-symbols-outlined mx-2"> clock_loader_10 </span> ${relatedCourses[i].weekDuration} 
                                |
                                <span class="material-symbols-outlined mx-2"> price_change </span> ${relatedCourses[i].coursePrice} VND
                            </p>
                            <a href="/course-detail/${relatedCourses[i].id}" class="btn btn-outline-primary mt-auto">View Details</a>
                        </div>
                    </div>
                </li>
            `;
        }

        initRelatedCourseSection();

    }
    catch (ex) {
        console.log(ex);
    }

}





// CALL FUNCTIONS
fetchCourseDetail();
