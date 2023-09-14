using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Azure;

namespace API.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<IFileService, FileService>();
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(config["Azure:Storage:ConnectionString"]);
        });

        return services;
    }
}
