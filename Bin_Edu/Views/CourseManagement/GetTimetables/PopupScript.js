
let courseIdTimetable = "";

async function initTimetablesModal(id) {

    courseIdTimetable = id; 


    await fetchCourseTimetables();


    const getTimetablesModal = new bootstrap.Modal(document.getElementById('getTimetablesModal'));
    getTimetablesModal.show();

}


async function fetchCourseTimetables() {

    const timetableTableBody = document.getElementById("timetableTableBody");
   
    try {

        
        const response = await axios.get(`/admin/dashboard/course-management/get-timetables/${courseIdTimetable}`)
        
        const responseData = response.data.data;

        console.log(responseData);
        

        for (let i = 0; i < responseData.length; i++) {
            timetableTableBody.innerHTML += `
                <tr>
                    <td>${responseData[i].dayOfWeek}</td>
                    <td>${responseData[i].startTime.substring(0, 5)}</td>
                    <td>${responseData[i].endTime.substring(0, 5)}</td>
                    
                </tr>
            `;
        }

    }
    catch (ex) {        
        console.log(ex);
        
    }
}