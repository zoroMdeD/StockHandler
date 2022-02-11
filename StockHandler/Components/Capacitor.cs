using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class Capacitor : Components
    {
        public string Capacity { get; private set; }
        public string Voltage { get; private set; }
        public string TCoefficient { get; private set; }

        public Capacitor(string type, string partNumber, string size, string capacity, string voltage, string tCoefficient) : base(type, partNumber, size)
        {
            Capacity = capacity;
            Voltage = voltage;
            TCoefficient = tCoefficient;
        }
    }
}
