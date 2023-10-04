namespace RedisCacahe.EventProcessing
{
    public interface IEventProcessing
    {
        public void EventProcess(string eventMessage);
    }
}