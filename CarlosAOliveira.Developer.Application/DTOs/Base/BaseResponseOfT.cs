namespace CarlosAOliveira.Developer.Application.DTOs.Base
{
    public class BaseResponse<T> : BaseResponse
    {
        public T? Data { get; set; }

        public static BaseResponse<T> CreateSuccess(T data, string? message = null)
        {
            return new BaseResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static new BaseResponse<T> CreateError(string message, List<string>? errors = null)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
