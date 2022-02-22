using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ElementsTypes : ObservableCollection<string>
    {
        public ElementsTypes()
        {
            Add("Resistors");
            Add("Capacitors");
        }
    }
}
