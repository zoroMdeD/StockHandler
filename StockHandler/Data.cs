using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class Data
    {
        private string nameOfSheet { get; set; }
        public List<string> ListOfComponents { get; private set; }
        public string KeyWord { get; set; }
        public Data()
        {

        }
        public Data(string nameOfSheet = "Лист1")
        {
            ListOfComponents = new List<string>();
            this.nameOfSheet = nameOfSheet;
        }
        public List<string> GetListOfResistors()
        {
            return ListOfComponents;
        }
        public List<string> GetData(ConnectToExcel ConnectToExcel)
        {
            try
            {
                //Query a worksheet with a header row (sintax SQL-Like LINQ)
                var GetSheet = from a in ConnectToExcel.UrlConnection.Worksheet<Data>(nameOfSheet)
                               select a;
                foreach (var result in GetSheet)
                {
                    if (!string.IsNullOrEmpty(result.KeyWord) && (result.KeyWord.Contains("РЕЗИСТОР")  || result.KeyWord.Contains("КОНДЕНСАТОР")  || 
                                                                  result.KeyWord.Contains("РЕЗИСТОРЫ") || result.KeyWord.Contains("КОНДЕНСАТОРЫ") || 
                                                                  result.KeyWord.Contains("резистор")  || result.KeyWord.Contains("конденсатор")  || 
                                                                  result.KeyWord.Contains("резисторы") || result.KeyWord.Contains("конденсаторы") ||
                                                                  result.KeyWord.Contains("Резистор")  || result.KeyWord.Contains("Конденсатор")  ||
                                                                  result.KeyWord.Contains("Резисторы") || result.KeyWord.Contains("Конденсаторы")))
                        ListOfComponents.Add(result.KeyWord);
                }
                return ListOfComponents;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}