# Meeting Room Booking System

A full-stack meeting room booking application
demonstrating Clean Architecture, SOLID principles
and modern development practices.

**Backend:** .NET 10 Web API
**Frontend:** Angular 17 (Standalone Components)
**Database:** SQL Server (LocalDB for development)

---

## Solution Structure
MeetingRoomBooking/
├── MeetingRoomBooking.Domain/
│   ├── Entities/          ← Room, Booking, User
│   ├── Exceptions/        ← Domain-specific exceptions
│   └── Enums/             ← BookingStatus
│
├── MeetingRoomBooking.Application/
│   ├── Interfaces/        ← IBookingService, IBookingRepository
│   ├── Services/          ← Business logic
│   ├── DTOs/              ← Request/Response models
│   └── Validators/        ← FluentValidation rules
│
├── MeetingRoomBooking.Infrastructure/
│   ├── Data/              ← AppDbContext, Migrations
│   └── Repositories/      ← EF Core implementations
│
├── MeetingRoomBooking.API/
│   ├── Controllers/       ← HTTP endpoints
│   ├── Middleware/        ← Global exception handling
│   └── Program.cs         ← DI registration, pipeline
│
├── MeetingRoomBooking.Tests/
│   └── Services/          ← Unit tests
│
└── meeting-room-booking/  ← Angular frontend

---

## Running the Backend

### Prerequisites
- .NET 10 SDK
- SQL Server LocalDB (included with Visual Studio)

### Steps
1. Clone the repository
2. Navigate to `MeetingRoomBooking.API`
3. Update `appsettings.json` connection string if needed
4. Run the application:
```bash
   dotnet run
```
5. API starts at `https://localhost:7200`
6. Database migrations run automatically on startup
7. 5 meeting rooms are seeded automatically
8. API documentation available at:
   `https://localhost:7200/scalar/v1`

---

## Running the Frontend

### Prerequisites
- Node.js 18+
- Angular CLI: `npm install -g @angular/cli`

### Steps
1. Navigate to `meeting-room-booking` folder
2. Install dependencies:
```bash
   npm install
```
3. Start the development server:
```bash
   ng serve
```
4. Application runs at `http://localhost:4200`
5. Register an account or use the booking
   features directly (auth is optional per spec)

---

## Running the Tests

### Backend Tests
```bash
cd MeetingRoomBooking.Tests
dotnet test
```

### Frontend Tests
```bash
cd meeting-room-booking
ng test
```

---

## Key Features

- **View Rooms** — 5 pre-seeded meeting rooms
  with name, description and capacity
- **View Bookings** — all bookings or filtered
  by room, sorted by start time
- **Create Booking** — with full validation
  and conflict detection
- **Edit Booking** — updates with same
  business rules applied
- **Cancel Booking** — soft cancel with
  confirmation dialog and user feedback
- **Conflict Detection** — prevents overlapping
  bookings for the same room
- **JWT Authentication** — optional login/register
  to demonstrate security knowledge

---

## Architecture Decisions

### Why Clean Architecture?
Project references enforce layer boundaries
at compile time — not just folder conventions.
Domain has zero dependencies meaning infrastructure
changes never affect business logic.
Swapping SQL Server for MongoDB only touches
the Infrastructure layer.

### Why Repository Pattern?
BookingService depends on IBookingRepository
not AppDbContext directly — making business
logic independently testable with mock repositories
and decoupled from EF Core specifics.

### Why Soft Cancel?
Preserves audit trail for booking history.
Cancelled bookings don't block the same time
slot from being rebooked. Enables cancellation
rate reporting without losing historical data.

### Why FluentValidation?
Keeps validation rules separate from DTOs
maintaining Single Responsibility Principle.
Supports complex cross-field rules like
end time after start time more cleanly
than Data Annotations.

### Why Global Exception Middleware?
Centralises all error handling in one place.
Controllers contain zero try/catch — only
happy path logic. Error format changes once
and affects all endpoints consistently.

---

## API Endpoints

### Rooms
GET  /api/rooms          Get all rooms
GET  /api/rooms/{id}     Get room by ID

### Bookings
GET    /api/bookings              All bookings
GET    /api/bookings/{id}         Single booking
GET    /api/bookings/room/{id}    Bookings by room
POST   /api/bookings              Create booking
PUT    /api/bookings/{id}         Update booking
DELETE /api/bookings/{id}         Cancel booking

### Auth
POST /api/auth/register    Register account
POST /api/auth/login       Login

---

## Assumptions

- Authentication is not required per the spec
  but was included to demonstrate JWT knowledge
- Rooms are seeded rather than managed through
  an admin interface as per the spec
- Soft cancel used over hard delete for
  audit trail purposes
- Single office location supported
- No recurring bookings as per scope

## Known Limitations

- **Race condition** — simultaneous booking
  requests could theoretically both pass the
  conflict check before either saves.
  Production fix: optimistic concurrency with
  EF Core Timestamp property or database-level
  unique constraint on room/time combination.

- **No real-time updates** — users need to
  refresh to see bookings created by others.
  Production fix: SignalR for live updates.

- **JWT secret in config** — for production
  this moves to environment variables or
  Azure Key Vault.