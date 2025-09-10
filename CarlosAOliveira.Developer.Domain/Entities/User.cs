using CarlosAOliveira.Developer.Common.Security;
using CarlosAOliveira.Developer.Common.Validation;
using CarlosAOliveira.Developer.Domain.Common;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.Validation;

namespace CarlosAOliveira.Developer.Domain.Entities
{   
    public class User : BaseEntity, IUser
    {       
        public string Username { get; set; } = string.Empty;        
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;     
        public UserRole Role { get; set; }      
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }                
        string IUser.Id => Id.ToString();      
        string IUser.Username => Username;    
        string IUser.Role => Role.ToString();

        public User()
        {
            CreatedAt = DateTime.UtcNow;
        }
        public ValidationResultDetail Validate()
        {
            var validator = new UserValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }
       
        public void Activate()
        {
            Status = UserStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            Status = UserStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }      
        public void Suspend()
        {
            Status = UserStatus.Suspended;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
