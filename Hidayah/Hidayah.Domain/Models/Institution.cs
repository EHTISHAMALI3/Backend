using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Domain.Models
{
        public class Institution
        {
            [Key]
           public string InstitutionId { get; set; } = string.Empty;

            [Required(ErrorMessage = "Institution name is required.")]
            [StringLength(200)]
            public string InstitutionName { get; set; } = string.Empty;

            [StringLength(150)]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
            public string? InstitutionEmail { get; set; }

            [StringLength(20)]
            [RegularExpression(@"^((03\d{9})|(0[4-9]\d{8,9}))$",
            ErrorMessage = "Phone must be a valid Pakistani number (e.g., 03001234567 or 05823765891)")]
            public string? InstitutionPhone { get; set; }

            [Url(ErrorMessage = "Invalid website URL.")]
            public string? WebsiteUrl { get; set; }

            [DataType(DataType.Date)]
            public DateTime DateOfEstablishment { get; set; }

            [Required]
            public string CountryId { get; set; } = string.Empty;
            [Required]
            public string CityId { get; set; } = string.Empty;
            [Required]
            [StringLength(50)] // Increase to match DB
            public string InstitutionTypeId { get; set; } = string.Empty;


        [StringLength(100)]
            public string? State { get; set; }

            [StringLength(20)]
            public string? PostalCode { get; set; }

            [StringLength(300)]
            public string? AddressLine { get; set; }

            [Required]    
            [StringLength(150)]
            public string PrimaryContactPerson { get; set; } = string.Empty;

            [Required]
            [StringLength(150)]
            public string PrimaryContactFullName { get; set; } = string.Empty;

            [Required]
            [RegularExpression(@"^((03\d{9})|(0[4-9]\d{8,9}))$",
            ErrorMessage = "Phone must be a valid Pakistani number (e.g., 03001234567 or 05823765891)")]
            public string PrimaryContactPhoneNumber { get; set; } =string.Empty;

            [Required]
            [StringLength(150)]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid contact email format.")]
            public string PrimaryContactEmail { get; set; } = string.Empty;

            [Required]    
            public string PrimaryContactJobTitle { get; set; } = string.Empty;

            [Required]
            public bool InstitutionStatus { get; set; }

            [StringLength(100)]
            public string? CreatedBy { get; set; }

            [Required]
            public DateTime CreatedAt { get; set; } = DateTime.Now;

            [StringLength(100)]
            public string? ModifiedBy { get; set; }

            public DateTime? ModifiedAt { get; set; }

            [Required]
            public bool IsDeleted { get; set; } = false;
        }
    }
