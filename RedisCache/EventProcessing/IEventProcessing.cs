namespace RedisCacahe.EventProcessing
{
    public interface IEventProcessing
    {
        string EventProcess(string eventMessage);
    }
}