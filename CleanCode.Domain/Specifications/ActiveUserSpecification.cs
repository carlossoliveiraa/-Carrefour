using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;

namespace CleanCode.Domain.Specifications
{
    public class ActiveUserSpecification : ISpecification<User>
    {
        public bool IsSatisfiedBy(User user)
        {
            return user.Status == UserStatus.Active;
        }
    }
}
