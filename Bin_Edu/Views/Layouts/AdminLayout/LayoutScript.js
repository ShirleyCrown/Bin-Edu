

function setActiveNavLink() {
    const navLinks = document.querySelectorAll('.nav-link');
    const currentPath = window.location.pathname;

    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
            link.classList.remove('text-dark');
        } else {
            link.classList.remove('active');
            link.classList.add('text-dark');
        }
    });
}


async function logoutAdmin() {
    try {
        const response = await axios.post('/admin/logout');

        window.location.href = response.data.data;

    } catch (error) {
        console.error('Error during logout:', error);
    }
}




// CALL FUNCTION
setActiveNavLink();