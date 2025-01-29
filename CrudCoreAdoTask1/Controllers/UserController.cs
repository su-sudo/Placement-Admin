using Microsoft.AspNetCore.Mvc;
using CrudCoreAdoTask1.DAL;
using CrudCoreAdoTask1.Models;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace CrudCoreAdoTask1.Controllers
{
    public class UserController : Controller
    {
        private readonly User_DAL _userDal;

        public UserController(User_DAL user_DAL)
        {
            _userDal = user_DAL;
        }
        public IActionResult ListUsers()
        {
            List<Users> users = _userDal.GetAllUsers();

                return View(users);
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            Users user = _userDal.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("_EditUser", user); 
        }

        [HttpPost]
        public JsonResult EditUser(Users user, IFormFile ProfilePic)
        {
            if (ProfilePic != null)
            {
                user.ProfilePic = ConvertToBase64(ProfilePic);
            }

            string result = _userDal.UpdateUser(user);
            if (result.Contains("Success"))
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = result });
        }



        [HttpPost]
        public JsonResult DeleteUser(int Id)
        {
            string result = _userDal.DeleteUser(Id);
            if (result.Contains("Success"))
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Error deleting user" });
        }



        public IActionResult CreateUser()
        {
            return View();
        }


       [HttpPost]
        public IActionResult CreateUser(UserCreateViewModel userCreateViewModel)
        {
            if (ModelState.IsValid) {

                var user = new Users {
                    Name = userCreateViewModel.Name,
                    DateOfBirth = userCreateViewModel.DateOfBirth,
                    PasswordHash = ComputeHash(userCreateViewModel.Password)
                };
                 
                if (userCreateViewModel.ProfilePic != null)
                {
                    user.ProfilePic = ConvertToBase64(userCreateViewModel.ProfilePic);//returnds string
                }

                string result =_userDal.AddUser(user);
                ViewBag.Message = result;
                return RedirectToAction("ListUsers");
            }
            return View();
        }

        private string ConvertToBase64(IFormFile profilePic)
        {
            
            //throw new NotImplementedException();
            using (var memoryStream = new MemoryStream()) { 
                profilePic.CopyTo(memoryStream);
                //TODO remove metaDat
                return Convert.ToBase64String(memoryStream.ToArray());
            }

        }

        private string ComputeHash(string password)
        {
            using(SHA256 sha256 = SHA256.Create())
            {
                byte[] byteData = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < byteData.Length; i++)
                {
                    stringBuilder.Append(byteData[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
            //throw new NotImplementedException();
        }
    }
}
