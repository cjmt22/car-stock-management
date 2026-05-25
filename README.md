# 🚗 CarStock Management System

> A graduate-level full-stack car dealership platform built with ASP.NET Core 8, PostgreSQL, and JavaScript.

[![Live Demo](https://img.shields.io/badge/Live%20Demo-Visit%20Site-e94560?style=for-the-badge)](https://cjmt22.github.io/car-stock-management/frontend/index.html)
[![API Docs](https://img.shields.io/badge/API%20Docs-Swagger-85EA2D?style=for-the-badge)](https://carstock-api.onrender.com)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?style=for-the-badge&logo=github)](https://github.com/cjmt22/car-stock-management)

---

## 🌐 Live Demo

| | URL |
|---|---|
| 🌐 Frontend | https://cjmt22.github.io/car-stock-management/frontend/index.html |
| ⚙️ API (Swagger) | https://carstock-api.onrender.com |

> ⏳ **Note:** The API is on Render's free tier. First request after inactivity may take ~30 seconds to wake up. The frontend will show a loading notice while this happens.

---

## 🔑 Demo Credentials

Try the app instantly with these accounts:

| Role | Email | Password | Access |
|---|---|---|---|
| 👤 Buyer | buyer@carstock.com | Buyer2026! | Browse cars, book test drives |
| 🏪 Dealer | dealer@carstock.com | Dealer2026! | Manage listings, approve bookings |
| 🔧 Admin | admin@carstock.com | Admin2026! | Full platform access |

---

## ✨ Features

### 👤 Buyers
- Browse and search 10+ car listings with photos
- Filter by make, model, price range, year, colour
- Book test drives with preferred date and message
- Track booking status in real time
- Cancel pending bookings

### 🏪 Dealers
- Create, edit, and delete car listings with images
- View and manage test drive booking requests
- Approve or reject buyer bookings
- Mark cars as Available, Reserved, or Sold

### 🔧 Admins
- View all users, cars, and bookings platform-wide
- Manage user roles (promote Buyer to Dealer)
- Full audit log of every write action
- Platform analytics

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Backend | C# / ASP.NET Core 8 Web API |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Authentication | JWT (JSON Web Tokens) |
| Authorisation | Role-based — Admin, Dealer, Buyer |
| Frontend | HTML5 / CSS3 / Vanilla JavaScript |
| API Docs | Swagger / OpenAPI |
| Testing | xUnit + Moq + FluentAssertions |
| Deployment | Render (API + DB) · GitHub Pages (Frontend) |
| Version Control | Git / GitHub |

---

## 🏗️ Architecture

```
CarStockManagement/
├── backend/
│   ├── CarStock.API/
│   │   ├── Controllers/     # HTTP endpoints (Auth, Cars, Bookings, Admin)
│   │   ├── Services/        # Business logic + interfaces
│   │   ├── Models/          # EF Core database entities
│   │   ├── DTOs/            # Data transfer objects
│   │   ├── Data/            # AppDbContext + migrations
│   │   └── Program.cs       # App configuration + middleware
│   └── CarStock.Tests/      # xUnit tests (AuthService, CarService, BookingService)
└── frontend/
    ├── css/styles.css        # Full site styling
    ├── js/api.js             # API fetch wrappers
    ├── js/auth.js            # Token management + auth guards
    ├── index.html            # Home page
    └── pages/               # Login, Cars, Detail, Dashboards
```

---

## 📡 API Endpoints

### Authentication — `/api/auth`
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/register` | None | Register new Buyer account |
| POST | `/login` | None | Login — returns JWT token |
| GET | `/me` | Any | Get current user profile |

### Cars — `/api/cars`
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/` | None | List all cars with filters + pagination |
| GET | `/{id}` | None | Get single car detail |
| POST | `/` | Dealer/Admin | Create new listing |
| PUT | `/{id}` | Dealer/Admin | Update listing |
| DELETE | `/{id}` | Dealer/Admin | Delete listing |

### Bookings — `/api/bookings`
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/` | Buyer | Book a test drive |
| GET | `/my` | Buyer | My bookings |
| GET | `/dealer` | Dealer | Bookings for my cars |
| GET | `/` | Admin | All platform bookings |
| PATCH | `/{id}/status` | All | Update booking status |

### Admin — `/api/admin`
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/users` | Admin | All users |
| PUT | `/users/{id}` | Admin | Update user |
| PATCH | `/users/{id}/role` | Admin | Change user role |
| DELETE | `/users/{id}` | Admin | Delete user |
| GET | `/audit` | Admin | Audit log |

---

## 🚀 Run Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 16](https://www.postgresql.org/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community)

### 1. Clone the repo
```bash
git clone https://github.com/cjmt22/car-stock-management.git
cd car-stock-management
```

### 2. Set up database
Create a PostgreSQL database named `CarStockDb` then update `backend/CarStock.API/appsettings.json`:
```json
"DefaultConnection": "Host=localhost;Port=5432;Database=CarStockDb;Username=postgres;Password=YOUR_PASSWORD"
```

### 3. Run migrations
```bash
cd backend/CarStock.API
dotnet ef database update
```

### 4. Run the API
Open `CarStock.sln` in Visual Studio 2022 → press **F5**

### 5. Open the frontend
Open `frontend/index.html` in your browser.

---

## 🧪 Run Tests

```bash
cd backend/CarStock.Tests
dotnet test
```

**Test coverage:**
- `AuthServiceTests` — Register, Login, wrong password, inactive account
- `CarServiceTests` — GetAll, filter, GetById, Create, Delete ownership
- `BookingServiceTests` — Create, past date, sold car, approve, cancel, role rules

---

## 📬 Postman Collection

Import `CarStock-API.postman_collection.json` from the root of this repo into Postman to test all API endpoints immediately.

---

## 👨‍💻 Author

**Carl Tungul** — Junior .NET Developer · Melbourne, Australia

- 🎓 Bachelor of IT — Victoria University (2024)
- 💼 Open to Junior .NET Developer roles in Melbourne
- 🔗 [LinkedIn](https://linkedin.com/in/YOUR-LINKEDIN) · [GitHub](https://github.com/cjmt22)

---

*Built as a graduate portfolio project · 2026*
