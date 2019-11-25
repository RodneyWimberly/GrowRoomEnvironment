using System.Threading;
using GrowRoomEnvironment.DataAccess.Repositories;
using GrowRoomEnvironment.DataAccess.Repositories.Interfaces;
using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrowRoomEnvironment.DataAccess
{
    public class UnitOfWorkTransactionFactory : IUnitOfWork
    {
        public int AutoDisposeTimerInterval { get; set; } = 1500;
        public Timer DisposeTimer { get; set; }
        public DbContextOptions<ApplicationDbContext> DbContextOptions { get; set; }
        public ApplicationDbContext DbContext { get; set; }

        public UnitOfWorkTransactionFactory(DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            DbContextOptions = dbContextOptions;
        }
            
        public IEnumLookupRespository EnumLookups
        {
            get
            {
                IEnumLookupRespository repository = new EnumLookupRespository(CreateDbContext());
                return repository;
            }
        }          
        
        public IDataPointRepository DataPoints
        {
            get
            {
                IDataPointRepository repository = new DataPointRepository(CreateDbContext());
                return repository;
            }
        }

        public int SaveChanges()
        {
            int returnValue = 0;
            CreateDbContext();
            returnValue = DbContext.SaveChanges();
            DisposeDbContext();
            return returnValue;
        }

        private ApplicationDbContext CreateDbContext()
        {
            if (DbContext == null)
            {
                if(AutoDisposeTimerInterval > 0)
                    DisposeTimer = new Timer(state => DisposeDbContext(), null, AutoDisposeTimerInterval, Timeout.Infinite);
                DbContext = new ApplicationDbContext(DbContextOptions);
            }

            return DbContext;
        }

        private void DisposeDbContext()
        {
            if (DisposeTimer != null)
            {
                DisposeTimer.Dispose();
                DisposeTimer = null;
            }
            if (DbContext != null)
            {
                DbContext.Dispose();
                DbContext = null;
            }
        }
    }
}
