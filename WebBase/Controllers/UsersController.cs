using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Constants;
using Data.Entities.User;
using Data.ViewModels.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IServices;
using WebBase.Attributes;

namespace WebBase.Controllers
{
    [Authorize()]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ApiBaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
        {
            var response = await _userService.Authenticate(model, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticateResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _userService.RefreshToken(refreshToken, IpAddress());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }


        [Authorize()]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.RefreshToken ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });
            
            // users can revoke their own tokens and admins can revoke any tokens
            if (!Account.OwnsToken(token) && Account.Role != StringEnums.Roles.SeniorAdmin)
                return Unauthorized(new { message = "Unauthorized" });

            await _userService.RevokeToken(token, IpAddress());
            return Ok(new { message = "Token revoked" });
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            var user = _mapper.Map<User>(model);
            await _userService.Register(user, model.Password, Request.Headers["origin"]);
            return Ok(new
                { message = "Registration successful, please check your email for verification instructions" });
        }


        [AllowAnonymous]
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailRequest model)
        {
            await _userService.VerifyEmail(model.Token);
            return Ok(new { message = "Verification successful, now you can login" });
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await _userService.ForgotPassword(model, Request.Headers["origin"]);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            await _userService.ResetPassword(model);
            return Ok(new { message = "Password reset successful, now you can login" });
            
        }

        [Authorize()]
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponse>> GetById(string id)
        {
            if (id != Account.Id && Account.Role != StringEnums.Roles.SeniorAdmin)
                return Unauthorized(new { message = "Unauthorized" });

            var account = await _userService.GetById(id);
            return Ok(account);
        }
        
        // helper method
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-for"))
                return Request.Headers["X-Forwarded-For"];
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }
    }
}