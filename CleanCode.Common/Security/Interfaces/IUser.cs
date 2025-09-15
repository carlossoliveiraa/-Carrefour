namespace CleanCode.Common.Security.Interfaces
{
    /// <summary>
    /// Define o contrato para representação de um usuário no sistema.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Obtém o identificador único do usuário.
        /// </summary>
        /// <returns>O ID do usuário como uma string.</returns>
        public string Id { get; }

        /// <summary>
        /// Obtém o nome de usuário.
        /// </summary>
        /// <returns>O nome de usuário.</returns>
        public string Username { get; }

        /// <summary>
        /// Obtém o email do usuário.
        /// </summary>
        /// <returns>O email do usuário.</returns>
        public string Email { get; }

        /// <summary>
        /// Obtém o telefone do usuário.
        /// </summary>
        /// <returns>O telefone do usuário.</returns>
        public string Phone { get; }

        /// <summary>
        /// Obtém o papel/função do usuário no sistema.
        /// </summary>
        /// <returns>O papel do usuário como uma string.</returns>
        public string Role { get; }

        /// <summary>
        /// Obtém o status do usuário no sistema.
        /// </summary>
        /// <returns>O status do usuário como uma string.</returns>
        public string Status { get; }

        /// <summary>
        /// Obtém a data de criação do usuário.
        /// </summary>
        /// <returns>A data de criação como string.</returns>
        public string CreatedAt { get; }

        /// <summary>
        /// Obtém a data de atualização do usuário.
        /// </summary>
        /// <returns>A data de atualização como string.</returns>
        public string UpdatedAt { get; }
    }
}
