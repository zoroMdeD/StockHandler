using ApiClient;
using ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
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
        ApplicationContext db;

        StorageComponents storageComponents;
        ParserDigiKey parserDigiKey;
        CancellationTokenSource cts;
        //List<string> ProcessedPartNumbers = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            db = new ApplicationContext();
            db.Capacitors.Load();
            db.Resistors.Load();
            //DataContext = db.Capacitors.Local.ToBindingList();
            //DataContext = db.Resistors.Local.ToBindingList();

            ComboBoxSelectType.Items.Add("Resistors");
            ComboBoxSelectType.Items.Add("Capacitors");

            storageComponents = new StorageComponents();

            storageComponents.MessageHandler += ShowAction;    //Добавляем метод для вывода на UI

            //Создаем новый компонент
            storageComponents.Add(new CapacitorModel("Capacitor", "ABCDEFGHIJK", "0805", "10u", "50V", "np0"));
            storageComponents.Add(new CapacitorModel("Capacitor", "WEQWQDQWEWEQQWEQ", "1206", "0.1u", "16V", "np0"));
            storageComponents.Add(new ResistorModel("Resistor", "VBFHGUEFKEO", "0402", "240Ohm", "0.125W", "5%"));

            //Открытие файла .xlsx
            ConnectToExcel connectionOne = new ConnectToExcel(@"D:\!!!ЗАКАЗ РАСХОДНОГО И ИНОГО МАТЕРИАЛА.xlsx");
            connectionOne.TryUpdateWorksheet(connectionOne);
            List<string> s = connectionOne.GetAllWorksheetNames();
        }
        void ShowAction(object sender, ActionEventArgs e)   //Метод для добавления в событие Action
        {
            TextBoxLogOut.Text += Environment.NewLine + $"{e.Message}";
        }
        private void Button_Parsing_Click(object sender, RoutedEventArgs e)
        {
            TaskRun();
        }
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Find_Click(object sender, RoutedEventArgs e)
        {
            //Upcasting и пример вывода полей класса
            CapacitorModel temp = (CapacitorModel)storageComponents.GetComponent("ABCDEFGHIJK");
        }
        private void Button_Analogue_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Кнопка \"Analogue\" нажата");
        }
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Кнопка \"Analogue\" нажата");
            if (ComboBoxSelectType.Text == "Resistors")
            {
                DataContext = db.Resistors.Local.ToBindingList();
                ResistorsWindow resistorssWindow = new ResistorsWindow(new Resistor());
                if (resistorssWindow.ShowDialog() == true)
                {
                    Resistor resistor = resistorssWindow.Resistor;
                    db.Resistors.Add(resistor);
                    db.SaveChanges();
                }
            }
            else if(ComboBoxSelectType.Text == "Capacitors")
            {
                DataContext = db.Capacitors.Local.ToBindingList();
                CapacitorsWindow capacitorsWindow = new CapacitorsWindow(new Capacitor());
                if (capacitorsWindow.ShowDialog() == true)
                {
                    Capacitor capacitor = capacitorsWindow.Capacitor;
                    db.Capacitors.Add(capacitor);
                    db.SaveChanges();
                }
            }
            else
            {
                MessageBox.Show("Выбери блять ЭКБ!!!");
            }
            /*
                DataContext = db.Capacitors.Local.ToBindingList();
                Capacitor capacitor = new Capacitor((CapacitorModel)storageComponents.GetComponent("ABCDEFGHIJK"), 1); 
                db.Capacitors.Add(capacitor);
                db.SaveChanges();

                DataContext = db.Resistors.Local.ToBindingList();
                Resistor resistor = new Resistor((ResistorModel)storageComponents.GetComponent("VBFHGUEFKEO"), 1);
                db.Resistors.Add(resistor);
                db.SaveChanges();
            */
        }
        async void TaskRun(/*string Path*/)
        {
            try
            {
                //Создаем здесь для того чтобы обрабатывать ивенты
                ApiClientSettings settings = ApiClientSettings.CreateFromConfigFile();
                ApiClientService client = new ApiClientService(settings);
                client.MessageHandler += ShowAction;
                parserDigiKey = new ParserDigiKey(client, settings);
                parserDigiKey.MessageHandler += ShowAction;

                await parserDigiKey.ParserInit();

                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                //ActionWithExcel ActionWithExcel = new ActionWithExcel();
                //ProcessedPartNumbers = Parser.FindSpecialSymbol(ActionWithExcel.UpdateExcelDoc(Path, 0));

                // since this is a UI event, instantiating the Progress class
                // here will capture the UI thread context
                var progress = new Progress<int>(i => ProgressBar.Value = i);
                ProgressBar.Minimum = 0;
                //ProgressBar.Maximum = (ProcessedPartNumbers.Count - 1) * 2;
                ProgressBar.Maximum = 2;
                // pass this instance to the background task
                _ = OutData(progress, token);
            }
            catch (Exception ex)
            {
                //textBox1.AppendText(Environment.NewLine + "Initialization error: " + ex.Message);
                //label1.Text = "Error";
                //CheckBtnParsing = false;
                //oAuthToolStripMenuItem.Enabled = true;
                //saveToolStripMenuItem.Enabled = true;
                //pathToolStripMenuItem.Enabled = true;
            }
        }
        async Task OutData(IProgress<int> p, CancellationToken token)
        {
            bool ExFlag = false;
            string status = "Ready";
            //textBox1.AppendText(Environment.NewLine + "Parsing...");
            //for (int i = 0; i < ProcessedPartNumbers.Count; i++)
            //{
                //if (token.IsCancellationRequested)
                //{
                //    if (!ExFlag)
                //        textBox1.AppendText(Environment.NewLine + "Interrupted: The process was interrupted by the user.");
                //    label1.Text = status;
                //    p.Report(0);
                //    CheckBtnParsing = false;
                //    oAuthToolStripMenuItem.Enabled = true;
                //    saveToolStripMenuItem.Enabled = true;
                //    pathToolStripMenuItem.Enabled = true;
                //    cts.Dispose();
                //    return;
                //}
                try
                {
                    await parserDigiKey.FindDescPack("CL05A104KA5NNNC");
                }
                catch (AggregateException)
                {
                    ExFlag = true;
                    //textBox1.AppendText(Environment.NewLine + "Request limit exceeded: 0 out of 1000 requests left");
                    cts.Cancel();
                    status = "Error";
                }
                catch (NullReferenceException)
                {
                    ExFlag = true;
                    //textBox1.AppendText(Environment.NewLine + "Authorization error: The token is outdated");
                    cts.Cancel();
                    status = "Error";
                }
                catch (Exception ex)
                {
                    ExFlag = true;
                    //textBox1.AppendText(Environment.NewLine + "Unhandled exception: Something went wrong " + ex.Message);
                    cts.Cancel();
                    status = "Error";
                }
                //p.Report(i);

            //}
            /*            catch(HttpResponseException ex)
{
    textBox1.AppendText(Environment.NewLine + "Request limit exceeded: " + ex.Message);
    label1.Text = "Error";
    CheckBtnParsing = false;
    oAuthToolStripMenuItem.Enabled = true;
    saveToolStripMenuItem.Enabled = true;
    pathToolStripMenuItem.Enabled = true;
}*/
            //DocBuild.RunWorkerAsync(null);
        }
    }
}
