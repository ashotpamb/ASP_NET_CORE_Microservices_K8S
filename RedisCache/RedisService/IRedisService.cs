namespace RedisCacahe.RedisService
{
    public interface IRedisService
    {
        T GetData<T>(string key);
        T SetData<T>(string key, T value, DateTimeOffset expirationTime);
        object RemoveData(string key);
        // public void ConnectToRedis(IConfiguration configuration);
    }
}