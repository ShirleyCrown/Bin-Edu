
let courseAttendanceTimetableId = "";

async function initAttendancesModal(id) {

    courseAttendanceTimetableId = id; 
    await fetchAttendances();

    const getAttendancesModal = new bootstrap.Modal(document.getElementById('getAttendancesModal'));
    getAttendancesModal.show();
}


async function fetchAttendances() {

    const attendancesTableBody = document.getElementById("attendancesTableBody");

    attendancesTableBody.innerHTML = '';
   
    try {


        const response = await axios.get(`/admin/dashboard/course-management/course-sessions/${courseIdSession}/get-attendances/${courseAttendanceTimetableId}`)
        
        const responseData = response.data.data;

        

        for (let i = 0; i < responseData.length; i++) {
            attendancesTableBody.innerHTML += `
                <tr>
                    <td>${responseData[i].studentFullName}</td>
                    <td>${responseData[i].attendanceStatus ?? "N/A"}</td>
                    <td>${responseData[i].attendedAt == "01/01/0001 00:00:00" ? "N/A" : responseData[i].attendedAt}</td>
                     <td>
                        <button class="btn btn-sm btn-light text-success" onclick="markPresent('${responseData[i].studentId}')"><i class="bi bi-bookmark-check"></i></button>
                        <button class="btn btn-sm btn-light text-danger" onclick="markAbsent('${responseData[i].studentId}')"><i class="bi bi-bookmark-x"></i></button>
                    </td>
                </tr>
            `;
        }

    }
    catch (ex) {        
        console.log(ex);
        
    }
}

async function markPresent(studentId) {

    try {
        await axios.post(`/admin/dashboard/course-management/course-sessions/${courseIdSession}/mark-present/${studentId}`);
        await fetchAttendances();
    }
    catch (ex) {
        console.log(ex);
    }

}

async function markAbsent(studentId) {

    try {
        await axios.post(`/admin/dashboard/course-management/course-sessions/${courseIdSession}/mark-absent/${studentId}`);
        await fetchAttendances();
    }
    catch (ex) {
        console.log(ex);
    }
}