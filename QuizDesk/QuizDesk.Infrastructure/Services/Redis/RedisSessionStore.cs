using QuizDesk.Application.Abstractions;
using QuizDesk.Application.DTOs.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace QuizDesk.Infrastructure.Services.Redis
{
    public class RedisSessionStore : ISessionStore
    {
        private readonly IDatabase _db;

        public RedisSessionStore(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        public async Task<SessionData?> GetAsync(string provider, int userId)
        {
             var value = await _db.StringGetAsync(
            $"oauth:session:{provider}:{userId}");

        return value.IsNull
            ? null
            : JsonSerializer.Deserialize<SessionData>(value!);
        }

        public Task RemoveAsync(string provider, int userId)
        {
            return _db.KeyDeleteAsync(
            $"oauth:session:{provider}:{userId}");
        }

        public async Task SaveAsync(SessionData session)
        {
            var key = $"oauth:session:{session.Provider}:{session.UserId}";
            var ttl = session.ExpiresAt - DateTimeOffset.UtcNow;

            await _db.StringSetAsync(
                key,
                JsonSerializer.Serialize(session),
                ttl
            );
        }
    }
}
