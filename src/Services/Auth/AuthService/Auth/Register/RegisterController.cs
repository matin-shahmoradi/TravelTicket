using AuthService.Model.DTOs.RegisterDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Auth.Register
{
    [Route("auth/[controller]")]
    [ApiController]
    public class RegisterController(RegisterService registerService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<RegisterResponse>> RegisterAsync([FromBody] RegisterRequest request)
        {
            var result = await registerService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}
