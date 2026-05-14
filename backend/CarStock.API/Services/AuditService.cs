using CarStock.API.Data;
using CarStock.API.Models;

namespace CarStock.API.Services
{
    public interface IAuditService
    {
        Task LogAsync(int userId, string action, string entityType, int entityId);
    }

    public class AuditService : IAuditService
    {
        private readonly AppDbContext _db;

        public AuditService(AppDbContext db)
        {
            _db = db;
        }

        public async Task LogAsync(int userId, string action, string entityType, int entityId)
        {
            _db.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId
            });
            await _db.SaveChangesAsync();
        }
    }
}
