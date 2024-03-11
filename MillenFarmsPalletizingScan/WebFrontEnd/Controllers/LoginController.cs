using Model;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebFrontEnd.Controllers
{
    public class LoginController : Controller
    {
        public static string LOGGED_IN_USERNAME;
        private LoginService loginService = new LoginService();

        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Login", "Index"));
            }
        }

        [HttpPost]
        public ActionResult Index(string userID, string password)
        {
            try
            {
                if (userID == string.Empty || password == string.Empty)
                {
                    ViewBag.Msg = "Fields cannot be empty.";
                    return View();
                }
                Login log = loginService.ValidateUser(userID, password);
                if (log == null)
                {
                    ViewBag.Msg = "Account not found or username or password is incorrect, please try again.";
                    return View();
                }
                else
                {
                    LOGGED_IN_USERNAME = log.Username;
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Login", "Index"));
            }
        }
    }
}