using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using UserService.Feature.AppUser;
using UserService.Feature.Authentication;
using HealthCheckService = UserService.Feature.HealthCheck.HealthCheckService;

[assembly: InternalsVisibleTo("AuthenticationService.Tests")]
namespace UserService;

using HealthCheckService = HealthCheckService;

public class Program
{
    private const string Name = "UserService";
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add asp.net & prometheus services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // Http-Connection to AuthService
        builder.Services.AddHttpClient<IAuthClient, AuthClient>(client =>
        {
            // Address of service e.g. url or kubernetes service name
            client.BaseAddress = new Uri(builder.Configuration["AuthService:BaseUrl"]);
        });
        
        // Custom services
        builder.Services.AddScoped<Feature.AppUser.UserService>();
        
        // Connect to otel-collector to send data
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.SetResourceBuilder(
                            ResourceBuilder.CreateDefault().AddService(serviceName: Name, serviceVersion: System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3))
                            .AddAttributes(new Dictionary<string, object>
                            {
                                { "host.name", Environment.MachineName }
                            })
                        )    
                        .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddProcessInstrumentation()
                        .AddOtlpExporter((opt, reader) =>
                        {
                            opt.Endpoint = new Uri("http://otel-collector:4317"); // OTLP-Endpoint Otel-Collector
                            opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc; // Http for 4318
                            reader.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000; // Important, otherwhise we lose data if intervall is too high
                        });
            }).WithTracing(tracing =>
            {
                tracing.SetResourceBuilder(
                            ResourceBuilder.CreateDefault().AddService(serviceName: Name, serviceVersion: System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3))
                            .AddAttributes(new Dictionary<string, object>
                            {
                                { "host.name", Environment.MachineName }
                            })
                        ) 
                        .AddAspNetCoreInstrumentation(options => { options.RecordException = true; })
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(opt =>
                        {
                            opt.Endpoint = new Uri("http://otel-collector:4317"); // OTLP-Endpoint Otel-Collector
                            opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc; // Http for 4318
                        });
            }).WithLogging(logging =>
                {
                    logging.SetResourceBuilder(
                                ResourceBuilder.CreateDefault().AddService(serviceName: Name, serviceVersion: System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3))
                                .AddAttributes(new Dictionary<string, object>
                                {
                                    { "host.name", Environment.MachineName }
                                })
                            ) 
                            .AddOtlpExporter(opt =>
                            {
                                opt.Endpoint = new Uri("http://otel-collector:4317"); // OTLP-Endpoint Otel-Collector
                                opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc; // Http for 4318
                            });
                }
            );
        
        // Health check
        builder.Services.AddHealthChecks().AddCheck<HealthCheckService>(nameof(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService));

        // Connect to PostgreSQL
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        //builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("teste"));
        
        // Adding Authentication & JWT-Bearer
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],  // Access config files and insert value
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            });

        // Conditional logic for development environment (optional)
        if (builder.Environment.IsDevelopment())
        {
            // Disable cors by allowing from all originsAdd commentMore actions
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin() 
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            
            builder.Services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo { Title = "User-API", Version = "v1" });
                
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });
        }

        // Configure the HTTP request pipeline
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Create the database if it doesn't exist
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var authClient = scope.ServiceProvider.GetRequiredService<IAuthClient>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
            
            dbContext.Database.EnsureDeleted();  // Be careful with EnsureDeleted(), it deletes the database
            dbContext.Database.EnsureCreated();  // Ensures the database is created

            try
            {
                AppDbContext.SeedAsync(logger, dbContext, authClient).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                logger.LogError(e.StackTrace);
                throw;
            }
        }
        
        // 1. HTTPS
        app.UseHttpsRedirection();

        // 2. Static files
        app.UseDefaultFiles();   
        app.UseStaticFiles();     // CSS/JS/Pictures

        // 3. Routing & CORS
        app.UseRouting();
        app.UseCors("AllowAll");

        // 4. AuthN & AuthZ
        app.UseAuthentication();
        app.UseAuthorization();

        // 5. API-Endpoints
        app.MapControllers();
        app.MapHealthChecks("/health");

        // 6. SPA‑Fallback for static files
        app.MapFallbackToFile("index.html");
        
        app.Run();
    }
}