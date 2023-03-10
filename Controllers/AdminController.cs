using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Homework.Controllers
{
    [Authorize(policy: "AdminOnly")]
    public class AdminController : Controller
    {
        public IActionResult Home() => View();
    }
}