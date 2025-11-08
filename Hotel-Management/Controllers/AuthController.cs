using System.Diagnostics;
using Hotel_Management.Models;
using Hotel_Management.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;

namespace AspnetCoreMvcFull.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult ForgotPassword() => View();
    public IActionResult Login() => View();
    [HttpGet]
    public IActionResult Register() 
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model) 
    {
        if (!ModelState.IsValid) 
        {
            return View(model);
        }

        //foreach(var error in result.Errors)
        //{
        //    ModelState.AddModelError(string.Empty, error.Description);
        //}
        return View();
    }
}
