using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RoleDefinition_PrivilegeController : ControllerBase
{
    private readonly IRoleDefinition_PrivilegeService _service;
    public RoleDefinition_PrivilegeController(IRoleDefinition_PrivilegeService service) => _service = service;

    [HttpGet]
    public IActionResult GetList() => Ok(_service.GetList());

    [HttpPost]
    public IActionResult Add([FromBody] RoleDefinition_Privilege link) => Ok(_service.Add(link));

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return NoContent();
    }
}