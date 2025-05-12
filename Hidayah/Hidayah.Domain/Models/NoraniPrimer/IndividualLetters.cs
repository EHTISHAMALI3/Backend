using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Domain.Models.NoraniPrimer
{
    public class IndividualLetters
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(10)]
        public string Arabic { get; set; }
        [Required, MaxLength(20)]
        public string Urdu { get; set; }

        [Required, MaxLength(255)]
        public string SvgPath { get; set; }

        [Required, MaxLength(255)]
        public string AudioPath { get; set; }

        [Required, MaxLength(100)]
        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool Deleted { get; set; } = false;
    }
}