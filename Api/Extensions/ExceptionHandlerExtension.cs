using System.Text.Json;
using Api.Exceptions;
using Api.Models;
using Api.Models.Responses;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Extensions;

public static class ExceptionHandlerExtension
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    var statusCode = contextFeature.Error switch
                    {
                        DatabaseErrorException => StatusCodes.Status500InternalServerError,
                        BadRequestException => StatusCodes.Status400BadRequest,
                        NotFoundException => StatusCodes.Status404NotFound,
                        AlreadyExistsException => StatusCodes.Status409Conflict,
                        PreconditionFailedException => StatusCodes.Status412PreconditionFailed,
                        UnauthorizedException => StatusCodes.Status401Unauthorized,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    // Log the error
                    logger.LogError($"Global exception handled: {contextFeature.Error.Message}");

                    // Check if the exception is of type DatabaseErrorException
                    if (contextFeature.Error is DatabaseErrorException dbError)
                    {
                        var errorResponse = ApiResponse.Failed(
                            message: dbError.Message,
                            errors: dbError.Errors
                        );
                        context.Response.StatusCode = statusCode;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                    }
                    else
                    {
                        var errorResponse = ApiResponse.Failed(
                            message: contextFeature.Error.Message
                        );
                        context.Response.StatusCode = statusCode;
                        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                    }
                }
            });
        });
    }
}
