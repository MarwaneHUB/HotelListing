using HotelListing.API.Core.Exceptions;
using HotelListing.API.Models.Errors;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System.Net;

namespace HotelListing.API.Core.Middlewares;

public class ExceptionMiddleware {
    private readonly RequestDelegate _next;
     private readonly ILogger<ExceptionMiddleware> logger;
    public ExceptionMiddleware( RequestDelegate next, ILogger<ExceptionMiddleware> logger ) {
        this._next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync( HttpContext httpContext) {
        try {
            await _next( httpContext );
        } catch (Exception ex) {
            logger.LogError( ex.Message, ex.InnerException );
            await HandleExceptionAsync( httpContext,ex );
        }
    }

    private Task HandleExceptionAsync( HttpContext httpContext,Exception ex ) {
        
        var httpStatusCode = HttpStatusCode.InternalServerError;

        var erroDetails = new ErrorDetails{
            Type= "Failure",
            Message = ex.Message,
        };

        switch (ex) {
            case NotFoundException:
            httpStatusCode = HttpStatusCode.NotFound;
            erroDetails.Type = "Not Found";
            break;
            
            default:
            break;
        }

        var response = JsonConvert.SerializeObject(erroDetails);
        httpContext.Response.StatusCode = (int)httpStatusCode;
        httpContext.Response.ContentType = "application/json";

        return httpContext.Response.WriteAsync(response);
    }
}
