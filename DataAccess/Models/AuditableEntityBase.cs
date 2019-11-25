using GrowRoomEnvironment.Contracts.DataAccess;
using System;
using System.ComponentModel.DataAnnotations;

namespace GrowRoomEnvironment.DataAccess.Models
{
    public abstract class AuditableEntityBase : IAuditableEntity
    {
        [MaxLength(256)]
        public string CreatedBy { get; set; }
        [MaxLength(256)]
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
