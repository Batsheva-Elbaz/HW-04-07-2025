using HW_04_07_2025.Data;

namespace HW_04_07_2025.Web.Models
{
    public class HomeModel
    {
        public List<Simcha> Simachot { get; set; }
        public int TotalContribs { get; set; }
        public string Message { get; set; }
    }
}
