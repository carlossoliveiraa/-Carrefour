using FluentValidation.Results;

namespace CarlosAOliveira.Developer.Common.Validation
{
    public class ValidationErrorDetail
    {        
        public string PropertyName { get; init; } = string.Empty;      
        public string Error { get; init; } = string.Empty;
        public string Detail { get; init; } = string.Empty;
       
        public static explicit operator ValidationErrorDetail(ValidationFailure validationFailure)
        {
            return new ValidationErrorDetail
            {
                PropertyName = validationFailure.PropertyName,
                Error = validationFailure.ErrorCode,
                Detail = validationFailure.ErrorMessage
            };
        }
    }
}
