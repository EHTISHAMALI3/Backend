using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Domain.Models
{
    public class Country
    {
        [Key]
        public int CountryInternalId { get; set; }
        public string CountryId { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
    }
}
