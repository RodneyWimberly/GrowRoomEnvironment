using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public abstract class ApplicationEntityBase : IAuditableEntity, IConcurrencyTrackingEntity
    {
        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
