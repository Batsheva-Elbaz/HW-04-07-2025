using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW_04_07_2025.Data
{
    public class Contributor
    {
        public int Id { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Cell { get; set; }
        public DateTime Date { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Contributions { get; set; }
        public decimal Balance { get; set; }
    }
}
