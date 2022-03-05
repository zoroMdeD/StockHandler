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
        public StorageComponents storageComponents;// = new StorageComponents();
        static BackgroundWorker DocBuild;

        CapacitorModel capacitor;
        ResistorModel resistor;
        private StorageComponents selectedItem;
        private List<StorageComponents> components;

        private ApplicationContext db;

        private RelayCommand parsingCommand;    //Комманда парсинг
        RelayCommand saveCommand;       //Комманда сохранения
        private RelayCommand findCommand;       //Комманда поиска
        RelayCommand analogueCommand;   //Комманда подбора аналога
        RelayCommand selectCommand;     //Комманда выбора типа элемента

        //-------------Menu strip-------------
        RelayCommand fileCommand;       //Комманда открыть меню Файл   
        private RelayCommand openFileCommand;   //Комманда открыть файл
        RelayCommand saveFileCommand;   //Комманда сохранить файл
        RelayCommand quitCommand;       //Комманда выход
        
        RelayCommand commandCommand;    //Комманда открыть меню комманды
        RelayCommand infoCommand;       //Комманда открыть меню информация
        RelayCommand helpCommand;       //Комманда открыть меню помощь
        //-----------End Menu strip-----------

        private RelayCommand addCommand;
        private RelayCommand editCommand;
        private RelayCommand deleteCommand;

        IEnumerable<Resistor> resistors;
        IEnumerable<Capacitor> capacitors;

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
        public StorageComponents SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        public List<StorageComponents> Components
        {
            get { return components; }
            private set
            {
                components = value;
                OnPropertyChanged("Components");
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
        public ApplicationViewModel()
        {
            db = new ApplicationContext();
            db.Resistors.Load();
            db.Capacitors.Load();
            Resistors = db.Resistors.Local.ToBindingList();
            Capacitors = db.Capacitors.Local.ToBindingList();
            MessageHandler += ShowAction;
            storageComponents = new StorageComponents();
            storageComponents.MessageHandler += ShowAction;
            listOfComponents = new List<string>();

            if (capacitor != null)
                storageComponents.Edit(capacitor, capacitor.Id);
            
            PercentLoad = $"{CurrentProgress}%";

            DocBuild = new BackgroundWorker();
            DocBuild.WorkerSupportsCancellation = true;
            DocBuild.WorkerReportsProgress = true;
            DocBuild.DoWork += DocBuild_DoWork;
            DocBuild.ProgressChanged += DocBuild_ProgressChanged;
            DocBuild.RunWorkerCompleted += DocBuild_RunWorkerCompleted;
        }

        public RelayCommand ParsingCommand
        {
            get
            {
                return parsingCommand ?? (parsingCommand = new RelayCommand((o) => 
                {
                    MessageHandler?.Invoke(this, new ActionEventArgs("Run"));
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
                    MessageHandler?.Invoke(this, new ActionEventArgs("Error: Keyword(s) not found"));
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
                    //MessageHandler?.Invoke(this, new ActionEventArgs("Ready"));
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
            DateTime ThToday = DateTime.Now;
            string ThData = ThToday.ToString("HH:mm:ss" + "  --->  ");
            if (TextLogOut != null)
                TextLogOut += Environment.NewLine + ThData + $"{e.Message}";
            else
                TextLogOut += ThData + $"{e.Message}";
        }
        public RelayCommand OpenFileCommand     //Команда открытия файла
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
        public RelayCommand FindCommand     //Команда поиска
        {
            get
            { 
                return findCommand ?? 
                (findCommand = new RelayCommand((o) =>
                {
                    Components = new List<StorageComponents>();
                    storageComponents.Clear();
                    if (TextType.Contains("Resistors"))
                    {
                        var query = from u in Resistors
                                    where !string.IsNullOrEmpty(SelectedFindComponent) ? Regex.Replace(u.Resistance, @"\D+", string.Empty) == Regex.Replace(SelectedFindComponent, @"\D+", string.Empty) : string.IsNullOrEmpty(SelectedFindComponent)
                                    select u;
                        foreach (var Resistors in query)
                        {
                            storageComponents.Add(new ResistorModel(Resistors.PartNumber, Resistors.Resistance, Resistors.Power, Resistors.Accuracy, Resistors.Size, Resistors.Count, Resistors.Id));
                        }
                        Components = storageComponents.GetAll();
                        MessageHandler?.Invoke(this, new ActionEventArgs($"Found component(s) by parameter: \"{SelectedFindComponent}\""));
                    }
                    else if (TextType.Contains("Capacitors"))
                    {
                        var query = from u in Capacitors
                                    where !string.IsNullOrEmpty(SelectedFindComponent) ? Regex.Replace(u.Capacity, @"\D+", string.Empty) == Regex.Replace(SelectedFindComponent, @"\D+", string.Empty) : string.IsNullOrEmpty(SelectedFindComponent)
                                    select u;

                        foreach (var Capacitors in query)
                        {
                            storageComponents.Add(new CapacitorModel(Capacitors.PartNumber, Capacitors.Capacity, Capacitors.Voltage, Capacitors.TCoefficient, Capacitors.Size, Capacitors.Count, Capacitors.Id));
                        }
                        Components = storageComponents.GetAll();
                        MessageHandler?.Invoke(this, new ActionEventArgs($"Found component(s) by parameter: \"{SelectedFindComponent}\""));
                    }
                }));
            }
        }
        public RelayCommand AddCommand      //Команда добавления
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
                            MessageHandler?.Invoke(this, new ActionEventArgs($"Added component with part number: \"{resistor.PartNumber}\""));
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
                            MessageHandler?.Invoke(this, new ActionEventArgs($"Added component with part number: \"{capacitor.PartNumber}\""));
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
        public RelayCommand EditCommand     //Команда редактирования
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
                        resistor = SelectedItem as ResistorModel;

                        Resistor vm = new Resistor()
                        {
                            Id = resistor.Id,
                            PartNumber = resistor.PartNumber,
                            Resistance = resistor.PropertyOne,
                            Power = resistor.PropertyTwo,
                            Accuracy = resistor.PropertyThree,
                            Size = resistor.Size,
                            Count = resistor.Count,
                        };
                        ResistorsWindow resistorsWindow = new ResistorsWindow(vm);

                        if (resistorsWindow.ShowDialog() == true)
                        {
                            Resistor resistorDB = new Resistor();
                            //Получаем измененный объект
                            resistorDB = db.Resistors.Find(resistorsWindow.Resistor.Id);     
                            if (resistorDB != null)
                            {
                                resistorDB.PartNumber = resistorsWindow.Resistor.PartNumber;
                                resistorDB.Resistance = resistorsWindow.Resistor.Resistance;
                                resistorDB.Power = resistorsWindow.Resistor.Power;
                                resistorDB.Accuracy = resistorsWindow.Resistor.Accuracy;
                                resistorDB.Size = resistorsWindow.Resistor.Size;
                                resistorDB.Count = resistorsWindow.Resistor.Count;

                                resistor.Id = resistorDB.Id;
                                resistor.PartNumber = resistorDB.PartNumber;
                                resistor.PropertyOne = resistorDB.Resistance;
                                resistor.PropertyTwo = resistorDB.Power;
                                resistor.PropertyThree = resistorDB.Accuracy;
                                resistor.Size = resistorDB.Size;
                                resistor.Count = resistorDB.Count;

                                db.Entry(resistorDB).State = EntityState.Modified;
                                db.SaveChanges();
                                MessageHandler?.Invoke(this, new ActionEventArgs($"Changed component with part number: \"{resistor.PartNumber}\""));
                            }
                        }
                    }
                    if (TextType.Contains("Capacitors"))
                    {
                        if (SelectedItem == null) 
                            return;
                        //Получаем выделенный объект
                        capacitor = SelectedItem as CapacitorModel;

                        Capacitor vm = new Capacitor()
                        {
                            Id = capacitor.Id,
                            PartNumber = capacitor.PartNumber,
                            Capacity = capacitor.PropertyOne,
                            Voltage = capacitor.PropertyTwo,
                            TCoefficient = capacitor.PropertyThree,
                            Size = capacitor.Size,
                            Count = capacitor.Count,
                        };
                        CapacitorsWindow capacitorsWindow = new CapacitorsWindow(vm);

                        if (capacitorsWindow.ShowDialog() == true)
                        {
                            Capacitor capacitorDB = new Capacitor();
                            capacitorDB = db.Capacitors.Find(capacitorsWindow.Capacitor.Id);     //Получаем измененный объект
                            if (capacitorDB != null)
                            {
                                capacitorDB.PartNumber = capacitorsWindow.Capacitor.PartNumber;
                                capacitorDB.Capacity = capacitorsWindow.Capacitor.Capacity;
                                capacitorDB.Voltage = capacitorsWindow.Capacitor.Voltage;
                                capacitorDB.TCoefficient = capacitorsWindow.Capacitor.TCoefficient;
                                capacitorDB.Size = capacitorsWindow.Capacitor.Size;
                                capacitorDB.Count = capacitorsWindow.Capacitor.Count;

                                capacitor.Id = capacitorDB.Id;
                                capacitor.PartNumber = capacitorDB.PartNumber;
                                capacitor.PropertyOne = capacitorDB.Capacity;
                                capacitor.PropertyTwo = capacitorDB.Voltage;
                                capacitor.PropertyThree = capacitorDB.TCoefficient;
                                capacitor.Size = capacitorDB.Size;
                                capacitor.Count = capacitorDB.Count;

                                db.Entry(capacitorDB).State = EntityState.Modified;
                                db.SaveChanges();
                                MessageHandler?.Invoke(this, new ActionEventArgs($"Changed component with part number: \"{capacitor.PartNumber}\""));
                            }
                        }
                    }
                }));
            }
        }
        public RelayCommand DeleteCommand   //Команда удаления
        {
            get
            {
                return deleteCommand ??
                (deleteCommand = new RelayCommand((SelectedItem) =>
                {
                    Components = new List<StorageComponents>();
                    if (TextType.Contains("Resistors"))
                    {
                        if (SelectedItem == null) 
                            return;
                        //Получаем выделенный объект
                        resistor = SelectedItem as ResistorModel;
                        Resistor vm = new Resistor()
                        {
                            Id = resistor.Id,
                            PartNumber = resistor.PartNumber,
                            Resistance = resistor.PropertyOne,
                            Accuracy = resistor.PropertyTwo,
                            Power = resistor.PropertyThree,
                            Size = resistor.Size,
                            Count = resistor.Count
                        };
                        var customer = db.Resistors.Single(o => o.Id == resistor.Id);
                        db.Resistors.Remove(customer);
                        db.SaveChanges();
                        
                        storageComponents.Remove(resistor.Id);
                        Components = storageComponents.GetAll();
                        MessageHandler?.Invoke(this, new ActionEventArgs($"Removed component with part number: \"{resistor.PartNumber}\""));
                    }
                    else if (TextType.Contains("Capacitors"))
                    {
                        if (SelectedItem == null) 
                            return;
                        //Получаем выделенный объект
                        capacitor = SelectedItem as CapacitorModel;
                        Capacitor vm = new Capacitor()
                        {
                            Id = capacitor.Id,
                            PartNumber = capacitor.PartNumber,
                            Capacity = capacitor.PropertyOne,
                            Voltage = capacitor.PropertyTwo,
                            TCoefficient = capacitor.PropertyThree,
                            Size = capacitor.Size,
                            Count = capacitor.Count
                        };
                        var customer = db.Capacitors.Single(o => o.Id == capacitor.Id);
                        db.Capacitors.Remove(customer);
                        db.SaveChanges();

                        storageComponents.Remove(capacitor.Id);
                        Components = storageComponents.GetAll();
                        MessageHandler?.Invoke(this, new ActionEventArgs($"Removed component with part number: \"{capacitor.PartNumber}\""));
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
