using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class Components : IAction
    {
        protected List<Components> components { get; set; }
        public string Type { get; private set; }
        public string PartNumber { get; private set; }
        public string Size { get; private set; }
        public event EventHandler<ActionEventArgs> MessageHandler;  //Событие для оповещения действий над коллекцией

        public Components()
        {
            components = new List<Components>();
        }
        public Components(string type, string partNumber, string size)
        {
            Type = type;
            PartNumber = partNumber;
            Size = size;
        }
        public List<Components> GetAll()
        {
            return components;
        }
        public Components GetComponent(string partNumber)
        {
            if(partNumber.Length > 0)
                foreach (Components item in components)
                {
                    if (item.PartNumber == partNumber)
                        return item;
                }
            else
                MessageHandler?.Invoke(this, new ActionEventArgs("The string must not be empty."));
            return null;
        }
        public void TryAddComponent(Components component)
        {
            components.Add(component);
            MessageHandler?.Invoke(this, new ActionEventArgs("Component added."));
        }
        public void TryRemoveComponent(string partNumber)
        {
            if (partNumber.Length > 0)
                for (int i = 0; i < components.Count; i++)
                {
                    if (components[i].PartNumber == partNumber)
                    {
                        components.Remove(components[i]);
                        MessageHandler?.Invoke(this, new ActionEventArgs($"Component with part number {partNumber} removed."));
                        break;
                    }
                }
            else
                MessageHandler?.Invoke(this, new ActionEventArgs("The string must not be empty."));
        }
    }
}
