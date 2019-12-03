using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class EnumLookupRespository : Repository<EnumLookup>, IEnumLookupRespository
    {
        public EnumLookupRespository(DbContext context) : base(context)
        { }
    }
}
