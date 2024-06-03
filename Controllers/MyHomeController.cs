using MyFinance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json;

namespace MyFinance.Controllers
{
    public class MyHomeController : Controller
    {
        private DBContext db = new DBContext();

        public async Task<ActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = int.Parse(User.Identity.Name);
            var userTransactions = await db.Transactions.Include("Categories").Where(t => t.Id_User == userId).ToListAsync();

            //Recupero UserName
            var user = db.Users.Find(userId);
            ViewBag.UserName = user.UserName;

            // Ultimi 7 giorni
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransaction = await db.Transactions
                .Include("Categories")
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            // Totale uscite
            decimal totalSpent = userTransactions
                .Where(t => t.Categories.CategoryType == "Uscita")
                .Sum(t => t.Amount ?? 0);
            ViewBag.TotalSpent = totalSpent;

            // Totale entrate
            decimal totalEarned = userTransactions
                .Where(t => t.Categories.CategoryType == "Entrata")
                .Sum(t => t.Amount ?? 0);
            ViewBag.TotalEarned = totalEarned;

            // Totale bilancio
            decimal totalAmount = totalEarned - totalSpent;
            ViewBag.TotalAmount = totalAmount;

            //Grafico spese per categoria
            var expenseByCategory = userTransactions
                .Where(t => t.Categories.CategoryType == "Uscita")
                .GroupBy(t => t.Categories.CategoryName)
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = g.Sum(t => t.Amount ?? 0)
                })
                .ToList();

            ViewBag.ExpenseByCategoryJson = JsonConvert.SerializeObject(expenseByCategory);

            //Grafico entrate vs uscite
            var incomeVsExpense = userTransactions
                .GroupBy(t => t.Date)
                .Select(g => new
                {
                    Day = g.Key,
                    Income = g.Where(t => t.Categories.CategoryType == "Entrata").Sum(t => t.Amount ?? 0),
                    Expense = g.Where(t => t.Categories.CategoryType == "Uscita").Sum(t => t.Amount ?? 0),
                })
                .OrderBy(g => g.Day)
                .ToList();

            ViewBag.IncomeVsExpenseJson = JsonConvert.SerializeObject(incomeVsExpense);

            //Transazioni recenti
            DateTime startDate = DateTime.Now.AddDays(-6);
            DateTime endDate = DateTime.Today;

            var recentTransactions = await db.Transactions
                .Include("Categories")
                .Where(t => t.Id_User == userId && t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();

            ViewBag.RecentTransactions = recentTransactions;


            return View();
        }
    }
    
}