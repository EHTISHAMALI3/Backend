using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Dtos
{
    public class BranchRequest
    {

            [Key]
            public int BranchId { get; set; }

            [Required(ErrorMessage = "Institution is required.")]
            public int InstitutionId { get; set; }

            [Required(ErrorMessage = "Branch name is required.")]
            [MaxLength(100, ErrorMessage = "Branch name cannot exceed 100 characters.")]
            public string BranchName { get; set; }

            [MaxLength(50, ErrorMessage = "Branch code cannot exceed 50 characters.")]
            public string BranchCode { get; set; }

            [Required(ErrorMessage = "Branch Manager is required.")]
            public int BranchManagerId { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string BranchEmail { get; set; }

            [Required(ErrorMessage = "Phone number is required.")]
            [RegularExpression(@"^(\+92|0)?3[0-9]{9}$", ErrorMessage = "Invalid Pakistan phone number format.")]
            public string BranchPhone { get; set; }

            [Required(ErrorMessage = "Address is required.")]
            [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
            public string BranchAddress { get; set; }

            [MaxLength(100)]
            public string Street { get; set; }

            public int CityId { get; set; }

            [MaxLength(50)]
            public string State { get; set; }

            [MaxLength(20)]
            public string PostalCode { get; set; }

            public int CountryId { get; set; }

            [Required]
            public bool Status { get; set; } = true;
            [Required]
            public bool IsDeleted { get; set; } = false;
        }
    }

