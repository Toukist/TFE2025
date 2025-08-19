using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _service;
    public UserRoleController(IUserRoleService service) => _service = service;

    [HttpGet]
    public IActionResult GetList() => Ok(_service.GetList());

    [HttpPost]
    public IActionResult Add([FromBody] UserRole userRole) => Ok(_service.Add(userRole));

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return NoContent();
    }
}