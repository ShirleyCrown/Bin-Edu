
async function fetchSubmissions(page = 0) {
    const parts = window.location.pathname.split("/");
    const exercise_id = parts[parts.length - 2];
    const submissionTableBody = document.getElementById("submissionTableBody");
    try {
        const response = await axios.get(`/admin/api/exercise-submission/${exercise_id}/submissions`, {
            params: {
                "page": page
            },
        });
        const responseData = response.data.data;
        

        // Clear old rows before re-rendering
        submissionTableBody.innerHTML = "";
        for (let i = 0; i < responseData.length; i++) {
            submissionTableBody.innerHTML += `
                <tr>
                    <td>${responseData[i].studentName}</td>
                    <td>${responseData[i].submittedAt}</td>
                    <td>
                        <button class="btn btn-sm btn-light" onclick="initUpdateModal(${responseData[i].id})"><i class="bi bi-pencil-square"></i></button>

                        <button class="btn btn-sm btn-light text-primary" onclick="downloadFile('${responseData[i].id}')"><i class="bi bi-download"></i></button>
                    </td>
                </tr>
            `;
        }

        generatePagination(response.data.totalPages, page);

    } catch (ex) {
        console.log(ex);
    }
}

function downloadFile(submissionId) {
    window.location.href =
        `/admin/dashboard/exercise/api/exercise-submission/${submissionId}/download`;
}


async function initUpdateModal(submissionId) {
    const response = await axios.get(`/admin/dashboard/exercise/api/submission/${submissionId}/score`);
    const score = response.data.score;
    document.getElementById("Score").value = score;

    console.log(submissionId);
    document.getElementById("submissionId").value = submissionId;
    new bootstrap.Modal(
        document.getElementById("updateScoreModal")
    ).show();
}

async function updateScore() {
    const submissionId = document.getElementById("submissionId").value;
    
    const score = document.getElementById("Score").value;

    if(!score){
        Swal.fire("Error", "Score can't be empty !", "error");
        return;
    }

    try {
        const request = {
            "Score": score
        }

        const response = await axios.put(`/admin/dashboard/exercise/api/submission/${submissionId}/update-score`, request);
        console.log("response: ", response);
        window.location.href = `/${response.data.redirect}`;
    } catch (error) {
        console.log(error);
    }

}

function generatePagination(totalPages, currentPage) {
    try {
        const pagination = document.getElementById("pagination");
    pagination.innerHTML = "";

    // ---- « PREVIOUS BUTTON ----
    const prevLi = document.createElement("li");
    prevLi.className = "page-item " + (currentPage === 0 ? "disabled" : "");
    prevLi.innerHTML = `<a class="page-link" href="#">&laquo;</a>`;
    prevLi.onclick = () => {
        if (currentPage > 0) fetchSubmissions(0);
    };
    pagination.appendChild(prevLi);

    // ---- PAGE NUMBERS ----
    for (let i = 0; i < totalPages; i++) {
        const li = document.createElement("li");
        li.className = "page-item " + (i === currentPage ? "active" : "");
        li.innerHTML = `<a class="page-link" href="#">${i + 1}</a>`;

        li.onclick = () => fetchSubmissions(i);

        pagination.appendChild(li);
    }

    // ---- » NEXT BUTTON ----
    const nextLi = document.createElement("li");
    nextLi.className = "page-item " + (currentPage === totalPages - 1 ? "disabled" : "");
    nextLi.innerHTML = `<a class="page-link" href="#">&raquo;</a>`;
    nextLi.onclick = () => {
        if (currentPage < totalPages) fetchSubmissions(totalPages - 1);
    };
    pagination.appendChild(nextLi);
    } catch (error) {
        console.log(error);
    }
}


// CALL FUNCTIONS
fetchSubmissions()