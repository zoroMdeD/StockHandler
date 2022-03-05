using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public interface IAction
    {
        List<StorageComponents> GetAll();
        StorageComponents GetComponent(string partNumber);
        void Add(StorageComponents component);
        void Remove(int id);
    }
}
