using MyFinance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace MyFinance.Controllers
{
    public class TransactionController : Controller
    {
        DBContext db = new DBContext();

        [HttpGet]
        [Authorize]
        public ActionResult AllTransaction()
        {
            var userId = int.Parse(User.Identity.Name);
            var user = db.Users.Find(userId);

            List<Transaction> transactions = db.Transactions.Where(t => t.Id_User == userId).ToList();

            ViewBag.UserName = user.UserName;

            return View(transactions);
        }


        [HttpGet]
        [Authorize]
        public ActionResult AddTransaction()
        {
            var categories = db.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTransaction(Transaction transaction, int CategoryId)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.Identity.Name);
                transaction.Id_User = userId;
                transaction.Id_Category = CategoryId;

                db.Transactions.Add(transaction);
                db.SaveChanges();

                return RedirectToAction("AllTransaction", "Transaction");
            }

            var categories = db.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");

            return View(transaction);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(Category category, bool isInCome)
        {
            if (ModelState.IsValid)
            {
                category.CategoryType = isInCome ? "Entrata" : "Uscita";

                db.Categories.Add(category);
                db.SaveChanges();

                return RedirectToAction("AddTransaction", "Transaction");
            }
            return View(category);
        }

        public ActionResult DeleteTransaction(int id)
        {
            var transactionToDelete = db.Transactions.Find(id);
            if (transactionToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Transactions.Remove(transactionToDelete);
                db.SaveChanges();
            }

            return RedirectToAction("AllTransaction", "Transaction");
        }
    }

}
