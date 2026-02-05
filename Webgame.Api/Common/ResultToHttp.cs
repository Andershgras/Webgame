using Microsoft.AspNetCore.Mvc;
using Webgame.Application.Common;

namespace Webgame.Api.Common;

public static class ResultToHttp
{
    public static ObjectResult ToProblemDetails(ControllerBase controller, Error error)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title = error.Code,
            Detail = error.Message,
            Type = $"https://errors.webgame/{error.Code}"
        };

        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem) { StatusCode = status };
    }

    public static ActionResult<TDto> ToActionResult<TDomain, TDto>(
        ControllerBase controller,
        Result<TDomain> result,
        Func<TDomain, TDto> map,
        Func<TDto, ActionResult<TDto>> onSuccess)
    {
        if (result.IsSuccess)
        {
            var dto = map(result.Value!);
            return onSuccess(dto);
        }

        return ToProblemDetails(controller, result.Error!);
    }

    public static IActionResult ToActionResult(
        ControllerBase controller,
        Result result,
        Func<IActionResult> onSuccess)
    {
        if (result.IsSuccess)
            return onSuccess();

        return ToProblemDetails(controller, result.Error!);
    }
}

