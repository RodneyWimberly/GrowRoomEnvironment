using GrowRoomEnvironment.DataAccess.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class ActionDevice : ApplicationEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionDeviceId { get; set; }
        public string Name { get; set; }
        public ActionDeviceTypes Type { get; set; }

        [NotMapped]
        public virtual string TypeDescription
        {
            get
            {
                try
                {
                    return Enum.GetName(typeof(ActionDeviceTypes), Type);
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
                        Type = Enum.Parse<ActionDeviceTypes>(value);
                    }
                    catch { }
            }
        }

        public string Parameters { get; set; }
        public virtual ICollection<Event> Events { get; set; }

        public ActionDevice()
        {

        }

        public ActionDevice(int actionDeviceId, string name, ActionDeviceTypes type, string parameters)
        {
            ActionDeviceId = actionDeviceId;
            Name = name;
            Type = type;
            Parameters = parameters;
        }

    }
}
