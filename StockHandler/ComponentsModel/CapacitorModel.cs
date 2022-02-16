using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class CapacitorModel : ComponentsInfo
    {
        private string Capacity { get; set; }
        private string Voltage { get; set; }
        private string TCoefficient { get; set; }
        private string Size { get; set; }

        public CapacitorModel(string type, string partNumber, string size, string capacity, string voltage, string tCoefficient) : base(type, partNumber)
        {
            Size = size;
            Capacity = capacity;
            Voltage = voltage;
            TCoefficient = tCoefficient;
        }
    }
}
