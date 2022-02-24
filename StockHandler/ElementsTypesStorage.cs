using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ElementsTypesStorage : ObservableCollection<string>
    {
        public ElementsTypesStorage()
        {
            Add("Resistors");
            Add("Capacitors");
        }
    }
}
