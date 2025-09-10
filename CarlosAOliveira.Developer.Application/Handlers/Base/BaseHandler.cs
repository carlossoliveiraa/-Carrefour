using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Base
{
    /// <summary>
    /// Base handler class with common functionality
    /// </summary>
    public abstract class BaseHandler
    {
        protected readonly IMapper Mapper;

        protected BaseHandler(IMapper mapper)
        {
            Mapper = mapper;
        }

        /// <summary>
        /// Creates a successful response
        /// </summary>
        protected static BaseResponse<T> Success<T>(T data, string? message = null)
        {
            return BaseResponse<T>.CreateSuccess(data, message);
        }

        /// <summary>
        /// Creates an error response
        /// </summary>
        protected static BaseResponse<T> Error<T>(string message, List<string>? errors = null)
        {
            return BaseResponse<T>.CreateError(message, errors);
        }

        /// <summary>
        /// Creates a successful response without data
        /// </summary>
        protected static BaseResponse Success(string? message = null)
        {
            return BaseResponse.CreateSuccess(message);
        }

        /// <summary>
        /// Creates an error response without data
        /// </summary>
        protected static BaseResponse Error(string message, List<string>? errors = null)
        {
            return BaseResponse.CreateError(message, errors);
        }
    }
}
