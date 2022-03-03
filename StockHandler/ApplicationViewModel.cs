using ApiClient;
using ApiClient.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

        ConnectToExcel connectToExcel;
        private List<string> listOfComponents;
        public StorageComponents storageComponents;
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
        //IEnumerable<List<StorageComponents>> components;

        #region Fields for LaberPercent
        private string percentLoad;
        #endregion
        #region Fields for ProgressBar
        private int currentProgress;
        private int minValue;
        private int maxValue;
        #endregion
        #region Fields for TextBoxLogOut
        private string textLogOut;
        #endregion
        #region Fields for TextBoxInputValue
        private string selectedFindComponent;
        #endregion
        #region Fields for ComboBoxSelectType
        private string textType;
        #endregion
        #region Fields for TextBoxComponentInfo
        private string componentInfo;
        #endregion
        #region Fields for ComboBoxSelectSheet
        private List<string> listOfSheets;
        private string textSheet;
        #endregion

        private List<StorageComponents> selectedItem;

        public List<StorageComponents> SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

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

        private List<StorageComponents> components;

        public List<StorageComponents> Components
        {
            get { return components; }
            private set
            {
                components = value;
                OnPropertyChanged("Components");
            }
        }

        public ApplicationViewModel()
        {
            db = new ApplicationContext();
            db.Resistors.Load();
            db.Capacitors.Load();
            Resistors = db.Resistors.Local.ToBindingList();
            Capacitors = db.Capacitors.Local.ToBindingList();
            storageComponents = new StorageComponents();

            listOfComponents = new List<string>();
            MessageHandler += ShowAction;

            PercentLoad = $"{CurrentProgress}%";

            DocBuild = new BackgroundWorker();
            DocBuild.WorkerSupportsCancellation = true;
            DocBuild.WorkerReportsProgress = true;
            DocBuild.DoWork += DocBuild_DoWork;
            DocBuild.ProgressChanged += DocBuild_ProgressChanged;
            DocBuild.RunWorkerCompleted += DocBuild_RunWorkerCompleted;
        }
        #region Properties for LabelPercent
        public string PercentLoad
        {
            get { return percentLoad; }
            private set
            {
                percentLoad = value;
                OnPropertyChanged("PercentLoad");
            }
        }
        #endregion
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
        #region Properties for ComboBoxSelectSheet
        public List<string> ListOfSheets
        {
            get { return listOfSheets; }
            private set
            {
                listOfSheets = value;
                OnPropertyChanged("ListOfSheets");
            }
        }
        public string TextSheet
        {
            get { return textSheet; }
            set
            {
                textSheet = value;
                OnPropertyChanged("TextSheet");
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
        #region Properties for TextBoxComponentInfo
        public string ComponentInfo
        {
            get { return componentInfo; }
            set
            {
                componentInfo = value;
                OnPropertyChanged("ComponentInfo");
            }
        }
        #endregion
        #region Properties for TextBoxInputValue
        public string SelectedFindComponent
        {
            get { return selectedFindComponent; }
            set
            {
                selectedFindComponent = value;
                OnPropertyChanged("TelectedFindComponent");
            }
        }
        #endregion
        public RelayCommand ParsingCommand
        {
            get
            {
                return parsingCommand ?? (parsingCommand = new RelayCommand((o) => 
                { 
                    TaskRun(connectToExcel.PathExcelFile); 
                }));
            }
        }

        private async void TaskRun(string path)
        {
            try
            {
                ParserDigiKey parserDigiKey = new ParserDigiKey(MessageHandler);
                await parserDigiKey.ParserInit();
                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                ActionWithExcel actionWithExcel = new ActionWithExcel();
                listOfComponents = actionWithExcel.GetDataFromDocument(path, TextSheet, connectToExcel);
                if (listOfComponents.Count != 0)
                {
                    var progress = new Progress<int>(i => CurrentProgress = i);     //Since this is a UI event, instantiating the Progress class here will capture the UI thread context
                    MinValue = 0;
                    MaxValue = (listOfComponents.Count - 1) * 2;
                    await OutData(progress, token, parserDigiKey, cts);   //Pass this instance to the background task
                }
                else
                    MessageHandler?.Invoke(this, new ActionEventArgs("Error: Keyword not found"));
            }
            catch (Exception ex)
            {

            }
        }
        private async Task OutData(IProgress<int> p, CancellationToken token, ParserDigiKey parserDigiKey, CancellationTokenSource cts)
        {
            bool ExFlag = false;
            MessageHandler?.Invoke(this, new ActionEventArgs("Parsing..."));
            for (int i = 0; i < listOfComponents.Count; i++)
            {
                if (token.IsCancellationRequested)
                {
                    if (!ExFlag)
                        MessageHandler?.Invoke(this, new ActionEventArgs("Interrupted: The process was interrupted by the user"));
                    MessageHandler?.Invoke(this, new ActionEventArgs("Ready"));
                    p.Report(0);
                    //CheckBtnParsing = false;
                    cts.Dispose();
                    return;
                }
                await parserDigiKey.FindDescPack(listOfComponents[i]);
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
                MessageHandler?.Invoke(this, new ActionEventArgs("Interrupted: The process was interrupted by the user"));
            }
            else if (e.Error != null)
            {
                MessageHandler?.Invoke(this, new ActionEventArgs("Interrupted: There is no access to some files or the files are occupied by another process"));
            }
            else
            {
                MessageHandler?.Invoke(this, new ActionEventArgs(e.Result.ToString()));
            }
        }
        #endregion
        void ShowAction(object sender, ActionEventArgs e)   //Метод для добавления в событие Action
        {
            if(TextLogOut != null)
                TextLogOut += Environment.NewLine + $"{e.Message}";
            else
                TextLogOut += $"{e.Message}";
        }
        public RelayCommand OpenFileCommand
        {
            get
            {
                return openFileCommand ?? (openFileCommand = new RelayCommand((o) =>
                {
                    connectToExcel = new ConnectToExcel(OpenReadFile());
                    List<string> tmpListSheetNames = connectToExcel.UpdateWorksheet(connectToExcel);
                    ListOfSheets = tmpListSheetNames;
                }));
            }
        }
        private string OpenReadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xls;*.xlsx;)|*.xls;*.xlsx";
            openFileDialog.FileName = "Select file";
            if (openFileDialog.ShowDialog() == true)
                MessageHandler?.Invoke(this, new ActionEventArgs($"File selected: {openFileDialog.FileName}"));
            return openFileDialog.FileName;
        }
        public RelayCommand FindCommand
        {
            get
            { 
                return findCommand ?? (findCommand = new RelayCommand((o) =>
                {
                    //string strOut = string.Empty;
                    //if (TextType.Contains("Resistors"))
                    //{
                    //    var query = from u in Resistors
                    //                where Regex.Replace(u.Resistance, @"\D+", string.Empty) == Regex.Replace(SelectedFindComponent, @"\D+", string.Empty)
                    //                select u;

                    //    foreach (var Resistors in query)
                    //    {
                    //        strOut = $"PartNumber: {Resistors.PartNumber}" + Environment.NewLine
                    //                + $"Resistance: {Resistors.Resistance}" + Environment.NewLine
                    //                + $"Power: {Resistors.Power}" + Environment.NewLine
                    //                + $"Accuracy: {Resistors.Accuracy}" + Environment.NewLine
                    //                + $"Size: {Resistors.Size}" + Environment.NewLine
                    //                + $"Count: {Resistors.Count}" + Environment.NewLine;

                    //        if (ComponentInfo != null)
                    //            ComponentInfo += Environment.NewLine + $"{strOut}";
                    //        else
                    //            ComponentInfo += $"{strOut}";
                    //    }
                    //}
                    //else if(TextType.Contains("Capacitors"))
                    //{
                    //    var query = from u in Capacitors
                    //                where Regex.Replace(u.Capacity, @"\D+", string.Empty) == Regex.Replace(SelectedFindComponent, @"\D+", string.Empty)
                    //                select u;

                    //    foreach (var Capacitors in query)
                    //    {
                    //        strOut = $"PartNumber: {Capacitors.PartNumber}" + Environment.NewLine
                    //                + $"Capacity: {Capacitors.Capacity}" + Environment.NewLine
                    //                + $"Voltage: {Capacitors.Voltage}" + Environment.NewLine
                    //                + $"TCoefficient: {Capacitors.TCoefficient}" + Environment.NewLine
                    //                + $"Size: {Capacitors.Size}" + Environment.NewLine
                    //                + $"Count: {Capacitors.Count}" + Environment.NewLine;

                    //        if (ComponentInfo != null)
                    //            ComponentInfo += Environment.NewLine + $"{strOut}";
                    //        else
                    //            ComponentInfo += $"{strOut}";
                    //    }
                    //}

                    if (TextType.Contains("Capacitors"))
                    {
                        //CapacitorModel capacitorModel;
                        var query = from u in Capacitors
                                    where Regex.Replace(u.Capacity, @"\D+", string.Empty) == Regex.Replace(SelectedFindComponent, @"\D+", string.Empty)
                                    select u;

                        foreach (var Capacitors in query)
                        {
                            //CapacitorModel capacitorModel = new CapacitorModel(Capacitors.PartNumber, Capacitors.Capacity, Capacitors.Voltage, Capacitors.TCoefficient, Capacitors.Size, Capacitors.Count);
                            storageComponents.Add(new CapacitorModel(Capacitors.PartNumber, Capacitors.Capacity, Capacitors.Voltage, Capacitors.TCoefficient, Capacitors.Size, Capacitors.Count));
                            //strOut = $"PartNumber: {Capacitors.PartNumber}" + Environment.NewLine
                            //        + $"Capacity: {Capacitors.Capacity}" + Environment.NewLine
                            //        + $"Voltage: {Capacitors.Voltage}" + Environment.NewLine
                            //        + $"TCoefficient: {Capacitors.TCoefficient}" + Environment.NewLine
                            //        + $"Size: {Capacitors.Size}" + Environment.NewLine
                            //        + $"Count: {Capacitors.Count}" + Environment.NewLine;

                            //if (ComponentInfo != null)
                            //    ComponentInfo += Environment.NewLine + $"{strOut}";
                            //else
                            //    ComponentInfo += $"{strOut}";
                        }
                        //if(Components != null)
                        //    Components.Clear();
                        Components = storageComponents.GetAll();  //= (IEnumerable<List<StorageComponents>>)storageComponents.GetAll();
                        
                        //if (SelectedItem == null)
                        //    return;
                        ////Получаем выделенный объект
                        //Capacitor capacitor = SelectedItem as Capacitor;

                        //Capacitor vm = new Capacitor()
                        //{
                        //    Id = capacitor.Id,
                        //    PartNumber = capacitor.PartNumber,
                        //    Capacity = capacitor.Capacity,
                        //    Voltage = capacitor.Voltage,
                        //    TCoefficient = capacitor.TCoefficient,
                        //    Size = capacitor.Size,
                        //    Count = capacitor.Count,
                        //};
                        //CapacitorsWindow capacitorsWindow = new CapacitorsWindow(vm);

                        //if (capacitorsWindow.ShowDialog() == true)
                        //{
                        //    capacitor = db.Capacitors.Find(capacitorsWindow.Capacitor.Id);     //Получаем измененный объект
                        //    if (capacitor != null)
                        //    {
                        //        capacitor.PartNumber = capacitorsWindow.Capacitor.PartNumber;
                        //        capacitor.Capacity = capacitorsWindow.Capacitor.Capacity;
                        //        capacitor.Voltage = capacitorsWindow.Capacitor.Voltage;
                        //        capacitor.TCoefficient = capacitorsWindow.Capacitor.TCoefficient;
                        //        capacitor.Size = capacitorsWindow.Capacitor.Size;
                        //        capacitor.Count = capacitorsWindow.Capacitor.Count;
                        //        db.Entry(capacitor).State = EntityState.Modified;
                        //        db.SaveChanges();
                        //    }
                        //}
                    }

                }));
            }
        }
        public RelayCommand AddCommand  // команда добавления
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
        public RelayCommand EditCommand         //Команда редактирования
        {
            get
            {
                return editCommand ??
                  (editCommand = new RelayCommand((SelectedItem) =>
                  {
                      if (TextType.Contains("Resistors"))
                      {
                          if (SelectedItem == null) 
                              return;
                          //Получаем выделенный объект
                          Resistor resistor = SelectedItem as Resistor;

                          Resistor vm = new Resistor()
                          {
                              Id = resistor.Id,
                              PartNumber = resistor.PartNumber,
                              Resistance = resistor.Resistance,
                              Power = resistor.Power,
                              Accuracy = resistor.Accuracy,
                              Size = resistor.Size,
                              Count = resistor.Count,
                          };
                          ResistorsWindow resistorsWindow = new ResistorsWindow(vm);

                          if (resistorsWindow.ShowDialog() == true)
                          {
                              resistor = db.Resistors.Find(resistorsWindow.Resistor.Id);     //Получаем измененный объект
                              if (resistor != null)
                              {
                                  resistor.PartNumber = resistorsWindow.Resistor.PartNumber;
                                  resistor.Resistance = resistorsWindow.Resistor.Resistance;
                                  resistor.Power = resistorsWindow.Resistor.Power;
                                  resistor.Accuracy = resistorsWindow.Resistor.Accuracy;
                                  resistor.Size = resistorsWindow.Resistor.Size;
                                  resistor.Count = resistorsWindow.Resistor.Count;
                                  db.Entry(resistor).State = EntityState.Modified;
                                  db.SaveChanges();
                              }
                          }
                      }
                      if (TextType.Contains("Capacitors"))
                      {
                          if (SelectedItem == null) 
                              return;
                          //Получаем выделенный объект
                          Capacitor capacitor = SelectedItem as Capacitor;

                          Capacitor vm = new Capacitor()
                          {
                              Id = capacitor.Id,
                              PartNumber = capacitor.PartNumber,
                              Capacity = capacitor.Capacity,
                              Voltage = capacitor.Voltage,
                              TCoefficient = capacitor.TCoefficient,
                              Size = capacitor.Size,
                              Count = capacitor.Count,
                          };
                          CapacitorsWindow capacitorsWindow = new CapacitorsWindow(vm);

                          if (capacitorsWindow.ShowDialog() == true)
                          {
                              capacitor = db.Capacitors.Find(capacitorsWindow.Capacitor.Id);     //Получаем измененный объект
                              if (capacitor != null)
                              {
                                  capacitor.PartNumber = capacitorsWindow.Capacitor.PartNumber;
                                  capacitor.Capacity = capacitorsWindow.Capacitor.Capacity;
                                  capacitor.Voltage = capacitorsWindow.Capacitor.Voltage;
                                  capacitor.TCoefficient = capacitorsWindow.Capacitor.TCoefficient;
                                  capacitor.Size = capacitorsWindow.Capacitor.Size;
                                  capacitor.Count = capacitorsWindow.Capacitor.Count;
                                  db.Entry(capacitor).State = EntityState.Modified;
                                  db.SaveChanges();
                              }
                          }
                      }
                  }));
            }
        }
        public RelayCommand DeleteCommand           //Команда удаления
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand((SelectedItem) =>
                  {
                      if (TextType.Contains("Resistors"))
                      {
                          if (SelectedItem == null) 
                              return;
                          //Получаем выделенный объект
                          Resistor resistor = SelectedItem as Resistor;
                          db.Resistors.Remove(resistor);
                          db.SaveChanges();
                      }
                      if (TextType.Contains("Capacitors"))
                      {
                          if (SelectedItem == null) 
                              return;
                          //Получаем выделенный объект
                          Capacitor capacitor = SelectedItem as Capacitor;
                          db.Capacitors.Remove(capacitor);
                          db.SaveChanges();
                      }
                  }));
            }
        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
