using Infomatrix.Api.Abstractions.Services;
using Infomatrix.Api.Data;
using Infomatrix.Api.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StarterApp.Infrastructure.Auth;
using System.Text;
using AppSupabaseOptions = StarterApp.Infrastructure.Auth.SupabaseOptions;
using SupabaseClient = Supabase.Client;
using SupabaseOptions = Supabase.SupabaseOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("HYSDb"));

builder.Services.AddScoped(
    typeof(IRepository<>),
    typeof(Repository<>));

builder.Services.AddServices(builder.Configuration);


var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "HYS API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AppSupabaseOptions>(
            configuration.GetSection("Supabase"));

        services.AddSingleton(_ =>
            new SupabaseClient(
                configuration["Supabase:Url"]!,
                configuration["Supabase:Key"],
                new SupabaseOptions
                {
                    AutoRefreshToken = false,
                }));

        services.AddScoped<IAuthService, AuthService>();

        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                var bytes = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(bytes),
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidIssuer = configuration["Jwt:Issuer"],
                };
            });

        services.AddAuthorization();

        return services;
    }
}
