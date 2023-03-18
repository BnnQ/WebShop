using Homework.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Homework.Controllers;

[Authorize(policy: "AdminOnly")]
[RetrieveModelErrorsFromRedirector]
public class AdminController : Controller
{
    public IActionResult Home() => View();
}