using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizDesk.Application.DTOs.OAuth;
using QuizDesk.Application.Interfaces.OAuth;

namespace QuizDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IOAuthService _oauthService;
        public OAuthController(IOAuthService oauthService) => _oauthService = oauthService;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] OAuthRequest request)
        {
            var response = await _oauthService.LoginAsync(request);
            return Ok(response);
        }
       
    }
}
