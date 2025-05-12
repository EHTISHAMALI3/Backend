using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Domain.Models
{
    public class LabModel
    {
        [Key]
        public string LabId { get; set; } = string.Empty;
        public string LabName { get; set; } = string.Empty;
        [Required]
        public string BranchId { get; set; } = string.Empty; // Loosely coupled

        public int NumberOfSeats { get; set; }

        // Comma-separated equipment: "Computers,Headsets,Projector"
        public string? LabEquipment { get; set; }

        // Comma-separated days: "Monday,Tuesday"
        public string? OperationalDays { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string LabManagerId { get; set; } // Loosely coupled

        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }


}
