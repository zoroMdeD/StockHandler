using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ConnectToExcel
    {
        public string PathExcelFile { get; private set; }
        public ExcelQueryFactory UrlConnexion { get; private set; }
        private List<string> listWorksheetNames { get; set; }

        public ConnectToExcel(string path)
        {
            listWorksheetNames = new List<string>();
            PathExcelFile = path;
            UrlConnexion = new ExcelQueryFactory(PathExcelFile);
        }
        public List<string> GetAllWorksheetNames()
        {
            return listWorksheetNames;
        }
        public List<string> TryUpdateWorksheet(ConnectToExcel ConxObject)
        {
            try
            {
                var worksheetNames = ConxObject.UrlConnexion.GetWorksheetNames();
                foreach (var result in worksheetNames)
                {
                    listWorksheetNames.Add(result);
                }
                return listWorksheetNames;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
