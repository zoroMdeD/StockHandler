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
    /// Interaction logic for ComponentsWindow.xaml
    /// </summary>
    public partial class CapacitorsWindow : Window
    {
        public Capacitor Capacitor { get; private set; }
        public CapacitorsWindow(Capacitor c)
        {
            InitializeComponent();
            Capacitor = c;
            this.DataContext = Capacitor;
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
