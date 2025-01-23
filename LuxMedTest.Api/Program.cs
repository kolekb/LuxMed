using Serilog;
using LuxMedTest.Infrastructure.Extensions;
using LuxMedTest.Application.Extensions;
using LuxMedTest.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container
var isDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger(isDevelopment);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(isDevelopment);

//app
var app = builder.Build();

await app.UseAutoMigrationAsync();

app.UseSwagger(isDevelopment);

if (builder.Environment.IsDevelopment())
{
    app.UseCors(x => x
    .SetIsOriginAllowed(_=> true)
    .AllowCredentials()
    .AllowAnyMethod()
    .AllowAnyHeader());
}
app.UseCustomMiddlewares();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();

public partial class Program { }