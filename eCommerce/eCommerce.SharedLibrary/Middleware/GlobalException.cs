
using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "Internal server error occured. Try again";
            int statusCode = StatusCodes.Status500InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request made.";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "Not authorized to access";
                    statusCode = StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    message = "You are not allowed to access";
                    statusCode = StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message,statusCode);
                }
            }
            catch (Exception ex)
            {
                //Log Exceptions / file,debug,console
                LogException.LogExceptions(ex);
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "Request timeout";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }
                //if none, server error occured.
                await ModifyHeader(context, title, message, statusCode);
            }
        }
        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
        }
    }
}
