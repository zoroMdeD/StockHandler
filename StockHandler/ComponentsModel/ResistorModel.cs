using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ResistorModel : StorageComponents
    {
        private string propertyOne;
        private string propertyTwo;
        private string propertyThree;
        private string size;
        private int count;

        /// <summary>
        /// Resistance
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
        /// Power
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
        /// Accuracy
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
        public ResistorModel(string partNumber, string resistance, string power, string accuracy, string size, int count, int id, string type = "Resistor") : base(type, partNumber, id)
        {
            PropertyOne = resistance;
            PropertyTwo = power;
            PropertyThree = accuracy;
            Size = size;
            Count = count;
        }
    }
}
