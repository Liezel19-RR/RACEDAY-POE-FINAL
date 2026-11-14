using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class AccountController(ApiClient api) : Controller
{
    [HttpGet] public IActionResult Login() => View(new LoginVm());
    [HttpPost] public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var response = await api.Send("POST", "api/auth/login", vm);
        if (!response.ok)
        {
            ModelState.AddModelError("", response.message);
            return View(vm);
        }
        var user = response.data?.GetProperty("user");
        if (user.HasValue)
        {
            HttpContext.Session.SetString(SessionKeys.Role, user.Value.GetProperty("role").GetString() ?? "Participant");
            HttpContext.Session.SetString(SessionKeys.FullName, user.Value.GetProperty("fullName").GetString() ?? "");
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpGet] public IActionResult Register() => View(new RegisterVm());
    [HttpPost] public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var response = await api.Send("POST", "api/auth/register", vm);
        if (!response.ok)
        {
            ModelState.AddModelError("", response.message);
            return View(vm);
        }
        TempData["Message"] = "Account created. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost, SessionAuthorize]
    public async Task<IActionResult> Logout()
    {
        await api.Send("POST", "api/auth/logout");
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet, SessionAuthorize]
    public async Task<IActionResult> Profile()
    {
        var profile = await api.Get<ProfileVm>("api/users/me");
        return profile is null ? RedirectToAction(nameof(Login)) : View(profile);
    }

    [HttpPost, SessionAuthorize]
    public async Task<IActionResult> Profile(ProfileVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var response = await api.Send("PUT", "api/users/me", vm);
        if (!response.ok)
        {
            ModelState.AddModelError("", response.message);
            return View(vm);
        }
        HttpContext.Session.SetString(SessionKeys.FullName, vm.FullName);
        TempData["Message"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpPost, SessionAuthorize]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        var response = await api.Upload("api/users/me/profile-picture", file);
        TempData["Message"] = response.message;
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet] public IActionResult AccessDenied() => View();
}
