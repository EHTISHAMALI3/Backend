using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Hidayah.Domain.Models
{

    public class User
    {
        [Key]
        [Required]
        public string UserId { get; set; }

        [Required]
        public int UserTypeId { get; set; }

        [Required]
        public int RoleId { get; set; }

        //[Required(ErrorMessage = "Password is required.")]
        //[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string PasswordHash { get; set; }

        [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must be alphanumeric.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "First Name must be between 3 and 25 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Last Name must be between 3 and 25 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(20, ErrorMessage = "Phone Number cannot exceed 20 characters.")]
        [RegularExpression(@"^(03[0-9]{2}|04[0-9]{2}|05[0-9]{2}|06[0-9]{2}|07[0-9]{2}|08[0-9]{2}|09[0-9]{2})[0-9]{7}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Institution is required.")]
        public string InstitutionId { get; set; }

        [Required(ErrorMessage = "Branch is required.")]
        public string BranchId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "City cannot exceed 20 characters.")]
        public string CityId { get; set; }

        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
        public string? State { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Country cannot exceed 20 characters.")]
        public string CountryId { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "Designation cannot exceed 100 characters.")]
        public string? Designation { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        public string? Image { get; set; }

        [StringLength(50, ErrorMessage = "Grade cannot exceed 50 characters.")]
        public string? Grade { get; set; }

        [Required(ErrorMessage = "Joining Date is required.")]
        public DateTime JoiningDate { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        [Required]
        public bool IsLocked { get; set; } = false;

        [Required]
        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockoutEndTime { get; set; }

        //[Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string? ModifiedBy { get; set; }

        [Required]
        public bool Deleted { get; set; } = false;
    }



}
