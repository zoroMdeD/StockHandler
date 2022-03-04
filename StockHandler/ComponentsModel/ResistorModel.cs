using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ResistorModel : StorageComponents
    {
        private string resistance;
        private string power;
        private string accuracy;
        private string size;
        private int count;

        public string Resistance
        {
            get { return resistance; }
            set
            {
                resistance = value;
                OnPropertyChanged("Resistance");
            }
        }
        public string Power
        {
            get { return power; }
            set
            {
                power = value;
                OnPropertyChanged("Power");
            }
        }
        public string Accuracy
        {
            get { return accuracy; }
            set
            {
                accuracy = value;
                OnPropertyChanged("Accuracy");
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
        public ResistorModel(string partNumber, string resistance, string power, string accuracy, string size, int count, int id, string type = "Resistor") : base(type, partNumber, id)
        {
            Resistance = resistance;
            Power = power;
            Accuracy = accuracy;
            Size = size;
            Count = count;
        }
    }
}
