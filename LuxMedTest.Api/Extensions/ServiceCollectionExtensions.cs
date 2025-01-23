namespace LuxMedTest.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, bool isDevelopment)
        {
            if (isDevelopment)
            {
                services.AddSwaggerGen();
            }
            return services;
        }
    }
}
