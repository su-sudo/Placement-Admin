using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlacementAdmin.Models;
using PlacementAdmin.DAL;
using PlacementAdmin.Models.ViewModel;
using System.Threading.Tasks;

namespace PlacementAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserDAL _userDAL;

        public HomeController(ILogger<HomeController> logger, UserDAL userDAL)
        {
            _logger = logger;
            _userDAL = userDAL;
        }

        public async Task<IActionResult> Index()
        {
            List<CourseStream> courseStreams = new List<CourseStream>();

            try
            {
                courseStreams =  await _userDAL.GetAvailableCourseStreamsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching course streams.");
            }

            var signUpViewModel = new SignUpViewModel
            {
                DateOfBirth = DateTime.Now,
                CourseStreams = courseStreams
            };

            var loginViewModel = new loginViewModel();

            var model = new HomeViewModel
            {
                SignUpViewModel = signUpViewModel,
                loginViewModel = loginViewModel,
                CourseStreams = courseStreams
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }
    }
}
