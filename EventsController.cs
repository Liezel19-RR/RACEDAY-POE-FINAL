using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers;

public class EventsController(ApiClient api) : Controller
{
    public async Task<IActionResult> Index(EventFilterVm filter)
    {
        var parameters = new Dictionary<string, string?>();
        if (!string.IsNullOrWhiteSpace(filter.Search)) parameters["search"] = filter.Search;
        if (!string.IsNullOrWhiteSpace(filter.Type)) parameters["type"] = filter.Type;
        if (!string.IsNullOrWhiteSpace(filter.Location)) parameters["location"] = filter.Location;
        if (filter.From.HasValue) parameters["from"] = filter.From.Value.ToString("yyyy-MM-dd");
        if (filter.To.HasValue) parameters["to"] = filter.To.Value.ToString("yyyy-MM-dd");
        parameters["upcomingOnly"] = filter.UpcomingOnly.ToString().ToLowerInvariant();
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("api/events", parameters);
        ViewBag.Filter = filter;
        return View(await api.Get<List<EventVm>>(query) ?? []);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await api.Get<EventVm>($"api/events/{id}");
        if (item is null) return NotFound();
        ViewBag.Leaderboard = await api.Get<List<LeaderboardVm>>($"api/results/event/{id}") ?? [];
        return View(item);
    }

    [HttpGet, SessionAuthorize("Organiser")]
    public IActionResult Create() => View(new CreateEventVm());

    [HttpPost, SessionAuthorize("Organiser")]
    public async Task<IActionResult> Create(CreateEventVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var response = await api.Send("POST", "api/events", vm);
        if (!response.ok)
        {
            ModelState.AddModelError("", response.message);
            return View(vm);
        }
        return RedirectToAction("Mine", "Organiser");
    }

    [HttpGet, SessionAuthorize("Organiser")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await api.Get<CreateEventVm>($"api/events/{id}");
        if (item is null) return NotFound();
        ViewBag.Id = id;
        return View(item);
    }

    [HttpPost, SessionAuthorize("Organiser")]
    public async Task<IActionResult> Edit(int id, CreateEventVm vm)
    {
        if (!ModelState.IsValid) { ViewBag.Id = id; return View(vm); }
        var response = await api.Send("PUT", $"api/events/{id}", vm);
        if (!response.ok)
        {
            ModelState.AddModelError("", response.message);
            ViewBag.Id = id;
            return View(vm);
        }
        return RedirectToAction("Mine", "Organiser");
    }

    [HttpPost, SessionAuthorize("Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await api.Send("DELETE", $"api/events/{id}");
        TempData["Message"] = response.message;
        return RedirectToAction("Mine", "Organiser");
    }

    [HttpPost, SessionAuthorize("Organiser")]
    public async Task<IActionResult> UploadBanner(int id, IFormFile file)
    {
        var response = await api.Upload($"api/events/{id}/banner", file);
        TempData["Message"] = response.message;
        return RedirectToAction("Mine", "Organiser");
    }

    [HttpPost, SessionAuthorize("Participant")]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        var response = await api.Send("POST", "api/enrolments", new { eventId, categoryId });
        TempData["Message"] = response.message;
        return RedirectToAction(nameof(Details), new { id = eventId });
    }
}
