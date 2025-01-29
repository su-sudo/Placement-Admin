using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlacementAdmin.DAL;
using PlacementAdmin.Models;
using PlacementAdmin.Models.ViewModel;
using PlacementAdmin.services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static PlacementAdmin.DAL.UserDAL;

namespace PlacementAdmin.Controllers
{
    public class UserController : Controller
    {
        private readonly UserDAL _userDAL;
        private readonly UtilityService _utilityService;
        

        public UserController(UserDAL userDAL, UtilityService utilityService)
        {
            _userDAL = userDAL;
            
            _utilityService = utilityService;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("Username,Email,Password,ConfirmPassword,DateOfBirth,Gender,CourseStreamId,ProfilePicture")] SignUpViewModel model)
        {
            model.CourseStreams = null;
            ModelState.Remove("CourseStreams");

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = model.Username,
                    Password = _utilityService.ComputeHash(model.Password),
                    Email = model.Email,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    //ProfilePicture = model.ProfilePicture?.FileName,
                    CourseStreamId = model.CourseStreamId,
                    Role = "Student" 
                };
                if (model.ProfilePicture != null)
                {
                    user.ProfilePicture = _utilityService.ConvertToBase64(model.ProfilePicture);//returnds string
                }

                await _userDAL.AddUser(user);
                return RedirectToAction("Index", "Home");
            }
            TempData["ErrorMessage"] = "Failed to create User.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = _userDAL.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _userDAL.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                await _userDAL.UpdateUser(user);
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = _userDAL.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userDAL.DeleteUser(id);
            return RedirectToAction("Index", "Home");
        }

        //student home 
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Home()
        {
            string userName = User.Identity.Name;
            ViewBag.UserName = userName;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdClaim.Value);
            var testResults = await _userDAL.GetStudentTestResultsAsync(userId);
            var viewModel = new StudentHomeViewModel
            {
                TestResults = testResults
            };
            return View("StudentHome", viewModel);
        }

        public async Task<IActionResult> TestResults()
       {
           
           var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
           if (userIdClaim == null)
           {
               return Unauthorized();
           }

           int userId = int.Parse(userIdClaim.Value);
           var testResults = await _userDAL.GetStudentTestResultsAsync(userId);
           return View(testResults);
       }
    }

}
