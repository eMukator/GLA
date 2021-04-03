using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Hits { get; set; }
        public int Hitted { get; set; }
    }
}
