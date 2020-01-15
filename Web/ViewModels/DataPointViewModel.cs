using System.Collections.Generic;

namespace GrowRoomEnvironment.Web.ViewModels
{
    public class DataPointViewModel : ApplicationViewModelBase
    {
        public int DataPointId { get; set; }
        public string Name { get; set; }
        public bool IsEnabled { get; set; }

        public ICollection<EventConditionViewModel> EventConditions { get; set; }
    }
}
