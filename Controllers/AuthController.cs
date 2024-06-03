using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyFinance.Models;
using System.IO;

namespace MyFinance.Controllers
{
    public class AuthController : Controller
    {
        DBContext db = new DBContext();

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("AllTransaction", "Transaction");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName, Email, Password")] User user, bool keepLogged)
        {
            if (ModelState.IsValid)
            {

                var loggedUser = db.Users.FirstOrDefault(u => u.UserName == user.UserName && u.Email == user.Email && u.Password == user.Password);

                if (loggedUser != null)
                {

                    FormsAuthentication.SetAuthCookie(loggedUser.UserId.ToString(), keepLogged);
                    Response.Cookies.Add(new HttpCookie("UserId", loggedUser.UserId.ToString()));

                    return RedirectToAction("Index", "MyHome");
                }
                else
                {
                    TempData["ErrorLogin"] = "Credenziali non valide. Si prega di riprovare.";
                }
            }

            // Se si arriva a questo punto, il login non è riuscito
            return View(user);
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Email, UserName, Password")] User user)
        {
            if (ModelState.IsValid)
            {
                using (var db = new DBContext())
                {
                    bool emailExists = db.Users.Any(u => u.Email == user.Email);
                    bool nameExists = db.Users.Any(u => u.UserName == user.UserName);
                    bool passwordExists = db.Users.Any(u => u.Password == user.Password);

                    if (emailExists || nameExists || passwordExists)
                    {
                        if (emailExists)
                        {
                            ModelState.AddModelError("Email", "L'indirizzo email è già stato registrato.");
                        }
                        if (nameExists)
                        {
                            ModelState.AddModelError("Name", "Il nome utente è già stato registrato.");
                        }
                        if (passwordExists)
                        {
                            ModelState.AddModelError("Password", "La password è già utilizzata.");
                        }
                        return View(user);
                    }

                    db.Users.Add(user);
                    db.SaveChanges();
                }
                return RedirectToAction("Login", "Auth");
            }
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}