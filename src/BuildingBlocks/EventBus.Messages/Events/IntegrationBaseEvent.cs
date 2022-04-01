using System;

namespace EventBus.Messages.Events
{
    public class IntegrationBaseEvent
    {
        public IntegrationBaseEvent()
        {
            CorrelationId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public IntegrationBaseEvent(Guid correlationId, DateTimeOffset creationDate)
        {
            CorrelationId = correlationId;
            CreationDate = creationDate;
        }

        public Guid CorrelationId { get; private set; }
        public DateTimeOffset CreationDate { get; private set; }
    }
}
