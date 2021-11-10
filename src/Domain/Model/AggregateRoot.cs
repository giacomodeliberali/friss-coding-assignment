using System;

namespace Domain.Model
{
    public abstract class AggregateRoot
    {
        public Guid Id { get; internal set; }
    }
}
