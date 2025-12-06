

async function loginAdmin() {

    try {

        const loginAdminForm = document.getElementById("loginAdminForm");

        const formData = new FormData(loginAdminForm);

        const response = await axios.post('/admin/login', formData);

        const redirectUrl = response.data.data;

        console.log(response);

        window.location.href = `/${redirectUrl}`;
        

    }
    catch (ex) {
        console.log(ex);
        
    }

}