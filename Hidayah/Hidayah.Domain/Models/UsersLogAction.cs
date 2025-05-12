using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Domain.Models
{
    [Table("BGS_HA_TBL_UsersLogAction")]
    public class UsersLogAction
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Action { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        public string PerformedBy { get; set; } // Admin or System, etc.
    }
}
