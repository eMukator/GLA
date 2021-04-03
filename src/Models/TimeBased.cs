using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogParser.Models
{
    public class TimeBased
    {
        public int StartInSeconds { get; set; }
        public int FinishInSeconds { get; set; }
        public TimeSpan Duration => TimeSpan.FromSeconds(FinishInSeconds - StartInSeconds);
    }
}
