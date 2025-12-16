
let courseTimetableId = "";

async function showAttendanceQR(id) {

    courseTimetableId = id; 


    const attendanceQrCode = document.getElementById('attendanceQrCode');
    
    var qrcode = new QRCode(attendanceQrCode, {
        text: "course session id: " + courseTimetableId,
        width: 200,
        height: 200,
        colorDark : "#000000",
        colorLight : "#ffffff",
        correctLevel : QRCode.CorrectLevel.H
    });


    const showAttendanceQrModal = new bootstrap.Modal(document.getElementById('showAttendanceQrModal'));
    showAttendanceQrModal.show();

}
