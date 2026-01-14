
async function fetchSubjects() {
    const tbody = document.getElementById("subjectTableBody");

    try {
        const res = await axios.get(
            "/admin/dashboard/subject-management/get-subjects"
        );

        const subjects = res.data.data;
        tbody.innerHTML = "";

        subjects.forEach(s => {
            tbody.innerHTML += `
                <tr>
                    <td>${s.subjectName}</td>
                    <td>
                       
                        <a class="btn btn-sm btn-light text-primary" title="Update" onclick="initUpdateModal(${s.id})">
                            <i class="bi bi-pencil"></i>
                        </a>
                        <a class="btn btn-sm btn-light text-danger" title="Delete" onclick="initDeleteModal(${s.id})">
                            <i class="bi bi-trash"></i>
                        </a>
                    </td>
                </tr>
            `;
        });
    } catch (err) {
        console.error(err);
    }
}

fetchSubjects();
