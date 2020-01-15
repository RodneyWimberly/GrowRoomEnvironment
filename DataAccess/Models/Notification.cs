using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class Notification : ApplicationEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public bool IsPinned { get; set; }
        public DateTime Date { get; set; }

        public Notification()
        {

        }

        public Notification(int notificationId, string header, string body, bool isRead, bool isPinned, DateTime date)
        {
            NotificationId = notificationId;
            Header = header;
            Body = body;
            IsRead = isRead;
            IsPinned = isPinned;
            Date = date;
        }
    }
}
