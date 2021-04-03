using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class Log
    {
        public string Source { get; private set; }
        public DateTime Created { get; private set; }
        public Game LastGame { get; private set; }
        public IList<Game> Games { get; set; }


        public Log(string source)
        {
            Source = source;
            Games = new List<Game>();
        }


        public Game GetLastGame()
        {
            return Games.Last();
        }


        public Player GetPlayerByName(string name)
        {
            var pure = name.PureName();
            return GetLastGame().Players.FirstOrDefault(x => x.Name == pure);
        }


        public Game AddGame(int time, IDictionary<string, string> gameParams)
        {
            var game = new Game
            {
                Id = Games.Count,
                StartInSeconds = time,
                Params = gameParams
            };

            Games.Add(game);
            return game;
        }


        public Game FinishGame(int time)
        {
            var game = GetLastGame();
            game.FinishInSeconds = time;

            foreach (var player in game.Players)
            {
                player.SetFinish(time);
                player.EndKillingSprees(time, true);
                player.EndMultiKills(time, true);
            }

            return game;
        }


        public Player AddPlayer(int time, string playerName)
        {
            var game = GetLastGame();
            var pl = game.Players.FirstOrDefault(x => x.Name == playerName.PureName());
            if (pl == null)
            {
                var player = new Player { Name = playerName.PureName(), StartInSeconds = time };
                game.Players.Add(player);
            }
            else
                pl.SetFinish(time);

            return pl;
        }


        public void UpdateWinLoss(int time, string teamName, string[] players, bool isWin)
        {
            foreach (var player in players)
            {
                var pl = GetPlayerByName(player);
                if (pl == null)
                    continue;
                pl.SetFinish(time);
                pl.SetTeamName(teamName);
                pl.Win = isWin;
            }

            if (isWin)
            {
                var game = GetLastGame();
                if (!String.IsNullOrWhiteSpace(teamName))
                    game.WonTeam = teamName;
                else
                    game.WonPlayer = players.FirstOrDefault();
            }
        }


        public void UpdateDamages(int time, string killerTeamName, string killerName, string victimTeamName, string victimName, string weapon, string damage, string damageType, string damageLocation)
        {
            var victim = GetPlayerByName(victimName);
            var killer = GetPlayerByName(killerName);
            if (victim == null || killer == null)
                return;

            victim.AddWeaponAction(weapon, false, false);
            killer.AddWeaponAction(weapon, true, false);

            victim.DamagesFrom.Add(new Event
            {
                Time = time,
                Team = killerTeamName,
                Player = killerName,
                Weapon = weapon,
                Damage = int.Parse(damage),
                DamageType = damageType,
                DamageLocation = damageLocation
            });

            killer.DamagesTo.Add(new Event
            {
                Time = time,
                Team = victimTeamName,
                Player = victimName,
                Weapon = weapon,
                Damage = int.Parse(damage),
                DamageType = damageType,
                DamageLocation = damageLocation
            });

            victim.SetTeamName(victimTeamName);
            victim.SetFinish(time);
            victim.SetIsPlaying();

            killer.SetWeapon(weapon);
            killer.SetTeamName(killerTeamName);
            killer.SetFinish(time);
            killer.SetIsPlaying();
        }


        public void UpdateKills(int time, string killerTeamName, string killerName, string victimTeamName, string victimName, string weapon, string damage, string damageType, string damageLocation)
        {
            var victim = GetPlayerByName(victimName);
            var killer = GetPlayerByName(killerName);
            if (victim == null || killer == null)
                return;

            if (killerName == victimName)
            {
                victim.Sucides++;
                return;
            }

            victim.AddWeaponAction(weapon, false, true);
            killer.AddWeaponAction(weapon, true, true);

            victim.Deaths.Add(new Event
            {
                No = victim.Deaths.Count + 1,
                Time = time,
                Team = killerTeamName,
                Player = killerName,
                Weapon = weapon,
                Damage = int.Parse(damage),
                DamageType = damageType,
                DamageLocation = damageLocation,
                Owner = victim,
                Weapon2 = victim.WeaponInHand
            });

            killer.Kills.Add(new Event
            {
                No = killer.Kills.Count + 1,
                Time = time,
                Team = victimTeamName,
                Player = victimName,
                Weapon = weapon,
                Damage = int.Parse(damage),
                DamageType = damageType,
                DamageLocation = damageLocation,
                Owner = killer,
                Weapon2 = victim.WeaponInHand
            });

            victim.SetTeamName(victimTeamName);
            victim.SetFinish(time);
            victim.SetIsPlaying();
            victim.AddDeath(time);

            killer.SetWeapon(weapon);
            killer.SetTeamName(killerTeamName);
            killer.SetFinish(time);
            killer.SetIsPlaying();
            killer.AddKill(time);
        }


        public void UpdateWeapon(int time, string playerName, string weapon)
        {
            var player = GetPlayerByName(playerName);
            if (player == null)
                return;
            player.SetWeapon(weapon);
        }


        public void RemoveUselessData()
        {
            // update last game for info
            LastGame = GetLastGame();
            Created = DateTime.Now;

            for (var i = Games.Count - 1; i >= 0; i--)
            {
                var game = Games[i];
                if (game.IsEmpty || game.Duration.TotalMinutes < 5)
                {
                    Games.RemoveAt(i);
                    continue;
                }

                // remove idle players
                for (var j = game.Players.Count - 1; j >= 0; j--)
                {
                    var player = game.Players[j];
                    if (!player.IsPlaying)
                    {
                        game.Players.RemoveAt(j);
                        continue;
                    }

                    // remove unfinshed killing spree
                    for (var k = player.KillingSprees.Count - 1; k >= 0; k--)
                        if (player.KillingSprees[k].Type == KillingSpreeType.None)
                            player.KillingSprees.RemoveAt(k);

                    // remove unfinshed multikills
                    for (var k = player.MultiKills.Count - 1; k >= 0; k--)
                        if (player.MultiKills[k].Type == MultiKillType.None)
                            player.MultiKills.RemoveAt(k);
                }

                // remove empty games
                if (game.Players.Count == 0)
                    Games.RemoveAt(i);
            }
        }


        public Player PlayerQuit(int time, string playerName)
        {
            var player = GetPlayerByName(playerName);
            player?.SetFinish(time, true);
            return player;
        }


        public Player PlayerSay(int time, string playerName, bool isTeamSay)
        {
            var player = GetPlayerByName(playerName);
            if (isTeamSay)
                player?.SetSayTeam(time);
            else
                player?.SetSay(time);
            return player;
        }

    }
}
