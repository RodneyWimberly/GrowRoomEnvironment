using System;

namespace GrowRoomEnvironment.Web.ViewModels
{
    public class ExtendedLogViewModel : AuditableViewModelBase
    {
        public string Browser { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string User { get; set; }
        public int EventId { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
