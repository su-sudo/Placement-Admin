using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlacementAdmin.DAL;
using PlacementAdmin.Models;
using PlacementAdmin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using PlacementAdmin.Controllers;
using PlacementAdmin.services;
using NLog;

public class AccountController : Controller
{
    private readonly UserDAL userDAL;
    private readonly ILogger<AccountController> logger;
    private readonly UtilityService utilityService;
    private readonly NLog.ILogger loginAttemptsLogger;

    public AccountController(UserDAL userDAL, ILogger<AccountController> logger, UtilityService utilityService)
    {
        this.userDAL = userDAL;
        this.logger = logger;
        this.utilityService = utilityService;
        this.loginAttemptsLogger = LogManager.GetLogger("LoginAttemptsLogger");
    }

    public IActionResult Login()
    {
        return RedirectToAction("Index", "Home");
    }

    public IActionResult SignUp()
    {
        

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        User user = null;
        SignUpViewModel signUpViewModel = null;
        try
        {
            //i have used Di to inject the password hasing as a service 
            var hashedPassword = utilityService.ComputeHash(password);
            user = await userDAL.GetUserByUsernameAndPassword(username, hashedPassword);
            
        }
        catch (Exception ex)
        {

            loginAttemptsLogger.Error(ex, "An error occurred while trying to log in.");
            Console.WriteLine(ex.Message);

        }

        var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            if (user.Role == "Student")
            {
                loginAttemptsLogger.Info("User {Username} logged in successfully at {Time} from IP {userIp}.", username, DateTime.UtcNow,userIp);
                return RedirectToAction("Home", "User"); 
            }
            else if(user.Role == "Admin")
            {
                loginAttemptsLogger.Info("User {Username} logged in successfully at {Time} from IP {userIp}.", username, DateTime.UtcNow, userIp);
                return RedirectToAction("Index", "Admin");
            }

            
            
        }

        loginAttemptsLogger.Warn("Failed login attempt for username {Username} at {Time} from the IP {userIp}.", username, DateTime.UtcNow, userIp);
        return RedirectToAction("Index","Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        loginAttemptsLogger.Info("User {Username} logged out successfully at {Time}.", User.Identity.Name, DateTime.UtcNow);
        return View("Logout");
    }
}
