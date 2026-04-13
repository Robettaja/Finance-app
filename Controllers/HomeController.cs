using Microsoft.AspNetCore.Mvc;

namespace finance.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> Index()
    {
        return View();
    }

}
