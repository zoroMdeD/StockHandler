using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class Resistor : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string partNumber;
        private string resistance;
        private string power;
        private string accuracy;
        private string size;
        private int count;

        public string PartNumber
        {
            get
            {
                return partNumber;
            }
            set
            {
                partNumber = value;
                OnPropertyChanged("PurtNumber");
            }
        }
        public string Resistance
        {
            get
            {
                return resistance;
            }
            set
            {
                resistance = value;
                OnPropertyChanged("Resistance");
            }
        }
        public string Power
        {
            get
            {
                return power;
            }
            set
            {
                power = value;
                OnPropertyChanged("Power");
            }
        }
        public string Accuracy
        {
            get
            {
                return accuracy;
            }
            set
            {
                accuracy = value;
                OnPropertyChanged("Accuracy");
            }
        }
        public string Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                OnPropertyChanged("Count");
            }
        }

        public Resistor()
        {

        }

        public Resistor(ResistorModel resistor, int count)
        {
            //CapacitorModel temp = (CapacitorModel)components.GetComponent("ABCDEFGHIJK");
            //temp.
            PartNumber = resistor.PartNumber;
            Resistance = resistor.Resistance;
            Power = resistor.Power;
            Accuracy = resistor.Accuracy;
            Size = resistor.Size;
            Count = count;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
