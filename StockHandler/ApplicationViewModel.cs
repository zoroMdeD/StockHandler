﻿using ApiClient;
using ApiClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockHandler
{
    public class ApplicationViewModel : INotifyPropertyChanged, IEventHandler
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ActionEventArgs> MessageHandler;

        private List<string> listOfComponents;
        private StorageComponents storageComponents;
        static BackgroundWorker DocBuild;

        private ApplicationContext db;

        RelayCommand parsingCommand;    //Комманда парсинг
        RelayCommand saveCommand;       //Комманда сохранения
        RelayCommand findCommand;       //Комманда поиска
        RelayCommand analogueCommand;   //Комманда подбора аналога
        RelayCommand selectCommand;     //Комманда выбора типа элемента

        //-------------Menu strip-------------
        RelayCommand fileCommand;       //Комманда открыть меню Файл   
        RelayCommand openFileCommand;   //Комманда открыть файл
        RelayCommand saveFileCommand;   //Комманда сохранить файл
        RelayCommand quitCommand;       //Комманда выход
        
        RelayCommand commandCommand;    //Комманда открыть меню комманды
        RelayCommand infoCommand;       //Комманда открыть меню информация
        RelayCommand helpCommand;       //Комманда открыть меню помощь
        //-----------End Menu strip-----------

        RelayCommand addCommand;
        RelayCommand editCommand;
        RelayCommand deleteCommand;
        
        IEnumerable<Resistor> resistors;
        IEnumerable<Capacitor> capacitors;

        #region Fields for ProgressBar
        private int currentProgress;
        private int minValue;
        private int maxValue;
        #endregion
        
        #region Fields for TextBoxLogOut
        private string textLogOut;
        #endregion
        #region Fields for ComboBoxSelectType
        private string textType;
        #endregion

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

            listOfComponents = new List<string>();
            MessageHandler += ShowAction;

            DocBuild = new BackgroundWorker();
            DocBuild.WorkerSupportsCancellation = true;
            DocBuild.WorkerReportsProgress = true;
            DocBuild.DoWork += DocBuild_DoWork;
            DocBuild.ProgressChanged += DocBuild_ProgressChanged;
            DocBuild.RunWorkerCompleted += DocBuild_RunWorkerCompleted;
        }
        #region Properties for ProgressBar 
        public int CurrentProgress
        {
            get { return currentProgress; }
            private set
            {
                currentProgress = value;
                OnPropertyChanged("CurrentProgress");
            }
        }
        public int MinValue
        {
            get { return minValue; }
            private set
            {
                minValue = value;
                OnPropertyChanged("MinValue");
            }
        }
        public int MaxValue
        {
            get { return maxValue; }
            private set
            {
                maxValue = value;
                OnPropertyChanged("MaxValue");
            }
        }
        #endregion
        #region Properties for ComboBoxSelectType
        public string TextType
        {
            get { return textType; }
            set
            {
                textType = value;
                OnPropertyChanged("TextType");
            }
        }
        #endregion
        #region Properties for TextBoxLogOut
        public string TextLogOut
        {
            get { return textLogOut; }
            set
            {
                textLogOut = value;
                OnPropertyChanged("TextLogOut");
            }
        }
        #endregion

        public RelayCommand ParsingCommand
        {
            get
            {
                return parsingCommand ?? (parsingCommand = new RelayCommand((o) => 
                { 
                    TaskRun("Path"); 
                }));
            }
        }

        private async void TaskRun(string Path)
        {
            try
            {
                ParserDigiKey parserDigiKey = new ParserDigiKey(MessageHandler);
                await parserDigiKey.ParserInit();
                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                ActionWithExcel actionWithExcel = new ActionWithExcel();
                listOfComponents = actionWithExcel.GetDataFromDocument("Path","NameSheet");

                // since this is a UI event, instantiating the Progress class
                // here will capture the UI thread context
                var progress = new Progress<int>(i => CurrentProgress = i);
                MinValue = 0;
                MaxValue = (listOfComponents.Count - 1) * 2;    //Колво элементов (*2 это для прогрессБара, см ниже)
                // pass this instance to the background task
                _ = OutData(progress, token, parserDigiKey, cts);
            }
            catch (Exception ex)
            {

            }
        }
        private async Task OutData(IProgress<int> p, CancellationToken token, ParserDigiKey parserDigiKey, CancellationTokenSource cts)
        {
            bool ExFlag = false;
            string status = "Ready";
            MessageHandler?.Invoke(this, new ActionEventArgs(Environment.NewLine + "Parsing..."));
            for (int i = 0; i < listOfComponents.Count; i++)
            {
                if (token.IsCancellationRequested)
                {
                    if (!ExFlag)
                        MessageHandler?.Invoke(this, new ActionEventArgs(Environment.NewLine + "Interrupted: The process was interrupted by the user"));
                    MessageHandler?.Invoke(this, new ActionEventArgs(Environment.NewLine + status));
                    p.Report(0);
                    //CheckBtnParsing = false;
                    cts.Dispose();
                    return;
                }
                await parserDigiKey.FindDescPack("CL05A104KA5NNNC");
                p.Report(i);
            }
            DocBuild.RunWorkerAsync(null);
        }
        #region Methods for BackgroundWorker
        private void DocBuild_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int Progress = listOfComponents.Count - 1;
                for (int i = 0; i < listOfComponents.Count; i++)
                {
                    if (DocBuild.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    //обработчик

                    DocBuild.ReportProgress(Progress++);
                }
                e.Result = "Completed";
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void DocBuild_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.CurrentProgress = e.ProgressPercentage;
        }
        private void DocBuild_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageHandler?.Invoke(this, new ActionEventArgs(Environment.NewLine + "Interrupted: The process was interrupted by the user"));
            }
            else if (e.Error != null)
            {
                MessageHandler?.Invoke(this, new ActionEventArgs(Environment.NewLine + "Interrupted: There is no access to some files or the files are occupied by another process"));
            }
            else
            {
                MessageHandler?.Invoke(this, new ActionEventArgs(Environment.NewLine + e.Result.ToString()));
            }
        }
        #endregion
        void ShowAction(object sender, ActionEventArgs e)   //Метод для добавления в событие Action
        {
            TextLogOut += Environment.NewLine + $"{e.Message}";
        }
        // команда добавления
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                (addCommand = new RelayCommand((o) =>
                {
                    if (TextType.Contains("Resistors"))
                    {
                        ResistorsWindow resistorssWindow = new ResistorsWindow(new Resistor());
                        if (resistorssWindow.ShowDialog() == true)
                        {
                            Resistor resistor = resistorssWindow.Resistor;
                            db.Resistors.Add(resistor);
                            db.SaveChanges();
                        }
                    }
                    else if (TextType.Contains("Capacitors"))
                    {
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
                        MessageBoxResult result;
                        result = MessageBox.Show("Please select the type of component you want to add to the database", "Type not selected", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }));
            }
        }

        //// команда редактирования
        //public RelayCommand EditCommand
        //{
        //    get
        //    {
        //        return editCommand ??
        //          (editCommand = new RelayCommand((selectedItem) =>
        //          {
        //              if (selectedItem == null) return;
        //              // получаем выделенный объект
        //              Phone phone = selectedItem as Phone;

        //              Phone vm = new Phone()
        //              {
        //                  Id = phone.Id,
        //                  Company = phone.Company,
        //                  Price = phone.Price,
        //                  Title = phone.Title
        //              };
        //              PhoneWindow phoneWindow = new PhoneWindow(vm);


        //              if (phoneWindow.ShowDialog() == true)
        //              {
        //                  // получаем измененный объект
        //                  phone = db.Phones.Find(phoneWindow.Phone.Id);
        //                  if (phone != null)
        //                  {
        //                      phone.Company = phoneWindow.Phone.Company;
        //                      phone.Title = phoneWindow.Phone.Title;
        //                      phone.Price = phoneWindow.Phone.Price;
        //                      db.Entry(phone).State = EntityState.Modified;
        //                      db.SaveChanges();
        //                  }
        //              }
        //          }));
        //    }
        //}
        //// команда удаления
        //public RelayCommand DeleteCommand
        //{
        //    get
        //    {
        //        return deleteCommand ??
        //          (deleteCommand = new RelayCommand((selectedItem) =>
        //          {
        //              if (selectedItem == null) return;
        //              // получаем выделенный объект
        //              Phone phone = selectedItem as Phone;
        //              db.Phones.Remove(phone);
        //              db.SaveChanges();
        //          }));
        //    }
        //}
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
