using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(DbContext context) : base(context)
        { }
    }
}
