﻿using ApiClient;
using ApiClient.Models;
using ApiClient.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHandler
{
    public class ParserDigiKey : IEventHandler
    {
        private string specialSpace;
        private string subStr = string.Empty;
        private int startIndex = 0;
        private int endIndex = 0;
        private char[] charToTrim = { ' ', '\n', '\"', '\\', '\r' };

        private List<string> partNumber;
        private List<string> family;
        private List<string> package;

        Dictionary<string, string> charReplace;

        private ApiClientSettings settings;
        private ApiClientService client;

        public event EventHandler<ActionEventArgs> MessageHandler;

        //private ActionWithExcel ActionWithExcel;

        public ParserDigiKey(ApiClientService client, ApiClientSettings settings)
        {
            this.settings = settings;   //ApiClientSettings.CreateFromConfigFile();
            this.client = client;       //new ApiClientService(settings);

            charReplace = new Dictionary<string, string>();
            partNumber = new List<string>();
            family = new List<string>();
            package = new List<string>();

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
        public async Task ParserInit()
        {
            try
            {
                MessageHandler?.Invoke(this, new ActionEventArgs(Environment.NewLine + "Ready"));

                if (settings.ExpirationDateTime < DateTime.Now)
                {
                    // Let's refresh the token
                    var oAuth2Service = new OAuth2Service(settings);
                    var oAuth2AccessToken = await oAuth2Service.RefreshTokenAsync();
                    if (oAuth2AccessToken.IsError)
                    {
                        // Current Refresh token is invalid or expired 
                        //return "Current Refresh token is invalid or expired ";
                    }

                    settings.UpdateAndSave(oAuth2AccessToken);

                    //return "After call to refresh" + Environment.NewLine + settings.ToString();
                }

                //return Environment.NewLine + "Ready";
            }
            catch (Exception)
            {
                throw;
            }

            //return Task.CompletedTask;
        }

        public async Task FindDescPack(string partNumber)
        {
            try
            {
                var response = await client.KeywordSearch(partNumber);

                subStr = "\"ExactManufacturerProductsCount\":";
                startIndex = response.IndexOf(subStr);
                string tmpResponse = response.Substring(startIndex);
                startIndex = tmpResponse.IndexOf(subStr);
                endIndex = tmpResponse.IndexOf(',');
                int num = int.Parse((tmpResponse.Substring(startIndex + subStr.Length, endIndex - (startIndex + subStr.Length))).Trim(charToTrim));
                if (num != 0)
                    FindFamily(FindPackage(tmpResponse));
                else
                    FindFamily(FindPackage(response));
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void FindFamily(string response)
        {
            subStr = "\"Family\",\"Value\":";
            if (response.IndexOf(subStr) != -1)
            {
                startIndex = response.IndexOf(subStr);
                response = response.Substring(startIndex);
                subStr = "\"Value\":";
                startIndex = response.IndexOf(subStr);
                endIndex = response.IndexOf('}');

                family.Add((response.Substring(startIndex + subStr.Length, endIndex - (startIndex + subStr.Length))).Trim(charToTrim));
            }
            else
            {
                family.Add("null");
            }
        }
        private string FindPackage(string response)
        {
            subStr = "\"Parameter\":\"Package / Case\",";
            if (response.IndexOf(subStr) != -1)
            {
                startIndex = response.IndexOf(subStr);
                response = response.Substring(startIndex);
                subStr = "\"Value\":";
                startIndex = response.IndexOf(subStr);
                endIndex = response.IndexOf("}");
                package.Add((response.Substring(startIndex + subStr.Length, endIndex - (startIndex + subStr.Length))).Trim(charToTrim));
            }
            else
            {
                package.Add("null");
            }

            return response;
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
