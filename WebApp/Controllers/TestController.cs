using Microsoft.AspNetCore.Mvc;

public class TestController : Controller
{
    public IActionResult SetRole(string role)
    {
        int userId = role switch
        {
            "Owner" => 1,
            "Editor" => 2,
            "Viewer" => 3,
            _ => 0
        };

        HttpContext.Session.SetInt32("UserId", userId);
        HttpContext.Session.SetString("UserName", role + "User");
        HttpContext.Session.SetString("UserRole", role);

        return Content($"Role set to {role} (UserId={userId})");
    }
}
