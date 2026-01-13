

async function fetchMyCourseDetail() {

    const courseId = document.getElementsByName("CourseId")[0];

    const courseThumbnail = document.getElementById("courseThumbnail");
    const courseTitle = document.getElementById("courseTitle");
    const courseDescription = document.getElementById("courseDescription");
    const courseSubject = document.getElementById("courseSubject");
    const courseTimetable = document.getElementById("courseTimetable");
    const courseTeacherName = document.getElementById("courseTeacherName");


    try {

        const response = await axios.get(`/my-courses/get-detail/${courseId.value}`)
        
        const courseDetail = response.data.data;

        console.log(courseDetail);
        courseThumbnail.style.backgroundImage = courseDetail.thumbNail
                ? `CourseImages/${courseDetail.thumbNail}`
                : `https://placehold.co/600x400?text=No+Image+Available`;
        courseTitle.innerText = courseDetail.courseTitle;
        courseDescription.innerText = courseDetail.courseDescription;
        courseSubject.innerText = courseDetail.courseSubject;
        
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

async function registerCourse() {
    
}



// CALL FUNCTIONS
fetchMyCourseDetail();
