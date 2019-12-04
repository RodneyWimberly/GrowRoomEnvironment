using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class Notification : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public bool IsPinned { get; set; }
        public DateTime Date { get; set; }
    }
}
