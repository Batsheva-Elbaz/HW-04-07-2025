using HW_04_07_2025.Data;
using HW_04_07_2025.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HW_04_07_2025.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;TrustServerCertificate=yes;";

        public IActionResult Index()
        {
            var db = new DBManager(_connectionString);
            var vm = new HomeModel
            {
                Simachot = db.GetSimachot(),
                TotalContribs = db.GetTotal()
            };

            if (TempData["message"] != null)
            {
                vm.Message = (string)TempData["message"];
            }
            return View(vm);
            
        } 

        [HttpPost]
        public IActionResult Add(Simcha s)
        {
            var db = new DBManager(_connectionString);
            db.AddASimcha(s);
            TempData["message"] = $"Mazal Tov! There is a new Simcha in the neighborhood";

            return Redirect("/");
        }

        public IActionResult Contributions(int simchaId)
        {
            var db = new DBManager(_connectionString);
            var vm = new ContributionModel
            { 
                Contributors = db.GetSimchaContributors(simchaId),
                SimchaId = simchaId,
                SimchaName = db.GetNameById(simchaId,"Simachot")
            };
            return View(vm);
             
        }

        [HttpPost]
        public IActionResult UpdateContribs(int simId, List<ContribIncusion> con)
        {
            var db = new DBManager(_connectionString);
            db.Delete(simId);
            var update = con.Where(c => c.Include).ToList();
            db.UpdateSimcha(simId, con);

            TempData["message"] = "Simcha updated successfully";
            return Redirect("/");
        }

    }
}
