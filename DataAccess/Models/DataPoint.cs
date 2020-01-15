using GrowRoomEnvironment.DataAccess.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public class DataPoint : ApplicationEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DataPointId { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public DataPointDataTypes DataType { get; set; }
        public virtual ICollection<EventCondition> EventConditions { get; set; }

        public DataPoint()
        {

        }

        public DataPoint(int dataPointId, string name, bool isEnabled)
        {
            DataPointId = dataPointId;
            Name = name;
            IsEnabled = isEnabled;
        }
    }
}
