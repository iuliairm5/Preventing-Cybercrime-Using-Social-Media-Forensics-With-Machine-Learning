using DisertatieIRIMIA.Data;

namespace DisertatieIRIMIA.Models
{
    public class EFStoreRepository2 : IStoreRepository2
    {
        private ApplicationDbContext2 context;

        public EFStoreRepository2(ApplicationDbContext2 ctx)
        {
            context = ctx;
        }

        
        ////////////////////////////////////////////////////
        ///

        public IQueryable<MonitorizedUser> MonitorizedUsers
        {
            get
            {
                return context.MonitorizedUsers;
            }
        }

        public async Task SavePredictionAsync(MonitorizedUser monitorizedUser)
        {

            context.MonitorizedUsers.Add(monitorizedUser);


            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MonitorizedUser monitorizedUser)
        {

            context.MonitorizedUsers.Update(monitorizedUser);
            await context.SaveChangesAsync();
        }

        public IQueryable<MonitorizedUser> MonitorizationforUser(string userId)
        {
            return context.MonitorizedUsers.Where(p => p.UserId == userId);
        }


        public async Task<MonitorizedUser> DeleteUserAsync(int monitorizationID)
        {
            MonitorizedUser dbEntry = context.MonitorizedUsers
                    .FirstOrDefault(p => p.MonitorizationID == monitorizationID);

            if (dbEntry != null)
            {
                context.MonitorizedUsers.Remove(dbEntry);
                await context.SaveChangesAsync();
            }

            return dbEntry;
        }

    }
}
