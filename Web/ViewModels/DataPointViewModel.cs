using System.Collections.Generic;

namespace GrowRoomEnvironment.Web.ViewModels
{
    public class DataPointViewModel : ApplicationViewModelBase
    {
        public int DataPointId { get; set; }
        public string Caption { get; set; }
        public string Icon { get; set; }
        public string Template { get; set; }
        public bool ShowInUI { get; set; }

        public ICollection<EventConditionViewModel> EventConditions { get; set; }
    }
}
