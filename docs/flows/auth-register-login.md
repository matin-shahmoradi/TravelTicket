# Flow: Register -> Confirm email -> Login
Service: Auth (`src/Services/Auth/AuthService`). Gateway prefix `/auth`. No messaging involved.

1. `POST auth/register` - `Auth/Register/UserRegisterEndpoint.cs` wraps `RegisterRequestDto` in `UserRegisterCommand`, sends via MediatR; failures go through `Result.ToHttpResult`.
2. `Auth/Register/UserRegisterCommandHandler.cs`:
   - Email already exists -> `Error.Conflict`.
   - `ApplicationUser.CreateUser(...)` (`Model/ApplicationUser.cs`); username = email.
   - Inside `ITransactionExecutor.ExecuteAsync`: create user (Identity) -> add to role `User` (`Model/Roles.cs`) -> `IUserRegistrationEmailSender.SendEmailRegistration`.
   - Email failure returns a 500 error from inside the transaction (rollback behaviour depends on `BuildingBlocks/EntityFramwork/EfTransactionExecutor.cs`; UNKNOWN).
3. Email (MailKit via `Services/Email/`) contains a link built from `EmailConfirmationUrl` config -> `GET auth/confirm-email?userId&token`.
4. `Auth/ConfirmEmail/ConfirmEmailEndpoint.cs` -> `ConfirmEmailCommand(userId, token)` -> handler in the same folder.
5. `POST auth/login` - `Auth/Login/LoginEndpoint.cs` -> `LoginCommandHandler.cs`:
   - Lookup by email; wrong user or password -> `Error.UnAuthorized` (same message for both).
   - Load roles via `IUserRoleQueryService`; no roles -> `Error.Forbidden`.
   - `Repositories/JsonWebTokenService.cs` builds an HS256 JWT. Claims: sub, unique_name, email, phone_number, jti, role(s). Lifetime 24h. Config `Jwt:Key|Issuer|Audience`.
   - Response `LoginResponse(email, username, token)`.
6. Client sends `Authorization: Bearer <token>`; other services validate it (Catalog uses section `JwtSetting`, Auth uses `Jwt`; the keys must match by deployment).

Notes:
- `JsonWebTokenService` uses `user.PhoneNumber!`; a user without a phone number would hit a null claim value (verify).
- Whether login requires a confirmed email: UNKNOWN (`IUserSignInManagerService.ValidateUserPassword` not read).
- Tests: `AuthSerive.UnitTests` (Login, Register, ConfirmEmail handlers).
