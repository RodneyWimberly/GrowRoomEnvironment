using GrowRoomEnvironment.DataAccess.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class Event : ApplicationEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }
        public string Name { get; set; }
        public int ActionDeviceId { get; set; }
        public virtual ActionDevice ActionDevice { get; set; }
        public ActionDeviceStates State { get; set; }

        [NotMapped]
        public virtual string StateDescription
        {
            get
            {
                try
                {
                    return Enum.GetName(typeof(ActionDeviceStates), State);
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
                        State = Enum.Parse<ActionDeviceStates>(value);
                    }
                    catch { }
            }
        }
        public virtual ICollection<EventCondition> EventConditions { get; set; }

        public Event()
        {

        }

        public Event(int eventId, string name, int actionDeviceId, ActionDeviceStates state)
        {
            EventId = eventId;
            Name = name;
            ActionDeviceId = actionDeviceId;
            State = state;
        }
    }
}
