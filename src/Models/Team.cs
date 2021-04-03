using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class Team
    {
        public string Name { get; set; }
        public IList<Player> Players { get; set; }

        public Team()
        {
            Players = new List<Player>();
        }
    }
}
