using Application.DataTransferModels.ResponseModel;
using Common.Methods;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace blogging_Website.Middlewares.Exceptions
{
    public class ExceptionMiddleware
    {
            private readonly ILogger<ExceptionMiddleware> _logger;
            private readonly RequestDelegate _next;
            private readonly IConfiguration _config;

            public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, RequestDelegate next, IConfiguration config)
            {
                _logger = logger;
                _next = next;
                _config = config;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (ArgumentNullException ex)
                {
                    HandleException(context, ex, "ArgumentNullException");
                }
                catch (InvalidOperationException ex)
                {
                    HandleException(context, ex, "InvalidOperationException");
                }
                catch (Exception ex)
                {
                    HandleException(context, ex, "GeneralException");
                }
            }

            private void HandleException(HttpContext context, Exception ex, string exceptionType)
            {
                GetEndpointInfo(context, ex, exceptionType);
                _logger.LogError(ex, $"Exception Type: {exceptionType}");
                HandleCustomExceptionResponseAsync(context, ex.Message).Wait();
            }

            private void GetEndpointInfo(HttpContext context, Exception ex, string exceptionType)
            {
                var controllerActionDescriptor = context
                    .GetEndpoint()?
                    .Metadata
                    .GetMetadata<ControllerActionDescriptor>();

                string controllerName = controllerActionDescriptor?.ControllerName ?? "UnknownController";
                string actionName = controllerActionDescriptor?.ActionName ?? "UnknownAction";
                string apiName = $"{controllerName}/{actionName}";

                string exceptionMessage = ex.ToString().Length > 1500 ? ex.ToString().Substring(0, 1500) : ex.ToString();
                string emailBody = $"Exception Type: {exceptionType}\nAPI: {apiName}\nMessage: {exceptionMessage}";

                //_ = CommonMethod.SendEmail("nm.atgsystem@gmail.com", "AdmissionLelo Exception Alert", emailBody, _config);
            }

            private async Task HandleCustomExceptionResponseAsync(HttpContext context, string exceptionMessage)
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ResponseVm response = ResponseVm.Instance;
                response.responseCode = context.Response.StatusCode;
                response.responseMessage = exceptionMessage;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
    }
}
