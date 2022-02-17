using ApiClient;
using ApiClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockHandler
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        StorageComponents storageComponents;
        ParserDigiKey parserDigiKey;
        CancellationTokenSource cts;

        ApplicationContext db;
        
        RelayCommand parsingCommand;
        RelayCommand saveCommand;
        RelayCommand findCommand;
        RelayCommand analogueCommand;
        RelayCommand selectCommand;
        
        RelayCommand fileCommand;
            RelayCommand openFileCommand;
            RelayCommand saveFileCommand;
            RelayCommand quitCommand;

        RelayCommand commandCommand;
        RelayCommand infoCommand;
        RelayCommand helpCommand;

        RelayCommand addCommand;
        RelayCommand editCommand;
        RelayCommand deleteCommand;
        
        IEnumerable<Resistor> resistors;
        IEnumerable<Capacitor> capacitors;

        public IEnumerable<Resistor> Resistors
        {
            get { return resistors; }
            set
            {
                resistors = value;
                OnPropertyChanged("resistors");
            }
        }
        public IEnumerable<Capacitor> Capacitors
        {
            get { return capacitors; }
            set
            {
                capacitors = value;
                OnPropertyChanged("capacitors");
            }
        }

        public ApplicationViewModel()
        {
            db = new ApplicationContext();
            db.Resistors.Load();
            db.Capacitors.Load();
            Resistors = db.Resistors.Local.ToBindingList();
            Capacitors = db.Capacitors.Local.ToBindingList();
        }
        // команда добавления
        public RelayCommand ParsingCommand
        {
            get
            {
                return parsingCommand ?? (parsingCommand = new RelayCommand((o) => 
                { 
                    TaskRun(); 
                }));
            }
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
            textBox1.AppendText(Environment.NewLine + "Parsing...");
            for (int i = 0; i < ProcessedPartNumbers.Count; i++)
            {
                if (token.IsCancellationRequested)
                {
                    if (!ExFlag)
                        textBox1.AppendText(Environment.NewLine + "Interrupted: The process was interrupted by the user.");
                    label1.Text = status;
                    p.Report(0);
                    CheckBtnParsing = false;
                    oAuthToolStripMenuItem.Enabled = true;
                    saveToolStripMenuItem.Enabled = true;
                    pathToolStripMenuItem.Enabled = true;
                    cts.Dispose();
                    return;
                }
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
            p.Report(i);

            }
            catch(HttpResponseException ex)
            {
                textBox1.AppendText(Environment.NewLine + "Request limit exceeded: " + ex.Message);
                label1.Text = "Error";
                CheckBtnParsing = false;
                oAuthToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                pathToolStripMenuItem.Enabled = true;
            }
            DocBuild.RunWorkerAsync(null);
        }
        void ShowAction(object sender, ActionEventArgs e)   //Метод для добавления в событие Action
        {
            //TextBoxLogOut.Text += Environment.NewLine + $"{e.Message}";
        }









        // команда добавления
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand((o) =>
                  {
                      PhoneWindow phoneWindow = new PhoneWindow(new Phone());
                      if (phoneWindow.ShowDialog() == true)
                      {
                          Phone phone = phoneWindow.Phone;
                          db.Phones.Add(phone);
                          db.SaveChanges();
                      }
                  }));
            }
        }
        // команда редактирования
        public RelayCommand EditCommand
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem == null) return;
                      // получаем выделенный объект
                      Phone phone = selectedItem as Phone;

                      Phone vm = new Phone()
                      {
                          Id = phone.Id,
                          Company = phone.Company,
                          Price = phone.Price,
                          Title = phone.Title
                      };
                      PhoneWindow phoneWindow = new PhoneWindow(vm);


                      if (phoneWindow.ShowDialog() == true)
                      {
                          // получаем измененный объект
                          phone = db.Phones.Find(phoneWindow.Phone.Id);
                          if (phone != null)
                          {
                              phone.Company = phoneWindow.Phone.Company;
                              phone.Title = phoneWindow.Phone.Title;
                              phone.Price = phoneWindow.Phone.Price;
                              db.Entry(phone).State = EntityState.Modified;
                              db.SaveChanges();
                          }
                      }
                  }));
            }
        }
        // команда удаления
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((selectedItem) =>
                  {
                      if (selectedItem == null) return;
                      // получаем выделенный объект
                      Phone phone = selectedItem as Phone;
                      db.Phones.Remove(phone);
                      db.SaveChanges();
                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
