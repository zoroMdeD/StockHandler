using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StockHandler
{
    /// <summary>
    /// Interaction logic for ResistorsWindow.xaml
    /// </summary>
    public partial class ResistorsWindow : Window
    {
        public Resistor Resistor { get; private set; }
        public ResistorsWindow(Resistor r)
        {
            InitializeComponent();
            Resistor = r;
            this.DataContext = Resistor;
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
