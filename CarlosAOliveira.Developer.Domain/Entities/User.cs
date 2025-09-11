using CarlosAOliveira.Developer.Common.Security;
using CarlosAOliveira.Developer.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CarlosAOliveira.Developer.Domain.Entities
{   
    /// <summary>
    /// User entity using ASP.NET Core Identity
    /// </summary>
    public class User : IdentityUser<Guid>, IUser
    {       
        public UserRole Role { get; set; }      
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }                
        
        string IUser.Id => Id.ToString();      
        string IUser.Username => UserName ?? string.Empty;    
        string IUser.Role => Role.ToString();

        public User()
        {
            CreatedAt = DateTime.UtcNow;
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
