using AutoMapper;
using FluentValidation;
using LuxMedTest.Application.Commands.Login;
using LuxMedTest.Application.Pipelines;
using LuxMedTest.Application.Profiles;
using LuxMedTest.Application.Services;
using LuxMedTest.Application.Settings;
using LuxMedTest.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LuxMedTest.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
            if (jwtSettings != null)
            {
                services.AddSingleton(jwtSettings);
            }
            else
            {
                jwtSettings = new JwtSettings();
                services.AddSingleton(jwtSettings);
            }
            services.AddSingleton<IRevokedTokenService, RevokedTokenService>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;
        }
    }
}

