using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public interface IAction
    {
        List<Components> GetAll();
        Components GetComponent(string partNumber);
        void TryAddComponent(Components component);
        void TryRemoveComponent(string partNumber);
    }
}
