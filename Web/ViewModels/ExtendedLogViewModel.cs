using GrowRoomEnvironment.DataAccess.Core.Enums;
using System;

namespace GrowRoomEnvironment.Web.ViewModels
{
    public class ExtendedLogViewModel : ApplicationViewModelBase
    {
        public string Browser { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string User { get; set; }
        public int EventId { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
        public string LevelDescription { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public int StatusCode { get; set; }
        public string ServerVariables { get; set; }
        public string Cookies { get; set; }
        public string FormVariables { get; set; }
        public string QueryString { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
