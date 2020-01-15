

using System.ComponentModel.DataAnnotations;

namespace GrowRoomEnvironment.DataAccess.Core.Interfaces
{
    public interface IConcurrencyTrackingEntity
    {
        [Timestamp]
        byte[] RowVersion { get; set; }
    }
}
