using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ResistorModel : StorageComponents
    {
        public string Resistance { get; private set; }
        public string Power { get; private set; }
        public string Accuracy { get; private set; }
        public string Size { get; private set; }

        public ResistorModel(string type, string partNumber, string size, string resistance, string power, string accuracy) : base(type, partNumber)
        {
            Size = size;
            Resistance = resistance;
            Power = power;
            Accuracy = accuracy;
        }
    }
}
