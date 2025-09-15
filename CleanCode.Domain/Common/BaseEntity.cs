using CleanCode.Domain.Interface;

namespace CleanCode.Domain.Common
{
    public abstract class BaseEntity : IComparable<BaseEntity>
    {
        public Guid Id { get; set; }
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
               
        public int CompareTo(BaseEntity? other)
        {
            if (other == null)
            {
                return 1;
            }

            return other!.Id.CompareTo(Id);
        }
    }
}
