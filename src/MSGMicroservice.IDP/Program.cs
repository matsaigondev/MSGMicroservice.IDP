using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MSGMicroservice.IDP.Extensions;
using MSGMicroservice.IDP.Persistence;
using Serilog;

Log.Information("Starting up");

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.Host.AddAppConfigurations();
    builder.Host.ConfigureSerilog();

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    SeedUserData.EnsureSeedData(builder.Configuration.GetConnectionString("IdentitySqlConnection"));
    // var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
    //
    // builder.Services.AddAuthentication(x =>
    //     {
    //         x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //         x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //     })
    //     .AddJwtBearer(x => {
    //         x.RequireHttpsMetadata = false;
    //         x.SaveToken = true;
    //         //x.Authority = "https://localhost:7003/";
    //         x.TokenValidationParameters = new TokenValidationParameters
    //         {
    //             ValidateIssuerSigningKey = true,
    //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
    //             ValidateIssuer = false,
    //             ValidateAudience = false
    //         };
    //     });
    app.MigrateDatabase()
        .Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shutdown {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}