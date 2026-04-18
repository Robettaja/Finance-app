using finance.Models;
using finance.Models.Tables;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace finance.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> Index()
    {
        if (User.Identity!.IsAuthenticated)
        {

            User user = await DatabaseManipulator.GetSingle<User>(u => u.Username == User.Identity!.Name);
            HomeViewModel homeViewModel = new() { UserId = user.Id };
            await homeViewModel.Initialize();
            return View(homeViewModel);

        }
        return View();
    }
    [RequireAntiforgeryToken]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Index(int savingGoal)
    {
        Console.WriteLine(savingGoal);
        User user = await DatabaseManipulator.GetSingle<User>(u => u.Username == User.Identity!.Name);
        user.SavingGoal = savingGoal;

        await DatabaseManipulator.Update(user, u => u.Id == user.Id);

        HomeViewModel homeViewModel = new() { UserId = user.Id };
        await homeViewModel.Initialize();
        return View(homeViewModel);
    }
}
