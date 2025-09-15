namespace CleanCode.Common.Messaging.Interfaces
{
    /// <summary>
    /// Interface para serviço de mensageria básico
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Envia uma mensagem para a fila
        /// </summary>
        /// <param name="message">Conteúdo da mensagem</param>
        /// <param name="queueName">Nome da fila</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Task representando a operação assíncrona</returns>
        Task SendMessageAsync(string message, string queueName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Envia uma mensagem estruturada para a fila
        /// </summary>
        /// <typeparam name="T">Tipo da mensagem</typeparam>
        /// <param name="message">Objeto da mensagem</param>
        /// <param name="queueName">Nome da fila</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Task representando a operação assíncrona</returns>
        Task SendMessageAsync<T>(T message, string queueName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lê mensagens da fila
        /// </summary>
        /// <param name="queueName">Nome da fila</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de mensagens</returns>
        Task<IEnumerable<string>> ReadMessagesAsync(string queueName, CancellationToken cancellationToken = default);
    }
}
