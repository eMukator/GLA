using LogParser.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogParser.Parsers
{
    public class CoD2Parser : IParser
    {
        readonly string LinePattern = @"^\s*(\d+):(\d{2})\s+(.*)$";

        IMemoryCache cache;
        IConfiguration configuration;
        string file;
        int cacheInMinutes;

        public string ParserType => "cod2";


        public CoD2Parser(IMemoryCache cache, IConfiguration configuration)
        {
            this.cache = cache;
            this.configuration = configuration;
            this.file = configuration.GetValue<string>("PathToLogFileCod");
            this.cacheInMinutes = configuration.GetValue<int>("CacheInMinutes", 5);
        }


        public void SelectFile(string file)
        {
            this.file = file;
        }


        public async Task<Log> GetData()
        {
            if (String.IsNullOrWhiteSpace(file))
                throw new ArgumentException($"Log file not selected. Use 'SelectFile()' first.");

            if (!File.Exists(file))
                throw new ArgumentException($"Log file '{file}' not found.");

            return await cache.GetOrCreateAsync("Log:{file}", async (x) =>
            {
                x.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheInMinutes);
                return await ParseLog(file);
            });
        }


        async Task<Log> ParseLog(string file)
        {
            var log = new Log(ParserType);
            var lastTime = 0;
            var deltaTime = 0;

            try
            {
                Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // running on windows with czech locale
                    using (var reader = new StreamReader(stream, Encoding.GetEncoding(1250), true))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            var match = Regex.Match(line, LinePattern);
                            if (!match.Success || match.Groups.Count < 3 || line.EndsWith("-----"))
                                continue;

                            var msg = match.Groups[3].Value;
                            var time = NormalizeTime(match.Groups[1].Value, match.Groups[2].Value, ref deltaTime, lastTime);
                            lastTime = time;

                            ProcessGameRow(log, time, msg);
                            ProcessEventRow(log, time, msg);
                        }
                    }
                }

                log.RemoveUselessData();

                return log;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        int NormalizeTime(string minutes, string seconds, ref int deltaTime, int lastTime)
        {
            var time = int.Parse(minutes) * 60 + int.Parse(seconds) + deltaTime;
            if (time < lastTime)
            {
                var diffTime = lastTime - time;
                deltaTime += diffTime;
                time += diffTime;
            }
            return time;
        }


        void ProcessGameRow(Log log, int time, string msg)
        {
            if (!Regex.IsMatch(msg, @"^\w+:.*$"))
                return;

            // InitGame: \g_antilag\1\g_gametype\ctf\gamename\Call of Duty 2\mapname\mp_decoy\protocol\118\scr_friendlyfire\0\scr_killcam\1\shortversion\1.3\sv_allowAnonymous\0\sv_floodProtect\1\sv_hostname\uPipky\sv_maxclients\20\sv_maxPing\0\sv_maxRate\0\sv_minPing\0\sv_privateClients\0\sv_punkbuster\0\sv_pure\1\sv_voice\0
            if (msg.StartsWith("InitGame:"))
                log.AddGame(time, msg.SplitToDictionary("\\", 1));

            if (msg.StartsWith("ShutdownGame:"))
                log.FinishGame(time);
        }


        void ProcessEventRow(Log log, int time, string msg)
        {
            if (!Regex.IsMatch(msg, @"^\w+;.*$"))
                return;

            var parts = msg.Split(';', StringSplitOptions.None);

            // D;0;5;allies;^1S^2t^3r^4a^5š^6i^7d^8l^9o;0;1;axis;Kouda;g43_mp;45;MOD_RIFLE_BULLET;torso_lower
            if (msg.StartsWith("D;"))
                log.UpdateDamages(time, parts[7], parts[8], parts[3], parts[4], parts[9], parts[10], parts[11], parts[12]);

            // K;0;5;allies;^1S^2t^3r^4a^5š^6i^7d^8l^9o;0;1;axis;Kouda;g43_mp;45;MOD_RIFLE_BULLET;torso_lower
            // K;0;1;;^1S^2t^3r^4a^5š^6i^7d^8l^9o;0;1;;^1S^2t^3r^4a^5š^6i^7d^8l^9o;none;100000;MOD_SUICIDE;none
            if (msg.StartsWith("K;"))
                log.UpdateKills(time, parts[7], parts[8], parts[3], parts[4], parts[9], parts[10], parts[11], parts[12]);

            // W;axis;0;Alex;0;HOVADO
            if (msg.StartsWith("W;"))
                log.UpdateWinLoss(time, parts[1], parts.Where((x, y) => y % 2 == 1).ToArray().Skip(1).ToArray(), true);

            // L;allies;0;Pipa;0;Kouda
            if (msg.StartsWith("L;"))
                log.UpdateWinLoss(time, parts[1], parts.Where((x, y) => y % 2 == 1).ToArray().Skip(1).ToArray(), false);

            // J;0;0;Pipa
            if (msg.StartsWith("J;"))
                log.AddPlayer(time, parts[3]);

            // Q;0;1;Alexisko
            if (msg.StartsWith("Q;"))
                log.PlayerQuit(time, parts[3]);

            // Weapon;0;1;Alexisko;frag_grenade_german_mp
            if (msg.StartsWith("Weapon;"))
                log.UpdateWeapon(time, parts[3], parts[4]);

            // say;0;0;Pipa;:D :D :D
            if (msg.StartsWith("say;"))
                log.PlayerSay(time, parts[3], false);

            // sayteam;0;0;Pipa;QUICKMESSAGE_FOLLOW_ME
            if (msg.StartsWith("sayteam;"))
                log.PlayerSay(time, parts[3], true);
        }

    }
}
