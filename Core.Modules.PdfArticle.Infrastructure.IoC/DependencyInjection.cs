using Core.Modules.PdfArticle.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Modules.PdfArticle.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreModulesPdfArticleInfrastructureDependencies(this IServiceCollection services)
        {
            services
                .AddScoped<IPdfArticleAdapter, PdfArticleAdapterQuestPdf>();
            return services;
        }
    }
}