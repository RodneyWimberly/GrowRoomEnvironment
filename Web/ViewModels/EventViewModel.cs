using GrowRoomEnvironment.DataAccess.Core.Enums;
using System.Collections.Generic;

namespace GrowRoomEnvironment.Web.ViewModels
{
    public class EventViewModel : AuditableViewModelBase
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public int ActionDeviceId { get; set; }
        public ActionDeviceViewModel ActionDevice { get; set; }
        public ActionDeviceStates State { get; set; }

        public ICollection<EventConditionViewModel> EventConditions { get; set; }
    }
}
