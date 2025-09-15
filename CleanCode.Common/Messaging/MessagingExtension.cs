using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanCode.Common.Messaging
{
    /// <summary>
    /// Extensões para configuração do serviço de mensageria
    /// </summary>
    public static class MessagingExtension
    {
        /// <summary>
        /// Adiciona o serviço de mensageria baseado em arquivos
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="configuration">Configuração da aplicação</param>
        /// <returns>Coleção de serviços</returns>
        public static IServiceCollection AddFileMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MessagingOptions>(configuration.GetSection("Messaging"));
            services.AddScoped<IMessageService, FileMessageService>();
            
            return services;
        }
    }

    /// <summary>
    /// Opções de configuração para o serviço de mensageria
    /// </summary>
    public class MessagingOptions
    {
        /// <summary>
        /// Caminho base para os arquivos de fila
        /// </summary>
        public string BasePath { get; set; } = "queues";

        /// <summary>
        /// Tamanho máximo do arquivo em bytes
        /// </summary>
        public long MaxFileSize { get; set; } = 1024 * 1024; // 1MB

        /// <summary>
        /// Número máximo de arquivos por fila
        /// </summary>
        public int MaxFilesPerQueue { get; set; } = 1000;
    }
}
