using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;

namespace CleanCode.Domain.Specifications
{
    /// <summary>
    /// Specification para buscar transações por tipo
    /// </summary>
    public class TransactionByTypeSpecification : ISpecification<Transaction>
    {
        private readonly TransactionType _type;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="type">Tipo da transação</param>
        public TransactionByTypeSpecification(TransactionType type)
        {
            _type = type;
        }

        /// <summary>
        /// Verifica se a transação atende ao critério
        /// </summary>
        /// <param name="transaction">Transação a ser verificada</param>
        /// <returns>True se a transação é do tipo especificado</returns>
        public bool IsSatisfiedBy(Transaction transaction)
        {
            return transaction.Type == _type;
        }
    }
}
