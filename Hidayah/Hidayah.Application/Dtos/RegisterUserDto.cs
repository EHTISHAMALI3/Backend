using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Dtos
{
    public class RegisterUserDto
    {
        public int? RoleId { get; set; }
        public int? SchoolId { get; set; }
        public int? SessionId { get; set; }
        public int? ClassId { get; set; }

        public string PasswordHash { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Designation { get; set; }
        public string Image { get; set; }
        public string Grade { get; set; }
        public DateTime JoiningDate { get; set; }
    }

}
