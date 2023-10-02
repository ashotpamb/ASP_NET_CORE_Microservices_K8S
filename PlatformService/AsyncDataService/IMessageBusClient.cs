using PlatformService.Dtos;

namespace PlatformService.AsyncDataService 
{
    public interface IMessageBusClient
    {
        void PublishNewMessage(PlatformPublishDto platformPublishDto);
    }
}