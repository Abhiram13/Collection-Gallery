using System.Net;
using CollectionGallery.InfraStructure.Data.Controllers;
using CollectionGallery.Shared.Exceptions;
using CollectionGallery.Shared.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionGallery.InfraStructure.Middlewares;

public class InvalidPayloadExceptionHandler : IExceptionHandler
{
    private readonly ILogger<InvalidPayloadExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;
    
    public InvalidPayloadExceptionHandler(ILogger<InvalidPayloadExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not InvalidPayloadException)
        {
            return false;
        }
        
        HttpRequest request = httpContext.Request;
        HttpResponse response = httpContext.Response;
        string requestUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        
        _logger.LogError(exception, "Invalid Payload Exception at Request = {Request}. Exception message = {ExceptionMessage}", requestUrl, exception.InnerException?.Message ?? exception.Message);

        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = "Bad Request",
            Detail = "Invalid payload provided. Check logs for more details",
            Status = StatusCodes.Status400BadRequest
        };
        
        response.StatusCode = StatusCodes.Status400BadRequest;
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            ProblemDetails =  problemDetails,
            HttpContext = httpContext,
        });
        
        return true;
    }
}

/// <summary>
/// <see href="https://codewithmukesh.com/blog/problem-details-in-aspnet-core/">Problem details in ASP.NET Core by Mukesh</see>
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;
    
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        HttpRequest request = httpContext.Request;
        HttpResponse response = httpContext.Response;
        string requestUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        
        _logger.LogError(exception, "Unknown Exception at Request = {Request}. Exception message = {ExceptionMessage}", requestUrl, exception.InnerException?.Message ?? exception.Message);

        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = "Unknown Error",
            Detail = "Something went wrong. Check logs for more details",
            Status = StatusCodes.Status500InternalServerError
        };
        
        response.StatusCode = StatusCodes.Status500InternalServerError;
        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            ProblemDetails =  problemDetails,
            HttpContext = httpContext,
        });
        
        return true;
    }
}