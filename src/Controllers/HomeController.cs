using LogParser.Models;
using LogParser.Parsers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Controllers
{
    public class HomeController : Controller
    {
        protected IParser parser;
        private readonly ILogger<HomeController> logger;
        protected Models.Log Data;

        public HomeController(ILogger<HomeController> logger, IParser parser)
        {
            this.logger = logger;
            this.parser = parser;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Index()
        {
            var log = await parser.GetData();
            return View(log);
        }


        public async Task<IActionResult> MostRecent()
        {
            var log = await parser.GetData();
            return View(log);
        }


        public async Task<IActionResult> HighScores()
        {
            var log = await parser.GetData();
            return View(log);
        }


        public async Task<IActionResult> Player(string id)
        {
            return View(new PlayerViewModel
            {
                Player = id,
                Data = await parser.GetData()
            });
        }


        public async Task<IActionResult> Map(string id, string type)
        {
            return View(new MapViewModel
            {
                MapName = id,
                GameType = type,
                Data = await parser.GetData()
            });
        }


        public async Task<IActionResult> LastGame()
        {
            var log = await parser.GetData();
            var gameId = log.Games.LastOrDefault()?.Id ?? 0;
            return await Game(gameId);
        }


        public async Task<IActionResult> Search(string player)
        {
            var log = await parser.GetData();
            var p = player.PureName().ToLower();
            var names = log.Games
                .SelectMany(x => x.Players)
                .GroupBy(x => x.Name)
                .Where(x => x.Key.PureName().ToLower().Contains(p))
                .Select(x => x.Key);

            var results = log.Games
                .SelectMany(x => x.Players)
                .GroupBy(x => x.Name)
                .Where(x => names.Contains(x.Key))
                .OrderByDescending(x => x.Sum(y => y.Kills.Count) - x.Sum(y => y.Sucides))
                .ThenByDescending(x => x.Sum(y => y.Kills.Count))
                .Select(player => new SearchPlayersViewModel
                {
                    Name = player.First().Name,
                    Frags = player.Sum(x => x.Kills.Count) - player.Sum(x => x.Sucides),
                    Kills = player.Sum(y => y.Kills.Count),
                    Grenades = player.Sum(y => y.Kills.Count(x => x.DamageType == "MOD_GRENADE_SPLASH")),
                    Melees = player.Sum(y => y.Kills.Count(x => x.DamageType == "MOD_MELEE")),
                    HeadShots = player.Sum(y => y.Kills.Count(x => x.DamageType == "MOD_HEAD_SHOT")),
                    Deaths = player.Sum(y => y.Deaths.Count),
                    Suicides = player.Sum(y => y.Sucides),
                    Efficiency = Math.Round((double)player.Sum(y => y.Kills.Count) / (player.Sum(y => y.Kills.Count) + player.Sum(y => y.Deaths.Count) + player.Sum(y => y.Sucides)) * 100, 1),
                    FPH = Math.Round((double)(player.Sum(y => y.Kills.Count) - player.Sum(y => y.Sucides)) / player.Sum(y => y.Duration.TotalHours), 1),
                    TTL = Math.Round(player.Sum(y => y.Duration.TotalSeconds) / (player.Sum(y => y.Deaths.Count) + player.Sum(y => y.Sucides)), 1),
                    Time = TimeSpan.FromSeconds(player.Sum(y => y.Duration.TotalSeconds)),
                });

            return View(new SearchViewModel
            {
                Player = player,
                SearchResults = results,
                Data = log
            });
        }


        public async Task<IActionResult> Game(int id)
        {
            var log = await parser.GetData();
            var game = log.Games.Where(x => x.Id == id);

            if (game == null)
                return NotFound();

            return View(nameof(Game), new GameViewModel { 
                GameId = id,
                Data = log,
            });
        }
    }


}