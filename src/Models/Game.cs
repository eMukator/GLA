using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class Game : TimeBased
    {
        public int Id { get; set; }
        public IList<Player> Players { get; set; }
        public bool IsEmpty => !Players.Any();
        public IDictionary<string, string> Params { get; set; }
        public string WonPlayer { get; set; }
        public string WonTeam { get; set; }
        public bool IsTeamGame => !String.IsNullOrWhiteSpace(WonTeam);


        public IList<Team> Teams
        {
            get
            {
                return Players
                    .GroupBy(x => x.TeamName)
                    .Select(x => new Team
                    {
                        Name = x.Key,
                        Players = x.ToList()
                    })
                    .ToList();
            }
        }


        public Game()
        {
            Players = new List<Player>();
            Params = new Dictionary<string, string>();
        }

    }
}
