using Hidayah.Application.Dtos;
using Hidayah.Application.Generic;
using Hidayah.Domain.Models;
using Hidayah.Infrastrcture.Repositriy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hidayah.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepositriy _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepositriy userRepository, ILogger<UsersController> logger) 
        { 
            _userRepository = userRepository;
            _logger = logger;


        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation failed during user registration.");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new mGeneric.mApiResponse<List<string>>(400, "Validation errors", errors));
            }

            // Check if user already exists
            if (await _userRepository.UserExistsAsync(model.UserName, model.Email))
            {
                _logger.LogWarning("User already exists.");
                return Conflict(new mGeneric.mApiResponse<string>(409, "Username or Email already exists."));
            }

            // Register user
            var response = await _userRepository.RegisterUserAsync(model);
            return StatusCode(response.RespCode, response);
        }
        [HttpPost("login")]
        public async Task<ActionResult<mGeneric.mApiResponse<object>>> Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.UsernameOrEmail) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new mGeneric.mApiResponse<object>(400, "Username/Email or Password is required"));
            }

            try
            {
                var result = await _userRepository.LoginUserAsync(loginRequest, Request);
                return Ok(new mGeneric.mApiResponse<object>(200, "Login successful", new { Token = result }));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new mGeneric.mApiResponse<object>(401, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new mGeneric.mApiResponse<object>(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new mGeneric.mApiResponse<object>(500, "An error occurred during login"));
            }
        }

    }
}
