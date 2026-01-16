using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using LoggAutorz.Erros;

namespace LoggAutorz.Middleware
{        public class MiddlewareException
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<MiddlewareException> _logger;
            private readonly IHostEnvironment _env;

            public MiddlewareException(RequestDelegate next, ILogger<MiddlewareException> logger, IHostEnvironment env)
            {
                _env = env;
                _logger = logger;
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
          
                _logger.LogError(ex, ex.Message);
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var statusCode = context.Response.StatusCode;
                    string message;

                    switch (statusCode)
                    {
                        case 400:
                            message = "Invalid request.";
                            break;
                        case 401:
                            message = "Unauthorizhed";
                            break;
                        case 403:
                            message = "Acess denied.";
                            break;
                        case 404:
                            message = "Resourse not found";
                            break;
                        case 500:
                        default:
                            message = "Internal server error";
                            break;
                    }

                    var response = _env.IsDevelopment()
                        ? new ApiException(context.Response.StatusCode.ToString(), ex.Message, ex.StackTrace.ToString()) //STACK TRACE é detalhado
                        : new ApiException(context.Response.StatusCode.ToString(), ex.Message, message);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    var json = JsonSerializer.Serialize(response, options);
                    await context.Response.WriteAsync(json);
                }
            }
        }
    }
