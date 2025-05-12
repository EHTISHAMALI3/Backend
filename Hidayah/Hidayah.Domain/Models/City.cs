using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Domain.Models
{
    public class City
    {
        [Key]
        public int CityInternalId { get; set; }
        public string CityId { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
    }
}
