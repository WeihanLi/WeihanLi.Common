using AspNetCoreSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AspNetCoreSample.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        ViewData["Message"] = "Your application description page.";

        return View();
    }

    public IActionResult Contact()
    {
        ViewData["Message"] = "Your contact page.";

        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Upload file
    /// upload file in asp.net core mvc
    /// https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
    /// </summary>
    /// <param name="formFile">fileInfo</param>
    /// <returns></returns>
    [HttpPost]
    public string Upload(IFormFile? formFile)
    {
        if (formFile == null)
        {
            return "failed";
        }
        Console.WriteLine(formFile.Name);
        return "success";
    }
}
