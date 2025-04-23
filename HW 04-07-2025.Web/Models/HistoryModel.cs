using HW_04_07_2025.Data;

namespace HW_04_07_2025.Web.Models
{
    public class HistoryModel
    {
        public List<Transactions> Transactions { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}
