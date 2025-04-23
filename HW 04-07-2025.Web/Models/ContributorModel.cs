using HW_04_07_2025.Data;

namespace HW_04_07_2025.Web.Models
{
    public class ContributorModel
    {
        public List<Contributor> Contributors { get; set; }
        public decimal Total { get; set; }
        public string Message { get; set; }
    }
}