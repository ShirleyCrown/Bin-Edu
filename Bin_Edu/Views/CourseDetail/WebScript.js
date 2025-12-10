

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

    }
    catch (ex) {
        console.log(ex);
    }

}





// CALL FUNCTIONS
fetchCourseDetail();