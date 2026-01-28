using JobQueue.Application.Commands;
using JobQueue.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace JobQueue.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromServices] CreateJobHandler handler,
        [FromBody] CreateJobCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Handle(request, cancellationToken);

        return Accepted(nameof(GetById), new { id = result });
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromServices] GetJobsQueryHandler handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Handle(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] GetJobQueryHandler handler,
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.Handle(id, cancellationToken);
        return Ok(result);
    }
}
