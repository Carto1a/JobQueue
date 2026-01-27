using JobQueue.Application;
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

    public IActionResult List()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        throw new NotImplementedException();
    }
}
