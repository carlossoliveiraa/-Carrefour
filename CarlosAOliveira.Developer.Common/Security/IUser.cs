namespace CarlosAOliveira.Developer.Common.Security
{
    public interface IUser
    {        
        public string Id { get; }       
        public string Username { get; }        
        public string Role { get; }
    }
}
