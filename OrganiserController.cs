using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

[SessionAuthorize("Organiser")]
public class OrganiserController(ApiClient api) : Controller
{
    public async Task<IActionResult> Dashboard()
    {
        var events = await api.Get<List<EventVm>>("api/events/mine") ?? [];
        return View(events);
    }

    public async Task<IActionResult> Categories(int eventId)
    {
        ViewBag.EventId = eventId;
        ViewBag.Event = await api.Get<EventVm>($"api/events/{eventId}");
        return View(await api.Get<List<CategoryVm>>($"api/events/{eventId}/categories") ?? []);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(int eventId, CategoryVm vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Message"] = "Enter valid category details.";
            return RedirectToAction(nameof(Categories), new { eventId });
        }
        var response = await api.Send("POST", $"api/events/{eventId}/categories",
            new { vm.Name, vm.Description, vm.DistanceKm, vm.Price, vm.Capacity });
        TempData["Message"] = response.message;
        return RedirectToAction(nameof(Categories), new { eventId });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCategory(int eventId, int categoryId, CategoryVm vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Message"] = "Enter valid category details.";
            return RedirectToAction(nameof(Categories), new { eventId });
        }
        var response = await api.Send("PUT", $"api/events/{eventId}/categories/{categoryId}",
            new { vm.Name, vm.Description, vm.DistanceKm, vm.Price, vm.Capacity });
        TempData["Message"] = response.message;
        return RedirectToAction(nameof(Categories), new { eventId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int eventId, int categoryId)
    {
        var response = await api.Send("DELETE", $"api/events/{eventId}/categories/{categoryId}");
        TempData["Message"] = response.message;
        return RedirectToAction(nameof(Categories), new { eventId });
    }

    public async Task<IActionResult> Enrolments(int id)
    {
        ViewBag.EventId = id;
        return View(await api.Get<List<EnrolmentVm>>($"api/enrolments/event/{id}") ?? []);
    }

    [HttpPost]
    public async Task<IActionResult> RecordResult(ResultInputVm vm, int eventId)
    {
        if (!ModelState.IsValid || vm.FinishTime <= TimeSpan.Zero)
        {
            TempData["Message"] = "Enter a positive finish time and position.";
            return RedirectToAction(nameof(Enrolments), new { id = eventId });
        }
        var response = await api.Send("POST", "api/results", vm);
        TempData["Message"] = response.message;
        return RedirectToAction(nameof(Enrolments), new { id = eventId });
    }
}
