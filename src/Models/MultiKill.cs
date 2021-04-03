using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class MultiKill : TimeBased
    {
        public int Count { get; set; }
        public bool IsRunning { get; set; }

        public MultiKillType Type
        {
            get
            {
                if (Count >= 5)
                    return MultiKillType.MonsterKill;
                if (Count == 4)
                    return MultiKillType.UltraKill;
                if (Count == 3)
                    return MultiKillType.MultiKill;
                if (Count == 2)
                    return MultiKillType.DoubleKill;
                return MultiKillType.None;
            }
        }
    }

    public enum MultiKillType
    {
        None,
        DoubleKill,
        MultiKill,
        UltraKill,
        MonsterKill,
    }
}
