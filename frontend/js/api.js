const API_BASE = 'https://localhost:7195/api';

// ─────────────────────────────────────────
// Core fetch wrapper
// Every API call goes through this function
// It automatically adds the JWT token to the header
// ─────────────────────────────────────────
async function apiFetch(endpoint, options = {}) {
    const token = sessionStorage.getItem('token');

    const headers = {
        'Content-Type': 'application/json',
        ...options.headers
    };

    // If we have a token, add it to every request automatically
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(`${API_BASE}${endpoint}`, {
        ...options,
        headers
    });

    return response;
}

// ─────────────────────────────────────────
// Auth endpoints
// ─────────────────────────────────────────
async function apiRegister(name, email, password) {
    return apiFetch('/auth/register', {
        method: 'POST',
        body: JSON.stringify({ name, email, password })
    });
}

async function apiLogin(email, password) {
    return apiFetch('/auth/login', {
        method: 'POST',
        body: JSON.stringify({ email, password })
    });
}

// ─────────────────────────────────────────
// Cars endpoints
// ─────────────────────────────────────────
async function apiGetCars(params = {}) {
    // Build query string from params object
    // e.g. { make: 'Toyota', maxPrice: 30000 } → ?make=Toyota&maxPrice=30000
    const query = new URLSearchParams(params).toString();
    return apiFetch(`/cars${query ? '?' + query : ''}`);
}

async function apiGetCarById(id) {
    return apiFetch(`/cars/${id}`);
}

async function apiCreateCar(carData) {
    return apiFetch('/cars', {
        method: 'POST',
        body: JSON.stringify(carData)
    });
}

async function apiUpdateCar(id, carData) {
    return apiFetch(`/cars/${id}`, {
        method: 'PUT',
        body: JSON.stringify(carData)
    });
}

async function apiDeleteCar(id) {
    return apiFetch(`/cars/${id}`, {
        method: 'DELETE'
    });
}

// ─────────────────────────────────────────
// Bookings endpoints
// ─────────────────────────────────────────
async function apiCreateBooking(carId, bookingDate, notes) {
    return apiFetch('/bookings', {
        method: 'POST',
        body: JSON.stringify({ carId, bookingDate, notes })
    });
}

async function apiGetMyBookings() {
    return apiFetch('/bookings/my');
}

async function apiGetDealerBookings() {
    return apiFetch('/bookings/dealer');
}

async function apiGetAllBookings() {
    return apiFetch('/bookings');
}

async function apiUpdateBookingStatus(id, status) {
    return apiFetch(`/bookings/${id}/status`, {
        method: 'PATCH',
        body: JSON.stringify({ status })
    });
}

// ─────────────────────────────────────────
// Admin endpoints
// ─────────────────────────────────────────
async function apiGetAllUsers() {
    return apiFetch('/admin/users');
}