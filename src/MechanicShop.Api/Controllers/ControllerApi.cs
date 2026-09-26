using MechanicShop.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MechanicShop.Api.Controllers;

[ApiController]
public class ControllerApi : ControllerBase
{
    protected ActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Problem();
        }
        else if (errors.All(e => e.Type == ErrorKind.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0])!;
    }

    private ObjectResult? Problem(Error error)
    {
        var problemCode = error.Type switch
        {
            ErrorKind.Validation => StatusCodes.Status400BadRequest,
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            ErrorKind.UnAuthorized => StatusCodes.Status401Unauthorized,
            ErrorKind.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Problem(title: error.Description, statusCode: problemCode);
    }

    private ActionResult ValidationProblem(List<Error> errors)
    {
        var modelStateDict = new ModelStateDictionary();

        errors.ForEach(e => modelStateDict.AddModelError(e.Code, e.Description));

        return ValidationProblem(modelStateDict);
    }
}
