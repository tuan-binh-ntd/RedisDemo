using System.Text.Json;
using RedisAPI.Model;
using StackExchange.Redis;

namespace RedisAPI.Data
{
    public class RedisPlatformRepo : IPlatformRepo
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisPlatformRepo(
            IConnectionMultiplexer redis
        )
        {
            _redis = redis;
        }

        public void CreatePlatform(Platform plat)
        {
            if (plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }
            var db = _redis.GetDatabase();
            var serialPlat = JsonSerializer.Serialize(plat);

            // db.StringSet(plat.Id, serialPlat);
            // db.SetAdd("PlatformSet", serialPlat);

            db.HashSet("HashPlatform", new HashEntry[]
            {new HashEntry(plat.Id, serialPlat)});
        }

        public IEnumerable<Platform?>? GetAllPlatforms()
        {
            var db = _redis.GetDatabase();
            // Use Set in Redis
            // var completeSet = db.SetMembers("PlatformSet");

            // if (completeSet.Length > 0)
            // {
            //     var obj = Array.ConvertAll(completeSet, val => JsonSerializer.Deserialize<Platform>(val)).ToList();
            //     return obj;
            // }

            //User Hash in Redis
            var completeHash = db.HashGetAll("HashPlatform");

            if (completeHash.Length > 0)
            {
                var obj = Array.ConvertAll(completeHash, val => JsonSerializer.Deserialize<Platform>(val.Value)).ToList();
                return obj;
            }
            
            return null;
        }

        public Platform? GetPlatformById(string id)
        {
            var db = _redis.GetDatabase();

            //Use Set in Redis
            //var plat = db.StringGet(id);

            //Use Hash in Redis
            var plat = db.HashGet("HashPlatform", id);

            if (!string.IsNullOrEmpty(plat))
            {
                return JsonSerializer.Deserialize<Platform>(plat);
            }

            return null;
        }
    }
}