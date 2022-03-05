using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class Capacitor : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string partNumber;
        private string capacity;
        private string voltage;
        private string tCoefficient;
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
        public string Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                capacity = value;
                OnPropertyChanged("Capacity");
            }
        }
        public string Voltage
        {
            get
            {
                return voltage;
            }
            set
            {
                voltage = value;
                OnPropertyChanged("Voltage");
            }
        }
        public string TCoefficient
        {
            get
            {
                return tCoefficient;
            }
            set
            {
                tCoefficient = value;
                OnPropertyChanged("TCoefficient");
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

        public Capacitor()
        {

        }

        //public Capacitor(CapacitorModel capacitor, int count)
        //{
        //    //CapacitorModel temp = (CapacitorModel)components.GetComponent("ABCDEFGHIJK");
        //    //temp.
        //    PartNumber = capacitor.PartNumber;
        //    Capacity = capacitor.Capacity;
        //    Voltage = capacitor.Voltage;
        //    TCoefficient = capacitor.TCoefficient;
        //    Size = capacitor.Size;
        //    Count = count;
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
