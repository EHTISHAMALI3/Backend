using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Domain.Models
{

        public class BranchModel
        {
            [Key]
            public string BranchId { get; set; } = string.Empty;

            [Required(ErrorMessage = "Institution is required.")]
            public string InstitutionId { get; set; } 

            [Required(ErrorMessage = "Branch name is required.")]
            [MaxLength(100, ErrorMessage = "Branch name cannot exceed 100 characters.")]
            public string BranchName { get; set; } = string.Empty;

            [MaxLength(50, ErrorMessage = "Branch code cannot exceed 50 characters.")]
            public string? BranchCode { get; set; }

            [Required(ErrorMessage = "Branch Manager is required.")]
            public string BranchManagerId { get; set; } 

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string BranchEmail { get; set; } = string.Empty;

            [StringLength(20)]
            [RegularExpression(@"^((03\d{9})|(0[4-9]\d{8,9}))$",
            ErrorMessage = "Phone must be a valid number (e.g., 03001234567 or 05823765891)")]
            public string BranchPhone { get; set; } = string.Empty;

            [Required(ErrorMessage = "Address is required.")]
            [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
            public string BranchAddress { get; set; } = string.Empty;

            [MaxLength(100)]
            public string? Street { get; set; }

            [Required]
            public string CityId { get; set; } =string.Empty;

            [MaxLength(50)]
            public string? State { get; set; }

            [MaxLength(20)]
            public string? PostalCode { get; set; }

            [Required]
            public string CountryId { get; set; } =string.Empty;

            [Required]
            public bool Status { get; set; } = true;
            [Required]
            public bool IsDeleted { get; set; } = false;

            [StringLength(100)]
            public string? CreatedBy { get; set; }
            [StringLength(100)]
            public string? ModifiedBy { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
            public DateTime? ModifiedAt { get; set; }
    }
}

