﻿using GrowRoomEnvironment.DataAccess.Core.Enums;
using System.Collections.Generic;

namespace GrowRoomEnvironment.Web.ViewModels
{
    public class ActionDeviceViewModel : AuditableViewModelBase
    {
        public int ActionDeviceId { get; set; }
        public string Name { get; set; }
        public ActionDeviceTypes Type { get; set; }
        public string Parameters { get; set; }
        public ICollection<EventViewModel> Events { get; set; }
    }
}
