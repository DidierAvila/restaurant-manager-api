using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result.Error);
    }

    protected IActionResult HandleResult(Result result)
    {
        return result.IsSuccess
            ? NoContent()
            : HandleError(result.Error);
    }

    protected IActionResult HandleError(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => NotFound(new
            {
                error.Code,
                error.Message
            }),
            ErrorType.Validation => BadRequest(new
            {
                error.Code,
                error.Message,
                Errors = error.ValidationErrors
            }),
            ErrorType.Conflict => Conflict(new
            {
                error.Code,
                error.Message
            }),
            ErrorType.Unauthorized => Unauthorized(new
            {
                error.Code,
                error.Message
            }),
            ErrorType.Forbidden => StatusCode(403, new
            {
                error.Code,
                error.Message
            }),
            _ => StatusCode(500, new
            {
                Code = error.Code != string.Empty ? error.Code : "ServerError",
                Message = error.Message != string.Empty ? error.Message : "Un error inesperado ocurrió"
            })
        };
    }
}
