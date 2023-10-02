using System.Text.Json;
using AutoMapper;
using CommandsService.Dtos;

namespace CommandsService.EventProcessing
{
    public class EventProcess : IEventProcessing
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcess(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determine Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDtos>(notificationMessage);
            switch (eventType.Event)
            {
                case "Plaform_Publish":
                    Console.WriteLine("--> Platform publshed Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    return EventType.Undetermined;
            }
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch(eventType)
            {
                case EventType.PlatformPublished:
                    break;
                default:
                    break;
            }
        }

        enum EventType
        {
            PlatformPublished,
            Undetermined
        }
    }
}