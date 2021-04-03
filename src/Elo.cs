using LogParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser
{
    public static class Elo
    {
        private const int KFACTOR = 16;


        public static (decimal, decimal) Calculate(decimal currentRatingA, decimal currentRatingB, decimal scoreA, decimal scoreB, int kFactorA = KFACTOR, int kFactorB = KFACTOR)
        {
            var exp = Predict(currentRatingA, currentRatingB);
            var na = currentRatingA + (kFactorA * (scoreA - exp.Item1));
            var nb = currentRatingB + (kFactorB * (scoreB - exp.Item2));
            return (na, nb);
        }


        public static (decimal, decimal) Predict(decimal currentRatingA, decimal currentRatingB)
        {
            var ea = 1 / (1 + (decimal)Math.Pow(10, (double)(currentRatingB - currentRatingA) / 400));
            var eb = 1 / (1 + (decimal)Math.Pow(10, (double)(currentRatingA - currentRatingB) / 400));
            return (ea, eb);
        }


        public static Dictionary<string, decimal> CalculateFor(IEnumerable<Game> games)
        {
            var table = games.SelectMany(x => x.Players).GroupBy(x => x.Name.PureName()).ToDictionary(x => x.Key, x => (decimal)500);

            foreach (var game in games.OrderBy(x => x.Id))
            {
                foreach (var kill in game.Players.SelectMany(x => x.Kills).OrderBy(x => x.Time))
                {
                    var killer = kill.Owner.Name.PureName();
                    var victim = kill.Player.PureName();
                    var newRank = Calculate(table[killer], table[victim], 1, 0);
                    table[killer] = newRank.Item1;
                    table[victim] = newRank.Item2;
                }
            }
            return table.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }


        public static Dictionary<string, (decimal, decimal)> CalculateDeltaFor(IEnumerable<Game> games)
        {
            var table = games.SelectMany(x => x.Players).GroupBy(x => x.Name.PureName()).ToDictionary(x => x.Key, x => (decimal)500);
            var tableOld = new Dictionary<string, decimal>();

            foreach (var game in games.OrderBy(x => x.Id))
            {
                tableOld = table.ToDictionary(x => x.Key, x => x.Value);

                foreach (var kill in game.Players.SelectMany(x => x.Kills).OrderBy(x => x.Time))
                {
                    var killer = kill.Owner.Name.PureName();
                    var victim = kill.Player.PureName();
                    var newRank = Calculate(table[killer], table[victim], 1, 0);
                    table[killer] = newRank.Item1;
                    table[victim] = newRank.Item2;
                }
            }

            var deltas = new Dictionary<string, (decimal, decimal)>();
            foreach (var item in table)
            {
                deltas[item.Key] = (item.Value, item.Value - tableOld[item.Key]);
            }

            return deltas.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

    }
}
