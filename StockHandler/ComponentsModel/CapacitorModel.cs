using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class CapacitorModel : StorageComponents
    {
        private string capacity;
        private string voltage;
        private string tCoefficient;
        private string size;
        private int count;

        public string Capacity
        {
            get { return capacity; }
            set
            {
                capacity = value;
                OnPropertyChanged("Capacity");
            }
        }
        public string Voltage
        {
            get { return voltage; }
            set
            {
                voltage = value;
                OnPropertyChanged("Voltage");
            }
        }
        public string TCoefficient
        {
            get { return tCoefficient; }
            set
            {
                tCoefficient = value;
                OnPropertyChanged("TCoefficient");
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
        public CapacitorModel(string partNumber, string capacity, string voltage, string tCoefficient, string size, int count,  string type = "Capacitor") : base(type, partNumber)
        {
            Size = size;
            Capacity = capacity;
            Voltage = voltage;
            TCoefficient = tCoefficient;
            Count = count;
        }
    }
}
