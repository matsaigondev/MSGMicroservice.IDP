using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSGMicroservice.IDP.Common.Systems;
using MSGMicroservice.IDP.Infrastructure.Domains;
using MSGMicroservice.IDP.Infrastructure.Repositories;
using MSGMicroservice.IDP.Infrastructure.Repositories.Interfaces;
using MSGMicroservice.IDP.Presentation;
using MSGMicroservice.IDP.Services.EmailService;
using Serilog;

namespace MSGMicroservice.IDP.Extensions
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            // uncomment if you want to add a UI
            builder.Services.AddRazorPages();

            //add services to the container
            builder.Services.AddConfigurationSettings(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddScoped<IEmailSender, SmtpMailService>();
            builder.Services.ConfigureCookiePolicy(); //khi nao ko login cung 1 trang
            builder.Services.ConfigureCors();
            //builder.Services.AddSingleton<ICorsPolicyService>((container) => {
            //    var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
            //    return new DefaultCorsPolicyService(logger)
            //    {
            //        AllowedOrigins = { "http://localhost:3002" }
            //    };
            //});
            builder.Services.ConfigureIdentity(builder.Configuration);
            builder.Services.ConfigureIdentityServer(builder.Configuration);
            builder.Services.AddSingleton<DapperContext>();
            builder.Services.AddTransient(typeof(IUnitOfWork),
                typeof(UnitOfWork));
            builder.Services.AddTransient(typeof(IRepositoryBase<,>),
                typeof(RepositoryBase<,>));
            
            builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                config.Filters.Add(new ProducesAttribute("application/json", "text/plain", "text/json"));
            }).AddApplicationPart(typeof(AssemblyReference).Assembly);
            builder.Services.ConfigurationAuthentication();
            builder.Services.ConfigurationAuthorization();
            builder.Services.ConfigureSwagger(builder.Configuration);
            builder.Services.Configure<RouteOptions>(option => option.LowercaseUrls = true);

            // DriveInfoHelper.CheckDriveInfo();
            builder.Services.AddScoped<IRepositoryBaseS, RepositoryBaseS>();
            builder.Services.AddScoped<ISystemRepository, SystemRepository>();
            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app) //Infrastructure
        {
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add a UI
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId("msg_microservice_swagger");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MSG Identity API");
                c.DisplayRequestDuration();
            });
            app.UseRouting();
            app.UseMiddleware<ErrorWrappingMiddleware>();
            app.UseHttpsRedirection();
            //set cookie policy before authentication/Authorization setup
            app.UseCookiePolicy(); //trong truong hop ko login trong 1 trang
            app.UseIdentityServer();

            // uncomment if you want to add a UI
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute().RequireAuthorization("Bearer");
                endpoints.MapRazorPages().RequireAuthorization();
            });

            return app;
        }
    }
}