namespace CarlosAOliveira.Developer.Domain.Common
{
    /// <summary>
    /// Base entity class with common properties and domain event handling
    /// </summary>
    public abstract class BaseEntity : IComparable<BaseEntity>
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// Collection of domain events
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Adds a domain event to the entity
        /// </summary>
        /// <param name="domainEvent">Domain event to add</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Clears all domain events
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Compares this entity with another by ID
        /// </summary>
        /// <param name="other">Other entity to compare</param>
        /// <returns>Comparison result</returns>
        public int CompareTo(BaseEntity? other)
        {
            if (other == null)
            {
                return 1;
            }

            return Id.CompareTo(other.Id);
        }

        /// <summary>
        /// Determines if this entity equals another by ID
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if equal, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            if (obj is BaseEntity other)
            {
                return Id == other.Id;
            }
            return false;
        }

        /// <summary>
        /// Gets hash code based on ID
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
