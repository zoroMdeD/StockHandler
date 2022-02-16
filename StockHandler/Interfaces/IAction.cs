using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public interface IAction
    {
        List<ComponentsInfo> GetAll();
        ComponentsInfo GetComponent(string partNumber);
        void Add(ComponentsInfo component);
        void Remove(string partNumber);
    }
}
