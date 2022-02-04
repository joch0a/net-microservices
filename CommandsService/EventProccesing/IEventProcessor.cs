namespace CommandsService.EventProccesing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}
