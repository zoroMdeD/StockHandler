using ApiClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ComponentsInfo : IAction, IEventHandler
    {
        public List<ComponentsInfo> components { get; set; }
        public string Type { get; private set; }
        public string PartNumber { get; private set; }

        public event EventHandler<ActionEventArgs> MessageHandler;

        /// <summary>
        /// Constructor for initializing
        /// </summary>
        public ComponentsInfo()
        {
            components = new List<ComponentsInfo>();
        }
        /// <summary>
        /// Constructor for the enumeration
        /// </summary>
        public ComponentsInfo(List<ComponentsInfo> components)
        {
            this.components = components;
        }
        /// <summary>
        /// Constructor for inheritors
        /// </summary>
        public ComponentsInfo(string type, string partNumber)
        {
            Type = type;
            PartNumber = partNumber;
        }

        public List<ComponentsInfo> GetAll()
        {
            return components;
        }
        public ComponentsInfo GetComponent(string partNumber)
        {
            if(partNumber.Length > 0)
                foreach (ComponentsInfo item in components)
                {
                    if (item.PartNumber == partNumber)
                        return item;
                }
            else
                MessageHandler?.Invoke(this, new ActionEventArgs("The string must not be empty."));
            return null;
        }
        public void Add(ComponentsInfo component)
        {
            components.Add(component);
            MessageHandler?.Invoke(this, new ActionEventArgs("Component added."));
        }
        public void Remove(string partNumber)
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
