// ─────────────────────────────────────────
// Save login data to sessionStorage after login
// sessionStorage clears when the browser tab closes
// ─────────────────────────────────────────
function saveAuthData(data) {
    sessionStorage.setItem('token', data.token);
    sessionStorage.setItem('role', data.role);
    sessionStorage.setItem('name', data.name);
    sessionStorage.setItem('email', data.email);
}

// Remove all auth data (logout)
function clearAuthData() {
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('role');
    sessionStorage.removeItem('name');
    sessionStorage.removeItem('email');
}

// Check if user is logged in
function isLoggedIn() {
    return !!sessionStorage.getItem('token');
}

// Get current user's role
function getUserRole() {
    return sessionStorage.getItem('role');
}

// Get current user's name
function getUserName() {
    return sessionStorage.getItem('name');
}

// ─────────────────────────────────────────
// Auth guards — call these at the top of
// each protected page
// ─────────────────────────────────────────

// Redirect to login if not logged in
function requireLogin() {
    if (!isLoggedIn()) {
        window.location.href = '/car-stock-management/frontend/pages/login.html';
    }
}

// Redirect if wrong role
function requireRole(role) {
    requireLogin();
    if (getUserRole() !== role) {
        alert('You do not have permission to view this page.');
        window.location.href = '/car-stock-management/frontend/index.html';
    }
}

// Update nav bar based on login state
function updateNav() {
    const navAuth = document.getElementById('nav-auth');
    const navUser = document.getElementById('nav-user');

    if (!navAuth || !navUser) return;

    if (isLoggedIn()) {
        navAuth.style.display = 'none';
        navUser.style.display = 'flex';
        const nameEl = document.getElementById('nav-name');
        if (nameEl) nameEl.textContent = getUserName();
    } else {
        navAuth.style.display = 'flex';
        navUser.style.display = 'none';
    }
}

// Logout function
function logout() {
    clearAuthData();
    window.location.href = '/car-stock-management/frontend/index.html';
}