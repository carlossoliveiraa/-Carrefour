using FluentValidation.Results;

namespace CleanCode.Common.Validation
{
    /// <summary>
    /// Representa um detalhe de erro de validação.
    /// Contém informações sobre a propriedade, o código de erro e a mensagem detalhada.
    /// </summary>
    public class ValidationErrorDetail
    {
        /// <summary>
        /// Nome da propriedade que falhou na validação.
        /// </summary>
        public string PropertyName { get; init; } = string.Empty;

        /// <summary>
        /// Código do erro de validação (por exemplo, "NotEmptyValidator").
        /// </summary>
        public string Error { get; init; } = string.Empty;

        /// <summary>
        /// Mensagem detalhada do erro de validação.
        /// </summary>
        public string Detail { get; init; } = string.Empty;

        /// <summary>
        /// Converte uma instância de <see cref="ValidationFailure"/> do FluentValidation
        /// em um objeto <see cref="ValidationErrorDetail"/>.
        /// </summary>
        /// <param name="validationFailure">
        /// O objeto <see cref="ValidationFailure"/> que será convertido.
        /// </param>
        /// <returns>
        /// Uma nova instância de <see cref="ValidationErrorDetail"/> com as informações extraídas.
        /// </returns>
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
