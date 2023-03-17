using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MSGMicroservice.IDP.Common;
using MSGMicroservice.IDP.Infrastructure.Entities;
using MSGMicroservice.IDP.Persistence;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace MSGMicroservice.IDP.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
            IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting))
                .Get<SMTPEmailSetting>();
            services.AddSingleton(emailSettings);

            return services;
        }

        internal static void AddAppConfigurations(this ConfigureHostBuilder host)
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            });
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    //.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()); //will be adjusted after production
            });
        }

        public static void ConfigureSerilog(this ConfigureHostBuilder host)
        {
            host.UseSerilog((context, configuration) =>
            {
                // var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
                // var username = context.Configuration.GetValue<string>("ElasticConfiguration:Username");
                // var password = context.Configuration.GetValue<string>("ElasticConfiguration:Password");
                var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-");

                // if (string.IsNullOrEmpty(elasticUri))
                //     throw new Exception("ElasticConfiguration Uri is not configured");

                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Debug()
                    .WriteTo.Console().ReadFrom.Configuration(context.Configuration)
                    // .WriteTo.Elasticsearch(
                    //     new ElasticsearchSinkOptions(new Uri(elasticUri))
                    //     {
                    //         IndexFormat =
                    //             $"{applicationName}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".","-")}-{DateTime.Now:yyyy-MM}",
                    //         AutoRegisterTemplate = true,
                    //         NumberOfShards = 2,
                    //         NumberOfReplicas = 1,
                    //         ModifyConnectionSettings = x => x.BasicAuthentication(username, password)
                    //     })
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", applicationName)
                    .ReadFrom.Configuration(context.Configuration);
            });
        }

        public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
            services.AddIdentityServer(options =>
                {
                    options.EmitStaticAudienceClaim = true;
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                //not recommended for product to secure your key
                .AddDeveloperSigningCredential()
                // .AddInMemoryIdentityResources(Config.IdentityResources)
                // .AddInMemoryApiScopes(Config.ApiScopes)
                // .AddInMemoryClients(Config.Clients)
                // .AddInMemoryApiResources(Config.ApiResources)
                // .AddTestUsers(TestUsers.Users)
                .AddConfigurationStore(opt =>
                {
                    opt.ConfigureDbContext = c => c.UseSqlServer(
                        connectionString,
                        builder => builder.MigrationsAssembly("MSGMicroservice.IDP"));
                })
                .AddOperationalStore(opt =>
                {
                    opt.ConfigureDbContext = c => c.UseSqlServer(
                        connectionString,
                        builder => builder.MigrationsAssembly("MSGMicroservice.IDP"));
                })
                .AddAspNetIdentity<User>()
                .AddProfileService<IdentityProfileService>()
                ;
        }

        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("IdentitySqlConnection");
            services
                .AddDbContext<MsgIdentityContext>(options => options
                    .UseSqlServer(connectionString, b => b.MigrationsAssembly("MSGMicroservice.IDP")))
                .AddIdentity<User, IdentityRole>(opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequiredLength = 6;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireLowercase = false;
                    opt.User.RequireUniqueEmail = true;
                    opt.Lockout.AllowedForNewUsers = true;
                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    opt.Lockout.MaxFailedAccessAttempts = 3;
                })
                .AddEntityFrameworkStores<MsgIdentityContext>()
                .AddUserStore<MsgUserStore>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MSG Identity Server API",
                    Version = "1.0",
                    Contact = new OpenApiContact
                    {
                        Name = "MSG Identity Service",
                        Email = "msg119@matsaigon.com",
                        Url = new Uri("https://matsaigon.com")
                    }
                });

                var identityServerBaseUrl = configuration.GetSection("IdentityServer:BaseUrl").Value;

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{identityServerBaseUrl}/connect/authorize"),
                            Scopes = new Dictionary<string, string>()
                            {
                                {"msg_microservice_api.read", "MSG Microservice API Read Scope"},
                                {"msg_microservice_api.write", "MSG Microservice API Write Scope"}
                            }
                        }
                    }
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                        },
                        new List<string>
                        {
                            "msg_microservice_api.read",
                            "msg_microservice_api.write"
                        }
                    }
                });
            });
        }

        public static void ConfigurationAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication()
                .AddLocalApi("Bearer", option => { option.ExpectedScope = "msg_microservice_api.read"; });
        }

        public static void ConfigurationAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("Bearer", policy =>
                    {
                        policy.AddAuthenticationSchemes("Bearer");
                        policy.RequireAuthenticatedUser();
                    });
                });
        }
    }
}