using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserAccessController : ControllerBase
{
    private readonly IUserAccessService _service;
    public UserAccessController(IUserAccessService service) => _service = service;

    [HttpGet]
    public IActionResult GetList() => Ok(_service.GetList());

    [HttpPost]
    public IActionResult Add([FromBody] UserAccess userAccess) => Ok(_service.Add(userAccess));

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return NoContent();
    }
}