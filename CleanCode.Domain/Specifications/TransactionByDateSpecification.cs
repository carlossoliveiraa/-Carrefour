using CleanCode.Domain.Entities;

namespace CleanCode.Domain.Specifications
{
    /// <summary>
    /// Specification para buscar transações por data
    /// </summary>
    public class TransactionByDateSpecification : ISpecification<Transaction>
    {
        private readonly DateTime _date;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="date">Data das transações</param>
        public TransactionByDateSpecification(DateTime date)
        {
            _date = date.Date;
        }

        /// <summary>
        /// Verifica se a transação atende ao critério
        /// </summary>
        /// <param name="transaction">Transação a ser verificada</param>
        /// <returns>True se a transação é do dia especificado</returns>
        public bool IsSatisfiedBy(Transaction transaction)
        {
            return transaction.TransactionDate.Date == _date;
        }
    }
}
