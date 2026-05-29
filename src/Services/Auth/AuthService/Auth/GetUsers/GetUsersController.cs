using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Auth.GetUsers
{
    [Route("auth/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class GetUsersController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            var query = new GetUsersQuery();
            var result = await sender.Send(query);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
    }
}
