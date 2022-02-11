using ApiClient;
using ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ParserDigiKey
    {
        private string specialSpace;

        private List<string> partNumber;
        private List<string> family;

        Dictionary<string, string> charReplace;

        private ApiClientSettings settings;
        private ApiClientService client;
        //private ActionWithExcel ActionWithExcel;

        public ParserDigiKey()
        {
            charReplace = new Dictionary<string, string>();
            partNumber = new List<string>();
            family = new List<string>();

            charReplace.Add("А", "A");
            charReplace.Add("В", "B");
            charReplace.Add("С", "C");
            charReplace.Add("Е", "E");
            charReplace.Add("Н", "H");
            charReplace.Add("К", "K");
            charReplace.Add("М", "M");
            charReplace.Add("О", "O");
            charReplace.Add("Р", "P");
            charReplace.Add("Т", "T");
            charReplace.Add("Х", "X");
            charReplace.Add("а", "a");
            charReplace.Add("с", "c");
            charReplace.Add("е", "e");
            charReplace.Add("о", "o");
            charReplace.Add("р", "p");
            charReplace.Add("х", "x");

            byte[] utf8SpecialSpace = new byte[] { 0xC2, 0xA0 };
            specialSpace = Encoding.GetEncoding("UTF-8").GetString(utf8SpecialSpace);
        }
        public async Task<string> ParserInit()
        {
            try
            {
                settings = ApiClientSettings.CreateFromConfigFile();
                client = new ApiClientService(settings);
                
                //if (settings.ExpirationDateTime < DateTime.Now)
                //{
                //    // Let's refresh the token
                //    var oAuth2Service = new OAuth2Service(settings);
                //    var oAuth2AccessToken = await oAuth2Service.RefreshTokenAsync();
                //    if (oAuth2AccessToken.IsError)
                //    {
                //        // Current Refresh token is invalid or expired 
                //        return "Current Refresh token is invalid or expired ";
                //    }

                //    settings.UpdateAndSave(oAuth2AccessToken);

                //    return "After call to refresh" + Environment.NewLine + settings.ToString();
                //}

                return Environment.NewLine + "Ready";
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool FindCyrillicSymbol(string keyword)
        {
            var cyrillic = Enumerable.Range(1024, 256).Select(ch => (char)ch);
            bool result = keyword.Any(cyrillic.Contains);
            return result;
        }
        public List<string> FindSpecialSymbol(List<string> listKeyword)
        {
            for (int i = 0; i < listKeyword.Count; i++)
            {
                listKeyword[i] = (listKeyword[i].Replace(" ", "")).Replace(specialSpace, "");
                if (FindCyrillicSymbol(listKeyword[i]))
                {
                    foreach (KeyValuePair<string, string> pair in charReplace)
                    {
                        listKeyword[i] = listKeyword[i].Replace(pair.Key, pair.Value);
                    }
                }
            }
            return listKeyword;
        }
    }
}
