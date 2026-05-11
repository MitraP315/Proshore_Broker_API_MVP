# Proshore House Broker MVP
A modern House Broker platform backend built using Clean Architecture, ASP.NET Core, JWT Authentication, and Entity Framework Core.
The system supports role-based property management, booking workflows, commission automation, and admin reporting.

Architecture
Solution Structure


ProshoreHouseBroker.Domain


ProshoreHouseBroker.Application


ProshoreHouseBroker.Infrastructure


ProshoreHouseBroker.Api


ProshoreHouseBroker.Tests



Tech Stack


ASP.NET Core Web API


.NET 8 (LTS)


Entity Framework Core


SQL Server


JWT Authentication


ASP.NET Identity


FluentValidation


Swagger / OpenAPI


xUnit Testing


In-Memory Caching



Key Features
Authentication & Security


JWT-based authentication


Role-based access (Admin, Broker, House Seeker)


Email confirmation required


Broker mobile verification


Optional MFA (Two-Factor Authentication)



Property Management


Broker-only property CRUD


Multi-image upload support


Public property search with filters


Cached listing endpoints for performance



Booking System


House seekers can book properties


Brokers approve or reject bookings


Admin monitors all bookings



Commission System


Database-driven commission rules


Configurable percentage slabs


Automatic commission calculation on property creation


Broker-only commission visibility


Admin commission reporting



Admin Features


Manage brokers and users


View booking activity


Commission reports


System-wide monitoring



Performance


In-memory caching for property data


Optimized EF Core queries



Testing


xUnit test framework


Fake repositories for isolation


Covers:


Commission calculation logic


Business rules validation


Service layer behavior





Getting Started
1. Clone repository
git clone https://github.com/MitraP315/Proshore_Broker_API_MVPcd ProshoreHouseBroker
2. Restore dependencies
dotnet restore ProshoreHouseBrokerMvp.sln
3. Build solution
dotnet build ProshoreHouseBrokerMvp.sln
4. Run API
dotnet run --project ProshoreHouseBroker.Api
5. Run tests
dotnet test ProshoreHouseBrokerMvp.sln

Database Configuration
The project uses SQL Server LocalDB:
(localdb)\MSSQLLocalDB
Configured in:
ProshoreHouseBroker.Api/appsettings.json
On startup:


Database is created automatically


Migrations are applied


Default commission rules are seeded



Project Goals


Clean and scalable backend architecture


Production-ready authentication system


Configurable business rules (commission engine)


Secure role-based platform


Designed for future microservices expansion



License
This project is licensed under the MIT License.
