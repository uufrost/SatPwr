using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frost.Battery
{
    class Battery
    {
        public double totalCapacity; //最大容量
        public const int maxCycle = 1000; //最大充电循环

        public double CurrentCapacity { get; set; }
        public double Volt { get; set; }
        public int CurrentCycle{ get; set; }

        public Battery(double v)
        {
            Volt = v;
        }
    }
}
