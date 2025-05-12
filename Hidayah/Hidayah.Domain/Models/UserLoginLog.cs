using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Domain.Models
{
    public class UserLoginLog
    {
        [Key]
        public int LogId { get; set; }
        public string UserId { get; set; }
        public DateTime AttemptTime { get; set; }
        public bool IsSuccessful { get; set; }
        public string FailedReason { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
