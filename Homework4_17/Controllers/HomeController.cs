using Homework4_17.Data;
using Homework4_17.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace Homework4_17.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Images; Integrated Security=true;";

        private IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upload(IFormFile image, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            image.CopyTo(fs);

            ImagesDb db = new ImagesDb(_connectionString);
            int id = db.AddImage(fileName, password);

            UploadViewModel vm = new()
            {
                Password = password,
                Id = id
            };

            return View(vm);

        }

        public IActionResult ViewImage(int id)
        {

            List<int> imageIds = HttpContext.Session.Get<List<int>>("imageIds");

            ImagesDb db = new ImagesDb(_connectionString);
            Image img = db.GetImageById(id);
            ViewImageViewModel vm = new()
            {
                ImageIds = imageIds,
                Image = img
            };

            if (TempData["Message"] != null)
            {
                vm.Message = (string)TempData["Message"];
            }

            return View(vm);
        }

        public IActionResult Password(int id, string password)
        {
            ImagesDb db = new ImagesDb(_connectionString);
            Image img = db.GetImageById(id);

            if (password != img.Password)
            {
                TempData["Message"] = $"Incorrect Password, Please Try again";
            }
            else
            {
                List<int> imageIds = HttpContext.Session.Get<List<int>>("imageIds");
                imageIds.Add(id);
                HttpContext.Session.Set("imageIds", imageIds);
            }

            return RedirectToAction("ViewImage");

        }




    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }

}