using System.Text.Json;
using RedisCacahe.Dtos;
using RedisCacahe.Model;
using RedisCacahe.RedisService;

namespace RedisCacahe.EventProcessing
{
    public class EventProcessor : IEventProcessing
    {
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;

        public EventProcessor(IRedisService redisService, IConfiguration configuration)
        {
            _redisService = redisService;
            _configuration = configuration;
        }
        public void EventProcess(string eventMessage)
        {
            var eventType = DermineEvent(eventMessage);

            switch (eventType)
            {
                case EventType.RedisCacheSet:
                    SetRedisCache(eventMessage);
                    break;
                case EventType.RedisCacheGet:
                    GetRedisCache(eventMessage);
                    break;
                default:
                    break;

            }
        }

        private void SetRedisCache(string message)
        {
            _redisService.SetData("test", "test", DateTimeOffset.Now.AddSeconds(30));
        }

        private void GetRedisCache(string message)
        {
            Console.WriteLine("--> Get data from readis");

        }

        private EventType DermineEvent(string publishedMessage)
        {
            Console.WriteLine("--> Determine Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDtos>(publishedMessage);
            switch (eventType.Event)
            {
                case "Cache_in":
                    Console.WriteLine("--> Cacheing in Redis");
                    return EventType.RedisCacheSet;
                case "Cache_out":
                    Console.WriteLine("--> Get cache from redis");
                    return EventType.RedisCacheGet;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    return EventType.Undetermined;
            }
        }
        enum EventType
        {
            RedisCacheSet,
            RedisCacheGet,
            Undetermined
        }
    }
}