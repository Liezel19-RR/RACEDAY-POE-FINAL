using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

[SessionAuthorize("Participant")]
public class ParticipantController(ApiClient api) : Controller
{
    public async Task<IActionResult> MyEvents()
    {
        return View(await api.Get<List<EnrolmentVm>>("api/enrolments/mine") ?? []);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var response = await api.Send("PUT", $"api/enrolments/{id}/status", new { status = "Cancelled" });
        TempData["Message"] = response.message;
        return RedirectToAction(nameof(MyEvents));
    }

    public async Task<IActionResult> Results()
    {
        return View(await api.Get<List<ResultVm>>("api/results/me") ?? []);
    }

    public async Task<IActionResult> Profile()
    {
        var profile = await api.Get<ProfileVm>("api/users/me");
        return profile is null ? RedirectToAction("Login", "Account") : View(profile);
    }
}
