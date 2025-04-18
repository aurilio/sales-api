using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using Ambev.DeveloperEvaluation.WebApi.Swagger;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ambev Developer Evaluation API",
                    Version = "v1",
                    //Version = "1.0.0",
                    //Version = "3.0.1",
                    Description = "API para gerenciamento de vendas",
                });
                c.UseAllOfToExtendReferenceSchemas();
                c.UseInlineDefinitionsForEnums();
                c.SupportNonNullableReferenceTypes();
                c.CustomSchemaIds(type => type.FullName);
                c.EnableAnnotations();
                c.DescribeAllParametersInCamelCase();
                c.OperationFilter<DynamicFilterOperationFilter>();
            });

            builder.Services.AddHttpContextAccessor();

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

            if (!builder.Environment.IsDevelopment())
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


            //app.UseSwagger();
             app.UseSwagger(c => c.SerializeAsV2 = true);
            //app.UseSwaggerUI();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ambev Developer Evaluation API v1");
                c.RoutePrefix = string.Empty; // Deixa Swagger disponível na raiz
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.UseStaticFiles();
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
