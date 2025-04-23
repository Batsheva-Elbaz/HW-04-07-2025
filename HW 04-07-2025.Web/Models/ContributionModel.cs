using HW_04_07_2025.Data;

namespace HW_04_07_2025.Web.Models
{
    public class ContributionModel
    {
        public List<Contributor> Contributors { get; set; }
        public int SimchaId { get; set; }
        public string SimchaName { get; set; }
    }
}