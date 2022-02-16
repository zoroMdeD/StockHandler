using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ResistorModel : StorageComponents
    {
        private string Resistance { get; set; }
        private string Power { get; set; }
        private string Accuracy { get; set; }
        private string Size { get; set; }

        public ResistorModel(string type, string partNumber, string size, string resistance, string power, string accuracy) : base(type, partNumber)
        {
            Size = size;
            Resistance = resistance;
            Power = power;
            Accuracy = accuracy;
        }
    }
}
