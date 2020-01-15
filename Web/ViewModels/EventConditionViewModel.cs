using GrowRoomEnvironment.DataAccess.Core.Enums;
using System;

namespace GrowRoomEnvironment.Web.ViewModels
{
    public class EventConditionViewModel : ApplicationViewModelBase
    {
        public int EventConditionId { get; set; }

        public int EventId { get; set; }
        public EventViewModel Event { get; set; }

        public int DataPointId { get; set; }
        public DataPointViewModel DataPoint { get; set; }

        public Operators Operator { get; set; }
        public string OperatorDescription { get; set; }

        public string Value { get; set; }
    }
}
