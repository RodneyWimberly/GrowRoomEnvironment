using GrowRoomEnvironment.DataAccess.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class EventCondition : AuditableEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventConditionId { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public int DataPointId { get; set; }
        public DataPoint DataPoint { get; set; }

        public Operators Operator { get; set; }

        public string Value { get; set; }
    }
}
