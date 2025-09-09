using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;


[ApiController]
[Route("api")]
public class UserDataController(IUserDataService userDataService) : ControllerBase
{
    [HttpPost("new-credentials")]
    public async Task<IActionResult> SetNewData([FromBody] UserData credentials, CancellationToken cancellationToken)
    {
        var newCredentials = await userDataService.GetData(credentials.Email, credentials.Address, credentials.PhoneNumber, cancellationToken);
         
        return Ok(newCredentials);
    }
}