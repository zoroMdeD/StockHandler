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
        
        public int Id { get; set; }
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
            }
        }
        public string PartNumber
        {
            get { return partNumber; }
            set
            {
                partNumber = value;
                OnPropertyChanged("PurtNumber");
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
        public StorageComponents(string type, string partNumber, int id)
        {
            Id = id;
            Type = type;
            PartNumber = partNumber;
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
                MessageHandler?.Invoke(this, new ActionEventArgs("The string must not be empty"));
            return null;
        }
        public void Add(StorageComponents component)
        {
            components.Add(component);
            //MessageHandler?.Invoke(this, new ActionEventArgs("Component added."));
        }
        public void Remove(int id)
        {
            if (id >= 0)
                for (int i = 0; i < components.Count; i++)
                {
                    if (components[i].Id == id)
                    {
                        components.Remove(components[i]);
                        break;
                    }
                }
            else
                MessageHandler?.Invoke(this, new ActionEventArgs("The Id must not be less than zero"));
        }
        public void Clear()
        {
            if(components.Count > 0)
                components.Clear();
        }
        public void Edit(StorageComponents component, int id)
        {
            int index = components.FindIndex(find => find.Id == id);
            components[index] = component;
        }
        public int GetIndex(List<StorageComponents> oldComponents, int id)
        {
            int index = oldComponents.FindIndex(find => find.Id == id);

            return index;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
