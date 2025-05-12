using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Dtos
{
    public class CompoundLettersDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(10)]
        public string Arabic { get; set; }
        [Required, MaxLength(20)]
        public string Urdu { get; set; }

        [Required]
        public IFormFile SvgFile { get; set; }

        [Required]
        public IFormFile AudioFile { get; set; }

        [Required, MaxLength(100)]
        public string CreatedBy { get; set; }
    }
}
