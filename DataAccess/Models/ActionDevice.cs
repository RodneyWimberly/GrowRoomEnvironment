using GrowRoomEnvironment.DataAccess.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class ActionDevice : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionDeviceId { get; set; }
        public string Name { get; set; }
        public ActionDeviceTypes Type { get; set; }
        public string Parameters { get; set; }
        public ICollection<Event> Events { get; set; }

    }
}
