using HW_04_07_2025.Data;
using HW_04_07_2025.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HW_04_07_2025.Web.Controllers
{
    public class ContributorsController : Controller
    {

        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;TrustServerCertificate=yes;";

        public IActionResult Index()
        {
            var db = new DBManager(_connectionString);
            var vm = new ContributorModel
            {
                Contributors = db.GetContributors(),
                Total = db.GetTotalCash()
            };

            if (TempData["message"] != null)
            {
                vm.Message = (string)TempData["message"];
            }
            return View(vm); 
        }

        [HttpPost]
        public IActionResult New(Contributor con, decimal initialDeposit)
        {
            var db = new DBManager(_connectionString);
            db.AddContributor(con);
            db.AddDeposit(new Deposit
            { 
                ContributorId = con.Id,
                Date = con.Date,
                Amount = initialDeposit
            });

            TempData["message"] = $"New Contributor Added";
            return Redirect("/COntributors");

        }


        [HttpPost]
        public IActionResult Edit(Contributor con)
        {
            var db = new DBManager(_connectionString);
            db.UpdateContributor(con);
            TempData["message"] = "Contributor Updated Successully";
            return Redirect("/Contributors");
        }



        public IActionResult History(int conId)
        {
            var db = new DBManager(_connectionString);
            var trans = new List<Transactions>();
            trans.AddRange(db.GetDepositHistory(conId));
            trans.AddRange(db.GetContribHistory(conId));

            var vm = new HistoryModel
            {
                Transactions = trans.OrderByDescending(t => t.Date).ToList(),
                Balance = db.GetBalance(conId),
                Name = db.GetNameById(conId, "Contributors")
            };
            return View(vm);

        }
    }
}
