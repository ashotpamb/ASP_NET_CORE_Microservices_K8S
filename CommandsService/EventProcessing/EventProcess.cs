using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

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

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    break;
                default:
                    break;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var PlatformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDtos>(platformPublishedMessage);
                try
                {
                    var plat = _mapper.Map<Platform>(PlatformPublishedDto);
                    if (!repo.ExternalPlatformExist(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("--> Platform alredy exists...");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
                }
            }
        }

        enum EventType
        {
            PlatformPublished,
            Undetermined
        }
    }
}