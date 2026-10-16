# Auth (`src/Services/Auth/AuthService`)
**Responsibility:** user registration, email confirmation, login (JWT issuer), user lookup. ASP.NET Identity on Postgres `AuthDb`.
**Endpoints** (Carter unless noted; Gateway prefix `/auth`):
- POST `auth/register` - RegisterRequestDto -> RegisterResponseDto; sends confirmation email.
- GET `auth/confirm-email?userId&token`
- POST `auth/login` - `LoginRequest(Email,Password)` -> `LoginResponse(email,username,token)`
- GET `auth/users?email=` - policy AdminOnly
- GET `auth/GetUsers` (MVC controller `Auth/GetUsers/GetUsersController.cs`, `[Route("auth/[controller]")]`) - AdminOnly; no route conflict with the Carter endpoint.
**Models:** `Model/ApplicationUser.cs` (IdentityUser + FirstName, LastName), roles `User|Admin|SuperAdmin` (`Model/Roles.cs`). Seed: `Data/SeedAuthDb.cs`.
**Structure:** `Auth/<Feature>/{Command,Handler,Endpoint}`; services in `Repositories/` behind `Interfaces/`; email in `Services/Email/` (MailKit active; FluentEmail legacy).
**Deps/events:** none sent/received. Mailpit SMTP (compose 2525; appsettings says 2526 - mismatch). Config keys: `Jwt:Key|Issuer|Audience` (empty in appsettings; supplied by env/user-secrets), `EmailConfirmationUrl`.
**Run:** compose port 6066, health `/health-auth`. Local: `dotnet run` in the project dir (http 5279 / https 7067 per Gateway).
**Test:** `dotnet test AuthSerive.UnitTests` (xUnit; handlers+validators for Login, Register, ConfirmEmail, GetUserByEmail).
**Unknowns:** exact Register DTO fields; token lifetime/claims (`Repositories/JsonWebTokenService.cs` unread); DB migration strategy.
