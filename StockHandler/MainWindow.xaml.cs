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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockHandler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Components components;
        public MainWindow()
        {
            InitializeComponent();
            ComboBoxSelectType.Items.Add("Resistors");
            ComboBoxSelectType.Items.Add("Capacitors");

            //Capacitor capacitor = new Capacitor("Capacitor", "ABCDEFGHIJK", "0805", "10u", "50V", "np0");
            components = new Components();

            components.MessageHandler += ShowAction;    //Добавляем метод для вывода на UI

            //Создаем новый компонент
            components.TryAddComponent(new Capacitor("Capacitor", "ABCDEFGHIJK", "0805", "10u", "50V", "np0"));
            components.TryAddComponent(new Capacitor("Capacitor", "WEQWQDQWEWEQQWEQ", "1206", "0.1u", "16V", "np0"));
            components.TryAddComponent(new Resistor("Resistor", "VBFHGUEFKEO", "0402", "240Ohm", "0.125W", "5%"));


        }
        void ShowAction(object sender, ActionEventArgs e)   //Метод для добавления в событие Action
        {
            TextBoxLogOut.Text += Environment.NewLine + $"{e.Message}";
        }
        private void Button_Parsing_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Кнопка \"Parsing\" нажата");
        }
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Кнопка \"Save\" нажата");

            //Удаляем компонент
            components.TryRemoveComponent("ABCDEFGHIJK");
        }
        private void Button_Find_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Кнопка \"Find\" нажата");

            //Upcasting и пример вывода полей класса
            Capacitor temp = (Capacitor)components.GetComponent("ABCDEFGHIJK");
            Resistor temp1 = (Resistor)components.GetComponent("VBFHGUEFKEO");
            Capacitor temp2 = (Capacitor)components.GetComponent("WEQWQDQWEWEQQWEQ");

            TextBoxComponentInfo.Text += Environment.NewLine + temp.Type
                                       + Environment.NewLine + temp.PartNumber
                                       + Environment.NewLine + temp.Size
                                       + Environment.NewLine + temp.Voltage
                                       + Environment.NewLine + temp.TCoefficient;

            TextBoxComponentInfo.Text += Environment.NewLine + temp2.Type
                                       + Environment.NewLine + temp2.PartNumber
                                       + Environment.NewLine + temp2.Size
                                       + Environment.NewLine + temp2.Voltage
                                       + Environment.NewLine + temp2.TCoefficient;

            TextBoxComponentInfo.Text += Environment.NewLine + temp1.Type
                                       + Environment.NewLine + temp1.PartNumber
                                       + Environment.NewLine + temp1.Size
                                       + Environment.NewLine + temp1.Resistance
                                       + Environment.NewLine + temp1.Accuracy;
        }
        private void Button_Analogue_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Кнопка \"Analogue\" нажата");
        }
    }


}
