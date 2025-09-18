using Estoque.Domain.Repository;
using Estoque.Infrastructure.Messaging;
using Estoque.Infrastructure.Persistence.Data;
using Estoque.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging;

namespace Estoque.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Database")!;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IEstoqueRepository, EstoqueRepository>();
            services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

            // Add messaging
            services.AddMessaging(configuration);
            
            // Register message consumers
            services.AddHostedService<StockReservationRequestConsumer>();

            return services;
        }
    }
}
