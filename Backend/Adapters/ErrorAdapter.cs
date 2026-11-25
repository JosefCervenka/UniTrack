using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Adapters;

public static class ErrorAdapter
{
    public static IActionResult ToActionResult(ValidationException e)
    {
        var errors = e.Errors.Select(err => new
        {
            Field = err.PropertyName,
            Message = err.ErrorMessage,
            ErrorCode = int.TryParse(err.ErrorCode, out var code) ? code : StatusCodes.Status400BadRequest,
            State = err.CustomState
        }).ToList();

        var statusCode = errors.Max(x => x.ErrorCode);

        return new ObjectResult(errors)
        {
            StatusCode = statusCode
        };
    }
}