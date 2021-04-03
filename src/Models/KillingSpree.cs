using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class KillingSpree : TimeBased
    {
        public int Count { get; set; }
        public bool IsRunnig { get; set; }

        public KillingSpreeType Type
        {
            get
            {
                if (Count >= 25)
                    return KillingSpreeType.GodLike;
                if (Count >= 20)
                    return KillingSpreeType.Unstoppable;
                if (Count >= 15)
                    return KillingSpreeType.Dominating;
                if (Count >= 10)
                    return KillingSpreeType.Rampage;
                if (Count >= 5)
                    return KillingSpreeType.KillingSpree;
                return KillingSpreeType.None;
            }
        }
    }

    public enum KillingSpreeType
    {
        None,
        KillingSpree,
        Rampage,
        Dominating,
        Unstoppable,
        GodLike
    }
}
