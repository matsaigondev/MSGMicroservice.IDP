using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MSGMicroservice.IDP.Infrastructure.Common.ApiResult;

namespace MSGMicroservice.IDP.Extensions
{
    public class ErrorWrappingMiddleware
    {
        private readonly ILogger<ErrorWrappingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ErrorWrappingMiddleware(RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            var errorMsg = string.Empty;
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                errorMsg = ex.Message;
                context.Response.StatusCode = 500;
            }

            if (!context.Response.HasStarted && context.Response.StatusCode == 401)
            {
                context.Response.ContentType = "application/json";

                var response = new ApiErrorResult<bool>("You are not authorized!");

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }

            if (!context.Response.HasStarted && context.Response.StatusCode != 204)
            {
                context.Response.ContentType = "application/json";

                var response = new ApiErrorResult<bool>(errorMsg);

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}