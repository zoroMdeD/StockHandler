using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient
{
    public interface IEventHandler
    {
        event EventHandler<ActionEventArgs> MessageHandler;  //Событие для оповещения действий над коллекцией
    }
}
