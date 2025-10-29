using DisertatieIRIMIA.Models;

namespace DisertatieIRIMIA.Data
{
    public interface IStoreRepository2
    {
        
        IQueryable<MonitorizedUser> MonitorizedUsers { get; }

        Task SavePredictionAsync(MonitorizedUser monitorizedUser);
        Task UpdateAsync(MonitorizedUser monitorizedUser);

        IQueryable<MonitorizedUser> MonitorizationforUser(string userId);
        Task<MonitorizedUser> DeleteUserAsync(int monitorizationID);
    }
}
