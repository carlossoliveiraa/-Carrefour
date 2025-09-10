namespace CarlosAOliveira.Developer.Application.DTOs.Base
{   
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static BaseResponse CreateSuccess(string? message = null)
        {
            return new BaseResponse
            {
                Success = true,
                Message = message
            };
        }

        public static BaseResponse CreateError(string message, List<string>? errors = null)
        {
            return new BaseResponse
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}