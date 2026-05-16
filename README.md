# 🚗 CarStock Management System

A graduate-level full-stack web application for managing car dealership stock, built with **ASP.NET Core 8 Web API**, **PostgreSQL**, and **vanilla JavaScript**.

![CarStock Screenshot](https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=1200)

---

## 🌐 Live Demo
- **Frontend:** https://cjmt22.github.io/car-stock-management/frontend/index.html
- **API (Swagger):** https://carstock-api.onrender.com
- **GitHub:** https://github.com/cjmt22/car-stock-management

> Note: The API is hosted on Render's free tier and may take 30 seconds
> to wake up on first request after a period of inactivity.

## 📋 Table of Contents
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [User Roles](#user-roles)
- [Screenshots](#screenshots)
- [Author](#author)

---

## ✨ Features

### For Buyers
- Browse and search car listings with filters (make, model, price, year, colour)
- View detailed car information with photos
- Book test drives with preferred date and time
- Track booking status (Pending → Approved/Rejected)
- Cancel pending bookings

### For Dealers
- Create, update, and delete car listings
- Upload car image URLs
- View and manage test drive booking requests
- Approve or reject bookings from buyers
- Mark cars as Sold

### For Admins
- Full platform visibility — all users, all cars, all bookings
- Change user roles (promote Buyer to Dealer)
- Full audit log of all system actions
- Platform-wide analytics

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Backend | C# / ASP.NET Core 8 Web API |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Authentication | JWT (JSON Web Tokens) |
| Authorisation | Role-based (Admin, Dealer, Buyer) |
| Frontend | HTML5 / CSS3 / Vanilla JavaScript |
| API Docs | Swagger / OpenAPI |
| Testing | xUnit + Moq + FluentAssertions |
| Version Control | Git / GitHub |

---

## 🏗️ Architecture

```
CarStockManagement/
├── backend/
│   ├── CarStock.API/
│   │   ├── Controllers/     # HTTP endpoints
│   │   ├── Services/        # Business logic
│   │   ├── Models/          # Database entities
│   │   ├── DTOs/            # Data transfer objects
│   │   ├── Data/            # DbContext + migrations
│   │   └── Program.cs       # App configuration
│   └── CarStock.Tests/      # xUnit test project
└── frontend/
    ├── css/styles.css
    ├── js/api.js            # API fetch wrappers
    ├── js/auth.js           # Auth + token management
    ├── index.html           # Home page
    └── pages/               # All other pages
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 16](https://www.postgresql.org/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community)

### 1. Clone the repository
```bash
git clone https://github.com/cjmt22/car-stock-management.git
cd car-stock-management
```

### 2. Set up the database
Create a PostgreSQL database called `CarStockDb`, then update the connection string in `backend/CarStock.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=CarStockDb;Username=postgres;Password=YOUR_PASSWORD"
}
```

### 3. Run migrations
```bash
cd backend/CarStock.API
dotnet ef database update
```

### 4. Run the API
Open `CarStock.sln` in Visual Studio 2022 and press **F5**, or:
```bash
dotnet run --project backend/CarStock.API
```

### 5. Open the frontend
Open `frontend/index.html` in your browser.

### 6. Default test accounts
| Role | Email | Password |
|---|---|---|
| Admin | admin@carstock.com | (set manually in DB) |
| Dealer | dealer@example.com | password123 |
| Buyer | carl@example.com | password123 |

---

## 📡 API Endpoints

### Authentication
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | None | Register new account |
| POST | `/api/auth/login` | None | Login and get JWT |
| GET | `/api/auth/me` | Any | Get current user |

### Cars
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/cars` | None | List all cars with filters |
| GET | `/api/cars/{id}` | None | Get single car |
| POST | `/api/cars` | Dealer/Admin | Create listing |
| PUT | `/api/cars/{id}` | Dealer/Admin | Update listing |
| DELETE | `/api/cars/{id}` | Dealer/Admin | Delete listing |

### Bookings
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/bookings` | Buyer | Book test drive |
| GET | `/api/bookings/my` | Buyer | My bookings |
| GET | `/api/bookings/dealer` | Dealer | Bookings for my cars |
| GET | `/api/bookings` | Admin | All bookings |
| PATCH | `/api/bookings/{id}/status` | All | Update status |

### Admin
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/admin/users` | Admin | All users |
| PUT | `/api/admin/users/{id}` | Admin | Update user |
| PATCH | `/api/admin/users/{id}/role` | Admin | Change role |
| DELETE | `/api/admin/users/{id}` | Admin | Delete user |

---

## 👥 User Roles

```
Admin   → Full platform access · user management · audit log
Dealer  → Manage own listings · approve/reject bookings
Buyer   → Browse cars · book test drives · cancel bookings
```

---

## 🧪 Running Tests

```bash
cd backend/CarStock.Tests
dotnet test
```

Tests cover: AuthService · CarService · BookingService

---

## 👨‍💻 Author

**Carl Tungul**
Junior .NET Developer · Melbourne, Australia
- GitHub: [@cjmt22](https://github.com/cjmt22)
- Built as a graduate portfolio project · 2026
