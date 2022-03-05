using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class CapacitorModel : StorageComponents
    {
        private string propertyOne;
        private string propertyTwo;
        private string propertyThree;
        private string size;
        private int count;

        /// <summary>
        /// Capacity
        /// </summary>
        public string PropertyOne
        {
            get { return propertyOne; }
            set
            {
                propertyOne = value;
                OnPropertyChanged("PropertyOne");
            }
        }
        /// <summary>
        /// Voltage
        /// </summary>
        public string PropertyTwo
        {
            get { return propertyTwo; }
            set
            {
                propertyTwo = value;
                OnPropertyChanged("PropertyTwo");
            }
        }
        /// <summary>
        /// TCoefficient
        /// </summary>
        public string PropertyThree
        {
            get { return propertyThree; }
            set
            {
                propertyThree = value;
                OnPropertyChanged("PropertyThree");
            }
        }
        public string Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                OnPropertyChanged("Count");
            }
        }
        public CapacitorModel()
        {

        }
        public CapacitorModel(string partNumber, string capacity, string voltage, string tCoefficient, string size, int count, int id, string type = "Capacitor") : base(type, partNumber, id)
        {
            Size = size;
            PropertyOne = capacity;
            PropertyTwo = voltage;
            PropertyThree = tCoefficient;
            Count = count;
        }
    }
}
