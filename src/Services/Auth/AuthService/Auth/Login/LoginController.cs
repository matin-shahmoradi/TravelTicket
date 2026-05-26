using AuthService.Model.DTOs.LoginDtos;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Auth.Login
{
    [ApiController]
    [Route("/auth/[controller]")]
    public class LoginController(LoginService loginService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await loginService.LoginAsync(request.Email,request.Password);
            if (!result.IsSuccess)
            {
                return Unauthorized(result.Error);
            }

            return Ok(result);
        }
    }
}
