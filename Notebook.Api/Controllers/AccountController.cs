using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notebook.Api.Models;
using Notebook.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace Notebook.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<NotebookUser> _userManager;
        private readonly SignInManager<NotebookUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<NotebookUser> userManager, SignInManager<NotebookUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<ActionResult<TokenResponse>> Register([FromBody] Credentials credentials)
        {
            var user = new NotebookUser { UserName = credentials.Email, Email = credentials.Email };
            var result = await _userManager.CreateAsync(user, credentials.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var response = new TokenResponse() { Token = _tokenService.CreateForUser(user) };
            return Ok(response);
        }

        public class Credentials
        {
            [Required]
            public string Email { get; set; } = null!;

            [Required]
            public string Password { get; set; } = null!;
        }

        public class TokenResponse
        {
            [Required]
            public string Token { get; set; } = null!;
        }
    }
}
