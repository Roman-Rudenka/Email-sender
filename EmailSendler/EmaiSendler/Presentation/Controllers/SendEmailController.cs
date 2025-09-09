using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;


[ApiController]
[Route("api")]
public class SendEmailController(IQueryService queryService)
{
    [HttpPost("sendEmail")]
    public void SendEmail([FromBody] string email)
    {
        queryService.SendEmailMessageAsync(email,"info","Data successfully added");
    }
}