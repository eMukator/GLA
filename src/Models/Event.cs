using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class Event
    {
        public int No { get; set; }
        public int Time { get; set; }
        public string Player { get; set; }
        public string Team { get; set; }
        public string Weapon { get; set; }
        public int Damage { get; set; }
        public string DamageType { get; set; }
        public string DamageLocation { get; set; }
        public Player Owner { get; set; }
        public string Weapon2 { get; set; }
    }
}
