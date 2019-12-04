using GrowRoomEnvironment.DataAccess.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class Event : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }
        public string Name { get; set; }
        public int ActionDeviceId { get; set; }
        public ActionDevice ActionDevice { get; set; }
        public ActionDeviceStates State { get; set; }

        public ICollection<EventCondition> EventConditions { get; set; }
    }
}
