using System.Text.Json;
using RedisCacahe.Dtos;

namespace RedisCacahe.EventProcessing
{
    public class EventProcessor : IEventProcessing
    {
        public void EventProcess(string eventMessage)
        {
            var eventType = DermineEvent(eventMessage);
            
            switch (eventType)
            {
                case EventType.RedisCacaheSet:
                    SetRedisCache(eventMessage);
                    break;
                case EventType.RedisCacaheGet:
                    GetRedisCache(eventMessage);
                    break;
                default:
                    break;

            }
        }

        private void SetRedisCache(string message)
        {
            Console.WriteLine("--> Set data to readis");
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
                    return EventType.RedisCacaheSet;
                case "Cache_out":
                    Console.WriteLine("--> Get cache from redis");
                    return EventType.RedisCacaheGet;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    return EventType.Undetermined;
            }
        }
        enum EventType
        {
            RedisCacaheSet,
            RedisCacaheGet,
            Undetermined
        }
    }
}