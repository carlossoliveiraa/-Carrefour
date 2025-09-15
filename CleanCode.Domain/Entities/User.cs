using CleanCode.Common.Security.Interfaces;
using CleanCode.Common.Validation;
using CleanCode.Domain.Common;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Validation;

namespace CleanCode.Domain.Entities
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
        string IUser.Email => Email;
        string IUser.Phone => Phone;
        string IUser.Role => Role.ToString();
        string IUser.Status => Status.ToString();
        string IUser.CreatedAt => CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ");
        string IUser.UpdatedAt => UpdatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "";   
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
