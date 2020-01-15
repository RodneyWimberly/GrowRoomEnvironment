using GrowRoomEnvironment.DataAccess.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class EventCondition : ApplicationEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventConditionId { get; set; }

        public int EventId { get; set; }
        public virtual Event Event { get; set; }

        public int DataPointId { get; set; }
        public virtual DataPoint DataPoint { get; set; }

        public Operators Operator { get; set; }

        [NotMapped]
        public virtual string OperatorDescription
        {
            get
            {
                try
                {
                    return Enum.GetName(typeof(Operators), Operator);
                }
                catch
                {
                    return null;
                }
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    try
                    {
                        Operator = Enum.Parse<Operators>(value);
                    }
                    catch { }
            }
        }

        public string Value { get; set; }

        public EventCondition()
        {

        }

        public EventCondition(int eventConditionId, int eventId, int dataPointId, Operators @operator, string value)
        {
            EventConditionId = eventConditionId;
            EventId = eventId;
            DataPointId = dataPointId;
            Operator = @operator;
            Value = value;
        }
    }
}
