let chart;

async function loadDashboard(year) {
    const res = await axios.get("/admin/api/dashboard/stats", {
        params: { year }
    });

    const data = res.data.data;

    document.getElementById("totalCourses").innerText = data.totalCourses;
    document.getElementById("totalStudents").innerText = data.totalStudents;

    const labels = data.monthlyRevenue.map(m => `Month ${m.month}`);
    const values = data.monthlyRevenue.map(m => m.revenue);

    if (chart) chart.destroy();

    chart = new Chart(document.getElementById("revenueChart"), {
        type: "bar",
        data: {
            labels,
            datasets: [{
                label: "Revenue",
                data: values,
                backgroundColor: "#2b8cee"
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false }
            }
        }
    });
}

// Year selector
function initYearSelect() {
    const select = document.getElementById("yearSelect");
    const currentYear = new Date().getFullYear();

    for (let y = 2024; y <= currentYear; y++) {
        const opt = document.createElement("option");
        opt.value = y;
        opt.innerText = y;
        select.appendChild(opt);
    }

    select.value = currentYear;
    select.onchange = () => loadDashboard(select.value);
}

// Init
initYearSelect();
loadDashboard(new Date().getFullYear());
