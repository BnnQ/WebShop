using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Controllers;

[Authorize(policy: "ProductManagement")]
public class ManagerController : Controller
{
    // GET
    public IActionResult Home() => View();
}