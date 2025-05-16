using Hidayah.Application.Dtos;
using Hidayah.Application.Generic;
using Hidayah.Application.Services.Decryption;
using Hidayah.Application.Services.Encryption;
using Hidayah.Domain.Models;
using Hidayah.Infrastrcture.AppDbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Infrastrcture.Repositriy
{
    public class UserRepositoryImpl : IUserRepositriy
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepositoryImpl> _logger;
        private readonly IConfiguration _configuration;
        private readonly DecryptionService _decryptionService;
        private readonly EncryptionService _encryptionService;
        public UserRepositoryImpl(ApplicationDbContext context, ILogger<UserRepositoryImpl> logger, IConfiguration configuration, DecryptionService decryptionService, EncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _decryptionService = decryptionService;
            _encryptionService = encryptionService;

        }

        public async Task<mGeneric.mApiResponse<User>> RegisterUserAsync(User model)
        {
            try
            {
                // 1. Determine prefix based on RoleId
                string prefix = model.RoleId switch
                {
                    1 => "S-",  // Learner
                    2 => "E-",  // Instructor
                    3 => "E-",  // Manager
                    4 => "E-",  // Admin
                    5 => "E-",  // SuperAdmin
                    _ => "S-"   // Default for unknown roles
                };

                bool userExsist = await _context.BGS_HA_TBL_USERS.AnyAsync(u => u.UserId == model.UserId);
                if (userExsist)
                {
                    return new mGeneric.mApiResponse<User>(409, $"User ID '{model.UserId}' already exists.");
                }
                // 2. Get all existing user IDs with the same prefix (E- or S-) that are not deleted
                var existingIds = await _context.BGS_HA_TBL_USERS
                    .Where(u => u.UserId.StartsWith(prefix))
                    .Select(u => u.UserId.Substring(2)) // remove prefix
                    .ToListAsync();

                // 3. Convert to int and calculate max
                var numericIds = existingIds
                    .Select(id => int.TryParse(id, out var val) ? val : 0)
                    .ToList();

                int nextNumericId = numericIds.Any() ? numericIds.Max() + 1 : 1;

                // 4. Construct the full UserId (e.g., E-0001 or S-0002)
                model.UserId = $"{prefix}{nextNumericId:D4}";

                // 5. Check for existing UserId just in case (should not happen, but for safety)
                // 6. Normalize and assign UserName from Email prefix
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var emailPrefix = model.Email.Split('@')[0].Trim().ToLower();
                    model.UserName = emailPrefix;
                }

                // 6. Check for duplicate username
                if (await _context.BGS_HA_TBL_USERS.AnyAsync(u => u.UserName == model.UserName ))
                {
                    return new mGeneric.mApiResponse<User>(409, $"Username '{model.UserName}' already exists.");
                }

                // 7. Check for duplicate email
                if (!string.IsNullOrEmpty(model.Email) &&
                    await _context.BGS_HA_TBL_USERS.AnyAsync(u => u.Email == model.Email ))
                {
                    return new mGeneric.mApiResponse<User>(409, $"Email '{model.Email}' is already registered.");
                }

                // 8. Prepare new user data
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);
                model.CreatedBy = "admin";
                model.CreatedAt = DateTime.UtcNow;

                // 9. Save to DB
                _context.BGS_HA_TBL_USERS.Add(model);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {model.UserName} registered successfully.");
                return new mGeneric.mApiResponse<User>(200, "User registered successfully", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration.");
                return new mGeneric.mApiResponse<User>(500, "An error occurred while processing your request.");
            }
        }


        public async Task<mGeneric.mApiResponse<LoginResponse>> LoginUserAsync(LoginRequest loginRequest, HttpRequest httpRequest)
        {
            try
            {
                // Decrypt username/email and password
                var decryptedUsernameOrEmail = _decryptionService.DecryptWithPrivateKey(loginRequest.UsernameOrEmail);
                var decryptedPassword = _decryptionService.DecryptWithPrivateKey(loginRequest.Password);

                if (string.IsNullOrEmpty(decryptedUsernameOrEmail) || string.IsNullOrEmpty(decryptedPassword))
                    return new mGeneric.mApiResponse<LoginResponse>(401, "Decryption failed. Invalid credentials.");

                var user = await GetUserByUsernameOrEmailAsync(decryptedUsernameOrEmail);
                if (user == null)
                    return new mGeneric.mApiResponse<LoginResponse>(404, "User not found");

                if (user.IsLocked)
                {
                    // Ensure both times are in UTC for proper comparison
                    DateTime currentTimeUtc = DateTime.UtcNow;

                    // Check if LockoutEndTime is not null and is greater than the current time in UTC
                    if (user.LockoutEndTime.HasValue && user.LockoutEndTime.Value > currentTimeUtc)
                    {
                        // Convert LockoutEndTime from UTC to PST (UTC +5)
                        DateTime convertedPST = TimeZoneInfo.ConvertTimeFromUtc(user.LockoutEndTime.Value, TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi"));

                        // Return the response with the converted time in PST
                        return new mGeneric.mApiResponse<LoginResponse>(403, "Account is locked until " + convertedPST.ToString("g"));
                    }
                }



                if (!BCrypt.Net.BCrypt.Verify(decryptedPassword, user.PasswordHash))
                {
                    await InsertLoginLogAsync(new UserLoginLog
                    {
                        UserId = user.UserId,
                        AttemptTime = DateTime.UtcNow,
                        IsSuccessful = false,
                        FailedReason = "Invalid password",
                        IPAddress = httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString(),
                        UserAgent = httpRequest.Headers["User-Agent"].ToString()
                    });

                    await IncrementFailedLoginAttemptsAsync(user.UserId);

                    return new mGeneric.mApiResponse<LoginResponse>(401, "Invalid credentials");
                }
                // Fetch role-based permissions from UserRoles table
                var userRole = await _context.BGS_HA_TBL_USER_ROLES
                    .FirstOrDefaultAsync(r => r.RoleId == user.RoleId);

                if (userRole == null)
                {
                    return new mGeneric.mApiResponse<LoginResponse>(404, "User role not found.");
                }

                await ResetFailedLoginAttemptsAsync(user.UserId);

                await InsertLoginLogAsync(new UserLoginLog
                {
                    UserId = user.UserId,
                    AttemptTime = DateTime.UtcNow,
                    IsSuccessful = true,
                    FailedReason = "Successful login",
                    IPAddress = httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = httpRequest.Headers["User-Agent"].ToString()
                });

                var token = GenerateToken(user);

                var response = new LoginResponse
                {
                    Token = token,
                    UserId = _encryptionService.EncryptWithPublicKey(user.UserId),
                    UserName = _encryptionService.EncryptWithPublicKey(user.UserName),
                    Email = _encryptionService.EncryptWithPublicKey(user.Email),
                    Role = user.RoleId.ToString(),
                    IsLocked = _encryptionService.EncryptWithPublicKey(user.IsLocked.ToString()),
                    FailedLoginAttempts = _encryptionService.EncryptWithPublicKey(user.FailedLoginAttempts.ToString()),

                    CanView = _encryptionService.EncryptWithPublicKey(userRole.CanView.ToString()),
                    CanAdd = _encryptionService.EncryptWithPublicKey(userRole.CanAdd.ToString()),
                    CanUpdate = _encryptionService.EncryptWithPublicKey(userRole.CanUpdate.ToString()),
                    CanDelete = _encryptionService.EncryptWithPublicKey(userRole.CanDelete.ToString())
                };

                return new mGeneric.mApiResponse<LoginResponse>(200, "Login successful", response);
            }
            catch (Exception ex)
            {
                // Log the error here
                _logger.LogError(ex, "LoginUserAsync failed");
                return new mGeneric.mApiResponse<LoginResponse>(500, "Internal server error");
            }
        }



        public async Task<User> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.BGS_HA_TBL_USERS
                .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);
        }

        public async Task IncrementFailedLoginAttemptsAsync(string userId)
        {
            var user = await _context.BGS_HA_TBL_USERS.FindAsync(userId);
            if (user != null)
            {
                user.FailedLoginAttempts += 1;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.IsLocked = true;
                    user.LockoutEndTime = DateTime.UtcNow.AddMinutes(3); // Lock for 2 minutes
                }
                _context.BGS_HA_TBL_USERS.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResetFailedLoginAttemptsAsync(string userId)
        {
            var user = await _context.BGS_HA_TBL_USERS.FindAsync(userId);
            if (user != null)
            {
                user.FailedLoginAttempts = 0;
                user.IsLocked = false;
                user.LockoutEndTime = null;
                _context.BGS_HA_TBL_USERS.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task InsertLoginLogAsync(UserLoginLog log)
        {
            await _context.BGS_HA_TBL_USER_LOGIN_LOGS.AddAsync(log);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UserExistsAsync(string userName, string email)
        {
            return await _context.BGS_HA_TBL_USERS.AnyAsync(u => u.UserName == userName || u.Email == email);

        }
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.Email) }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:AccessTokenExpireMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:ValidIssuer"],
                Audience = _configuration["Jwt:ValidAudience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        //}
    }

}
