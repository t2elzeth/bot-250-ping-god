using Infrastructure.Application.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.AspNetCore.Controllers;

[ApiController]
public abstract class ApiController : Controller
{
    protected IActionResult UnitResult(CommandHandlerResult result)
    {
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok();
    }

    protected ActionResult<TResponse> Result<TResponse>(CommandHandlerResult result)
    {
        if (result.IsFailure)
            return BadRequest(result.Error);

        return (TResponse)result.Value;
    }
    
    protected ActionResult Error(object error)
    {
        return BadRequest(error);
    }
}