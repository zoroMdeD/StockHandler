using ApiClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class StorageComponents : IAction, IEventHandler, INotifyPropertyChanged
    {
        private List<StorageComponents> components = new List<StorageComponents>();
        private string type;
        private string partNumber;

        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }
        public string PartNumber
        {
            get { return partNumber; }
            set
            {
                partNumber = value;
                OnPropertyChanged("PartNumber");
            }
        }
        public List<StorageComponents> Components
        {
            get { return components; }
            private set
            {
                components = value;
                OnPropertyChanged("Components");
            }
        }

        public event EventHandler<ActionEventArgs> MessageHandler;

        /// <summary>
        /// Constructor for initializing
        /// </summary>
        public StorageComponents()
        {

        }
        /// <summary>
        /// Constructor for inheritors
        /// </summary>
        public StorageComponents(string type, string partNumber)
        {
            Type = type;
            PartNumber = partNumber;
            //components = new List<StorageComponents>();
        }

        public List<StorageComponents> GetAll()
        {
            return components;
        }
        public StorageComponents GetComponent(string partNumber)
        {
            if(partNumber.Length > 0)
                foreach (StorageComponents item in components)
                {
                    if (item.PartNumber == partNumber)
                        return item;
                }
            else
                MessageHandler?.Invoke(this, new ActionEventArgs("The string must not be empty."));
            return null;
        }
        public void Add(StorageComponents component)
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
