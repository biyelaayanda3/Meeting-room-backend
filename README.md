# Meeting Room Booking System

## Overview
A full-stack meeting room booking application built with .NET 10 API and Angular 17, following Clean Architecture and SOLID principles.

## Solution Structure
```text
MeetingRoomBooking/
|-- MeetingRoomBooking.Domain/        <- Entities, exceptions
|-- MeetingRoomBooking.Application/   <- Business logic, DTOs
|-- MeetingRoomBooking.Infrastructure <- EF Core, repositories
|-- MeetingRoomBooking.API/           <- Controllers, middleware
|-- MeetingRoomBooking.Tests/         <- Unit tests
`-- meeting-room-booking/             <- Angular frontend
```

## Running the Backend
1. Navigate to `MeetingRoomBooking.API`
2. Update `appsettings.json` connection string
3. Run: `dotnet run`
4. API runs at: `https://localhost:7200`
5. API docs at: `https://localhost:7200/scalar/v1`
6. Migrations run automatically on startup!

## Running the Frontend
1. Navigate to `meeting-room-booking` folder
2. Run: `npm install`
3. Run: `ng serve`
4. App runs at: `http://localhost:4200`

## Running Tests
1. Navigate to `MeetingRoomBooking.Tests`
2. Run: `dotnet test`

## Assumptions
- No recurring bookings required
- Single office location
- Rooms seeded with 5 predefined rooms
- Authentication included to demonstrate JWT knowledge (not required per spec but added as bonus)
- Soft cancel used instead of hard delete for audit trail purposes

## Trade-offs
- Used in-memory for conflict detection instead of database-level locks for simplicity
- No real-time updates; page refresh needed to see other users' bookings
- Angular standalone components used throughout for modern best practices
