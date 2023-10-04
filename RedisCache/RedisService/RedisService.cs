using StackExchange.Redis;

namespace RedisCacahe.RedisService
{
    public class RedisService : IRedisService
    {

        private IDatabase _redisDb;
        private readonly IConfiguration _configuration;

        public RedisService(IConfiguration configuration)
        {
            _configuration = configuration;
            var redis = ConnectionMultiplexer.Connect(_configuration["RedisConnection"]);
            _redisDb = redis.GetDatabase();

        }

        public T GetData<T>(string key)
        {
            throw new NotImplementedException();
        }

        public object RemoveData(string key)
        {
            throw new NotImplementedException();
        }


        public T SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            Console.WriteLine("hello from redis set key");
            return default;
        }
    }
}