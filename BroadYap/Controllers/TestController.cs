using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok("Test successful");
    }

    [HttpGet("database")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> DatabaseTest(IUserRepository userRepository)
    {
        var isTaken = await userRepository.IsEmailTakenAsync("test@example.com");
        if (isTaken)
        {
            return Conflict("Email is already taken.");
        }
        else
        {
            return Ok("Email is available.");
        }
    }
}