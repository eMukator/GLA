using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class PlayerViewModel
    {
        public string Player { get; set; }
        public string GameType { get; set; }
        public string GameId { get; set; }
        public Log Data { get; set; }
    }

    public class GameViewModel
    {
        public string MapName { get; set; }
        public string GameType { get; set; }
        public int GameId { get; set; }
        public Log Data { get; set; }
    }

    public class MapViewModel
    {
        public string MapName { get; set; }
        public string GameType { get; set; }
        public Log Data { get; set; }
    }

    public class SearchViewModel
    {
        public string Player { get; set; }
        public IEnumerable<SearchPlayersViewModel> SearchResults { get; set; }
        public Log Data { get; set; }
    }


    public class SearchPlayersViewModel
    {
        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Grenades { get; set; }
        public int Melees { get; set; }
        public int HeadShots { get; set; }
        public int Deaths { get; set; }
        public int Suicides { get; set; }
        public double Efficiency { get; set; }
        public double FPH { get; set; }
        public double TTL { get; set; }
        public TimeSpan Time { get; set; }
    }


    public class HighScoreCareerHighsViewModel
    {
        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Suicides { get; set; }
        public int HeadShots { get; set; }
        public double FPH { get; set; }
        public int Wins { get; set; }
        public double Played { get; set; }
        public int Games { get; set; }
        public int DoubleKills { get; set; }
        public int MultiKills { get; set; }
        public int UltraKills { get; set; }
        public int MonsterKills { get; set; }
        public int KillingSprees { get; set; }
        public int Rampages { get; set; }
        public int Dominatings { get; set; }
        public int Unstoppables { get; set; }
        public int GodLikes { get; set; }
        public int PistolKills { get; set; }
        public int SniperRifleKills { get; set; }
        public int GrenadeKills { get; set; }
        public int MeleeKills { get; set; }
        public int PistolDeaths { get; set; }
        public int RifleDeaths { get; set; }
        public int GrenadeDeaths { get; set; }
        public int SniperRifleDeaths { get; set; }
    }
}
