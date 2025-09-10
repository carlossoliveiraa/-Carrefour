namespace CarlosAOliveira.Developer.Application.DTOs.User
{
    /// <summary>
    /// User DTO
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// User username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User phone
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// User role
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// User status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
