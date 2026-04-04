using Microsoft.AspNetCore.Mvc;

public class BaseController : ControllerBase
{
    protected virtual string GetUserId()
	{
		return User.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
	}
}