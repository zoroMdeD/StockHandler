using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ActionWithExcel
    {
        private List<string> listNameSheets { get; set; }
        public ConnectToExcel connectToExcel { get; private set; }
        public Data Data { get; private set; }

        public ActionWithExcel()
        {

        }
        public List<string> GetDataFromDocument(string path, string nameSheet, ConnectToExcel connectToExcel)
        {
            int numSheet = 0;
            try
            {
                this.connectToExcel = connectToExcel;
                listNameSheets = connectToExcel.GetAllWorksheetNames();
                for (int i = 0; i < listNameSheets.Count; i++)
                {
                    if (listNameSheets[i].Contains(nameSheet))
                    {
                        numSheet = i;
                        break;
                    }
                }
                Data = new Data(listNameSheets[numSheet]);
                return Data.GetData(connectToExcel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
