using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class StorageTypes
    {
        public List<string> Types { get; private set; }

        public StorageTypes()
        {
            Types = new List<string>();
        }
        public void Add(string type)
        {
            Types.Add(type);
        }
    }
}
