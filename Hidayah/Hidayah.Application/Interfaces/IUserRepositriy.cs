using Hidayah.Application.Dtos;
using Hidayah.Application.Generic;
using Hidayah.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Infrastrcture.Repositriy
{
    public interface IUserRepositriy
    {
        public Task<mGeneric.mApiResponse<User>> RegisterUserAsync(User model);
        public Task<mGeneric.mApiResponse<LoginResponse>> LoginUserAsync(LoginRequest loginRequest, HttpRequest httpRequest);
        public Task<bool> UserExistsAsync(string userName, string email);
        Task<User> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task IncrementFailedLoginAttemptsAsync(string userId);
        Task ResetFailedLoginAttemptsAsync(string userId);
        Task InsertLoginLogAsync(UserLoginLog log);
        public string GenerateToken(User user);
    }
}
