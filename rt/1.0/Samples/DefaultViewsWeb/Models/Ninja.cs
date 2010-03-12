using System.ComponentModel;
namespace DefaultViewsWeb.Models
{
    public class Ninja
    {
        public string Name { get; set; }
        [DisplayName("Shurikens")]
        public int ShurikenCount { get; set; }
        [DisplayName("Blowgun Darts")]
        public int BlowgunDartCount { get; set; }
        public string  Clan { get; set; }
    }
}
