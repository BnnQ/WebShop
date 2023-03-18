using Homework.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Homework.Controllers;

[Authorize(policy: "ProductManagement")]
[RetrieveModelErrorsFromRedirector]
public class ManagerController : Controller
{
    // GET
    public IActionResult Home() => View();
}