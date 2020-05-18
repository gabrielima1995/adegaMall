namespace Domain.Core
{
    public abstract class Command
    {
        public string AggregateId { get; private set; }

        public Command(string aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}
