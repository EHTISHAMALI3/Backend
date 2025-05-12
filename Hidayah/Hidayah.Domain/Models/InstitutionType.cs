using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Domain.Models
{
    public class InstitutionType
    {
        [Key]
        public string InstitutionTypeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type name is required.")]
        [StringLength(100, ErrorMessage = "Type name cannot exceed 100 characters.")]
        public string? TypeName { get; set; } 

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? TypeDescription { get; set; }
    }
}
