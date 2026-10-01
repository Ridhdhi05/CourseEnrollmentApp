using System.Text.Json;
using CourseEnrollmentApp.Models;
using CourseEnrollmentApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CourseEnrollmentApp.Controllers
{
    public class CourseController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        public CourseController(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            const string cacheKey = "CourseList";

            if (!_memoryCache.TryGetValue(cacheKey, out List<Course>? courses) || courses == null)
            {
                courses = new List<Course>
                {
                    new Course { CourseId = 101, CourseName = "ASP.NET Core Web API", Duration = "3 Months", Fees = 500, DiscountPercentage = 10 },
                    new Course { CourseId = 102, CourseName = "Full Stack C# & React", Duration = "6 Months", Fees = 1200, DiscountPercentage = 15 },
                    new Course { CourseId = 103, CourseName = "Cloud Computing with Azure", Duration = "4 Months", Fees = 800, DiscountPercentage = 20 }
                };

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _memoryCache.Set(cacheKey, courses, cacheEntryOptions);
            }

            if (!Request.Cookies.ContainsKey("StudentName"))
            {
                Response.Cookies.Append("StudentName", "Ridhdhi", new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7),
                    HttpOnly = true
                });
            }

            ViewBag.StudentName = Request.Cookies["StudentName"] ?? "Ridhdhi";
            ViewBag.SelectedCourse = HttpContext.Session.GetString("SelectedCourse") ?? "None";

            return View(courses);
        }

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Details(int courseId)
        {
            if (!_memoryCache.TryGetValue("CourseList", out List<Course>? courses) || courses == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var course = courses.FirstOrDefault(c => c.CourseId == courseId);
            if (course == null) return NotFound();

            HttpContext.Session.SetString("SelectedCourse", course.CourseName);
            ViewBag.StudentName = Request.Cookies["StudentName"] ?? "Ridhdhi";

            return View(course);
        }

        [HttpPost]
        public IActionResult Enroll(int hiddenCourseId, string studentName)
        {
            TempData["SuccessMessage"] = $"Student '{studentName}' successfully enrolled in Course ID: {hiddenCourseId}!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Users()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/users");

            List<UserViewModel> users = new List<UserViewModel>();

            if (response.IsSuccessStatusCode)
            {
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var content = await response.Content.ReadAsStringAsync();
                users = JsonSerializer.Deserialize<List<UserViewModel>>(content, jsonOptions) ?? new List<UserViewModel>();
            }

            return View(users);
        }
    }
}