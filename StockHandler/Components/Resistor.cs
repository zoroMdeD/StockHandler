using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class Resistor : Components
    {
        public string Resistance { get; private set; }
        public string Power { get; private set; }
        public string Accuracy { get; private set; }

        public Resistor(string type, string partNumber, string size, string resistance, string power, string accuracy) : base(type, partNumber, size)
        {
            Resistance = resistance;
            Power = power;
            Accuracy = accuracy;
        }
    }
}
