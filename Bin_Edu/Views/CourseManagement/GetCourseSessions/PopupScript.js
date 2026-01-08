
let courseIdSession = "";

async function initCourseSessionsModal(id) {

    courseIdSession = id; 


    await fetchCourseSessions();


    const getCourseSessionsModal = new bootstrap.Modal(document.getElementById('getCourseSessionsModal'));
    getCourseSessionsModal.show();

}


async function fetchCourseSessions() {

    const courseSessionTableBody = document.getElementById("courseSessionTableBody");

    courseSessionTableBody.innerHTML = '';
   
    try {

        
        const response = await axios.get(`/admin/dashboard/course-management/get-course-sessions/${courseIdSession}`)
        
        const responseData = response.data.data;

        

        for (let i = 0; i < responseData.length; i++) {
            courseSessionTableBody.innerHTML += `
                <tr>
                    <td>${responseData[i].dayOfWeek}</td>
                    <td>${responseData[i].startDate}</td>
                    <td>${responseData[i].startTime.substring(0, 5)}</td>
                    <td>${responseData[i].endTime.substring(0, 5)}</td>
                    <td>${responseData[i].totalPresent}</td>
                    <td>${responseData[i].totalAbsent}</td>
                     <td>
                        <button class="btn btn-sm btn-light text-warning" onclick="showAttendanceQR(${responseData[i].id})"><i class="bi bi-qr-code"></i></button>
                        <button class="btn btn-sm btn-light text-primary" onclick="initAttendancesModal(${responseData[i].id})"><i class="bi bi-people-fill"></i></button>
                    </td>
                </tr>
            `;
        }

    }
    catch (ex) {        
        console.log(ex);
        
    }
}