namespace CommandsService.EventProcessing
{
    public interface  IEventProcessing
    {
        public void ProcessEvent(string message);
    }
}