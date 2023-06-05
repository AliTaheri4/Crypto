using System.Security.Permissions;

namespace MyProject.Models
{
    public class SpFilteringModel
    {
        public decimal  WinRate { get; set; }
        public int Trades { get; set; }
        public int Tp { get; set; }
        public int Sl{ get; set; }
    }
}
