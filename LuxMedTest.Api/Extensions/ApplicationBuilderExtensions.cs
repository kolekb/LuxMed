using LuxMedTest.Api.Middlewares;

namespace LuxMedTest.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<CookieAuthenticationMiddleware>();

            return app;
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, bool isDevelopment)
        {
            if (isDevelopment)
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            return app;
        }
    }
}
