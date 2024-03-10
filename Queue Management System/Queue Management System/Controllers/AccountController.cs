using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Queue_Management_System.Interfaces;
using Queue_Management_System.Models;
using Queue_Management_System.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Queue_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _queueRepository;
        private readonly IAccountService _queueService;
        public AccountController(IAccountRepository queueRepository, IAccountService queueService)
        {
            _queueRepository = queueRepository;
            _queueService = queueService;
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginDetails, string? returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var storedHash = await _queueRepository.GetPasswordHashAsync(loginDetails.UserName);
                    var result = _queueService.VerifyPassword(loginDetails.Password, storedHash);
                    if (result)
                    {
                        var user = await _queueRepository.GetUserByName(loginDetails.UserName);
                        var roles = await _queueRepository.GetUserRolesAsync(user.Id);
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, user.Username));
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                            HttpContext.Session.SetString("Role", role.RoleName);
                        }
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(claimsIdentity);
                        var authProperties = new AuthenticationProperties { IsPersistent = loginDetails.RememberMe };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
                        HttpContext.Session.SetString("userName", loginDetails.UserName);

                        if (returnUrl == null)
                        {
                            if (HttpContext.Session.GetString("Role") == "admin")
                            {
                                return RedirectToAction("Dashboard", "Admin");
                            }
                            if (HttpContext.Session.GetString("Role") == "service provider")
                            {
                                return RedirectToAction("SelectServicePoint", "Account");
                            }
                            return RedirectToAction("Checkinpage", "Queue");
                        }
                        return Redirect(returnUrl);
                    }

                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(loginDetails);
                    }
                }
                return View(loginDetails);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occured");
                return View();
            }
        }
        public async Task<IActionResult> Logout()
        {
            // Clear the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear session data
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roles = await _queueRepository.GetRoles();
            List<SelectListItem> RoleItems = roles.Select(role => new SelectListItem
            {
                Value = role.RoleId.ToString(),
                Text = role.RoleName
            }).ToList();
            ViewBag.roles = RoleItems;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User userDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime date = DateTime.Now;

                    var user = await _queueRepository.GetUserByName(userDetails.Username);
                    if (user.Username == null)
                    {
                        userDetails.Password = _queueService.HashPassword(userDetails.Password);

                        var result = await _queueRepository.CreateUser(userDetails);
                        if (result)
                        {
                            //assign role
                            await _queueRepository.AddUserToRole(userDetails.Username, userDetails.RoleId);
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Could not create user. please try again");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"This user name is already taken. Choose a different user name and sign up again");

                    }


                }
                var roles = await _queueRepository.GetRoles();
                List<SelectListItem> RoleItems = roles.Select(role => new SelectListItem
                {
                    Value = role.RoleId.ToString(),
                    Text = role.RoleName
                }).ToList();
                ViewBag.roles = RoleItems;
                return View();
            }
            catch (Exception ex)
            {
                var roles = await _queueRepository.GetRoles();
                List<SelectListItem> RoleItems = roles.Select(role => new SelectListItem
                {
                    Value = role.RoleId.ToString(),
                    Text = role.RoleName
                }).ToList();
                ViewBag.roles = RoleItems;
                var dsfsd = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "service provider")]
        public async Task<IActionResult> SelectServicePoint()
        {
            var servicePoints = await _queueRepository.GetServicePointsAsync();
            List<SelectListItem> servicePointListItems = servicePoints.Select(sp => new SelectListItem
            {
                Value = sp.Id.ToString(),
                Text = sp.Name
            }).ToList();
            ViewBag.ServicePoints = servicePointListItems;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SelectServicePoint(CheckInViewModel serviceProvider)
        {
            HttpContext.Session.SetInt32("ServicePointId", serviceProvider.ServiceId);
            return RedirectToAction("ServicePoint", "Queue", new { servicePointId = serviceProvider.ServiceId });
        }
        public IActionResult AccessDenied()
        {

            return View();
        }
    }
}
