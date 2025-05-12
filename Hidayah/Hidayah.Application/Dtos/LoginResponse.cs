using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Dtos
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // If you want to send user role as well
        public string IsLocked { get; set; }
        public string FailedLoginAttempts { get; set; }
    }
}
