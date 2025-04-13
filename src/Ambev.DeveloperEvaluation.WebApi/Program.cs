using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");
            
            foreach (var envVar in Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>())
            {
                Log.Information($"Environment Variable: {envVar.Key} = {envVar.Value}");
            }

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
                )
            );

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.RegisterDependencies();

            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            builder.WebHost.UseUrls("http://0.0.0.0:8080");

            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                var connection = db.Database.GetDbConnection();

                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetry(
                        retryCount: 10,
                        sleepDurationProvider: attempt => TimeSpan.FromSeconds(5),
                        onRetry: (exception, timeSpan, retryCount, context) =>
                        {
                            Log.Warning(exception, $"Tentativa {retryCount} falhou. Tentando novamente em {timeSpan.TotalSeconds} segundos...");
                        });

                retryPolicy.Execute(() =>
                {
                    Log.Information("Testando conexão com o banco...");
                    try
                    {
                        connection.Open();
                        connection.Close();
                        Log.Information("Conexão com o banco bem-sucedida!");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Erro ao testar a conexão com o banco.");
                        throw;
                    }

                    Log.Information("Aplicando migrations pendentes...");
                    try
                    {
                        db.Database.Migrate();
                        Log.Information("Migrations aplicadas com sucesso!");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Erro ao aplicar as migrations.");
                        throw;
                    }
                });
            }

            app.UseMiddleware<ValidationExceptionMiddleware>();

            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
