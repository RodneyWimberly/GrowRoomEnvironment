using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class DataPoint : ApplicationEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int DataPointId { get; set; }
        public string Caption { get; set; }
        public string Icon { get; set; }
        public string Template { get; set; }
        public bool ShowInUI { get; set; }

        public virtual ICollection<EventCondition> EventConditions { get; set; }

        public DataPoint()
        {

        }

        public DataPoint(int dataPointId, string caption, string icon, string template, bool showInUI)
        {
            DataPointId = dataPointId;
            Caption = caption;
            Icon = icon;
            Template = template;
            ShowInUI = showInUI;
        }
    }
}
