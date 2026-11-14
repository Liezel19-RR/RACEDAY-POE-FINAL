using Microsoft.AspNetCore.Mvc;
namespace RaceDay.Mvc.Controllers;
public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Error(int? statusCode = null)
    {
        Response.StatusCode = statusCode ?? StatusCodes.Status500InternalServerError;
        ViewBag.StatusCode = Response.StatusCode;
        return View();
    }
}
