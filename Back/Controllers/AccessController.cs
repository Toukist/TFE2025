using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccessController : ControllerBase
{
    private readonly IAccessService _service;
    public AccessController(IAccessService service) => _service = service;

    [HttpGet]
    public IActionResult GetList() => Ok(_service.GetList());

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var access = _service.Get(id);
        return access == null ? NotFound() : Ok(access);
    }

    [HttpPost]
    public IActionResult Add([FromBody] Access access) => Ok(_service.Add(access));

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Access access)
    {
        _service.Update(id, access);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return NoContent();
    }
}