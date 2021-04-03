using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class Player : TimeBased
    {
        public string Name { get; set; }
        public string TeamName { get; set; }
        public int Sucides { get; set; }
        public int Say { get; set; }
        public int SayTeam { get; set; }
        public bool Win { get; set; }
        public bool IsPlaying { get; set; }
        public bool LeftGame { get; set; }
        public string WeaponInHand { get; set; }
        public IList<Event> Kills { get; set; }
        public IList<Event> Deaths { get; set; }
        public IList<Event> DamagesFrom { get; set; }
        public IList<Event> DamagesTo { get; set; }
        public IList<KillingSpree> KillingSprees { get; set; }
        public IList<MultiKill> MultiKills { get; set; }
        public KillingSpree LastKillingSpree => KillingSprees.Last();
        public MultiKill LastMultiKill => MultiKills.Last();
        public IList<Weapon> Weapons { get; set; }


        public Player()
        {
            Kills = new List<Event>();
            Deaths = new List<Event>();
            DamagesFrom = new List<Event>();
            DamagesTo = new List<Event>();
            KillingSprees = new List<KillingSpree>();
            MultiKills = new List<MultiKill>();
            Weapons = new List<Weapon>();
        }


        public void SetTeamName(string name)
        {
            if (String.IsNullOrWhiteSpace(TeamName) && !String.IsNullOrWhiteSpace(name))
                TeamName = name;
        }


        public void SetFinish(int time, bool leftGame = false)
        {
            if (LeftGame)
                return;

            FinishInSeconds = time;
            if (leftGame)
                LeftGame = true;
        }


        public void SetSay(int time)
        {
            Say++;
            SetFinish(time);
        }


        public void SetSayTeam(int time)
        {
            SayTeam++;
            SetFinish(time);
        }


        public void SetIsPlaying()
        {
            IsPlaying = true;
        }


        public void SetWeapon(string weapon)
        {
            if (weapon.Contains("grenade"))
                return;
            WeaponInHand = weapon;
        }


        public void AddKill(int time)
        {
            AddOrContinueMultiKills(time);
            AddOrContinueKillingSprees(time);
        }


        public void AddDeath(int time)
        {
            EndKillingSprees(time);
            EndMultiKills(time);
        }


        public void AddOrContinueMultiKills(int time)
        {
            if (!MultiKills.Any())
            {
                MultiKills.Add(new MultiKill { StartInSeconds = time, Count = 1, FinishInSeconds = time, IsRunning = true });
                return;
            }

            if (LastMultiKill.IsRunning && TimeSpan.FromSeconds(time - LastMultiKill.FinishInSeconds).TotalSeconds <= 3)
            {
                LastMultiKill.Count++;
                LastMultiKill.FinishInSeconds = time;
                return;
            }

            LastMultiKill.IsRunning = false;
            MultiKills.Add(new MultiKill { StartInSeconds = time, Count = 1, FinishInSeconds = time, IsRunning = true });
        }


        public void AddOrContinueKillingSprees(int time)
        {
            if (!KillingSprees.Any())
            {
                KillingSprees.Add(new KillingSpree { StartInSeconds = time, Count = 1, FinishInSeconds = time, IsRunnig = true });
                return;
            }

            if (LastKillingSpree.IsRunnig)
            {
                LastKillingSpree.Count++;
                LastKillingSpree.FinishInSeconds = time;
                return;
            }

            KillingSprees.Add(new KillingSpree { StartInSeconds = time, Count = 1, FinishInSeconds = time, IsRunnig = true });
        }


        public void EndKillingSprees(int time, bool IsEndGame = false)
        {
            if (!KillingSprees.Any())
                return;

            LastKillingSpree.FinishInSeconds = time;
            LastKillingSpree.IsRunnig = false;
        }


        public void EndMultiKills(int time, bool IsEndGame = false)
        {
            if (!MultiKills.Any())
                return;

            LastMultiKill.IsRunning = false;
            if (IsEndGame)
                LastMultiKill.FinishInSeconds = time;
        }

        public void AddWeaponAction(string name, bool meFired, bool isKill)
        {
            var w = Weapons.FirstOrDefault(x => x.Name == name);
            if (w == null)
            {
                w = new Weapon { Name = name };
                Weapons.Add(w);
            }

            if (meFired)
            {
                if (isKill)
                    w.Kills++;
                else
                    w.Hits++;
            }
            else
            {
                if (isKill)
                    w.Deaths++;
                else
                    w.Hitted++;
            }
        }
    }
}
