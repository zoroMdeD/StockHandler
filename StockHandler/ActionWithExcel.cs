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
        private ConnectToExcel ConnectToExcel { get; set; }
        public Data Data { get; private set; }

        public ActionWithExcel()
        {

        }
        public List<string> GetDataFromDocument(string path, string nameSheet)
        {
            int numSheet = 0;
            try
            {
                ConnectToExcel = new ConnectToExcel(path);
                listNameSheets = ConnectToExcel.UpdateWorksheet(ConnectToExcel);
                for (int i = 0; i < listNameSheets.Count; i++)
                {
                    if (listNameSheets[i].Contains(nameSheet))
                    {
                        numSheet = i;
                        break;
                    }
                }
                Data = new Data(listNameSheets[numSheet]);
                return Data.GetData(ConnectToExcel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
