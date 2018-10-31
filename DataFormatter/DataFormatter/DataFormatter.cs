using CsvHelper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aksharapura.DataFormatter
{
    public class DataFormatter
    {
        private List<string> sourceData;
        private Dictionary<string, List<KeyValuePair<string, string>>> parsedData;
        private int count = 1;

        public DataFormatter LoadData(string fileUrl)
        {
            sourceData = new List<string>(System.IO.File.ReadAllLines(fileUrl));

            Console.WriteLine($"Total Lines : {sourceData.Count}");

            return this;
        }

        public DataFormatter ParseAsDictfmt()
        {
            parsedData = new Dictionary<string, List<KeyValuePair<string, string>>>();
            string engWord = string.Empty;
            var termList = new List<KeyValuePair<string, string>>();

            foreach (var line in sourceData)
            {
                if(char.IsLetterOrDigit(line, 0))
                {
                    engWord = line.Trim();                    
                }
                else if(char.IsWhiteSpace(line, 0))
                {
                    termList.Add(new KeyValuePair<string, string>(
                        "en",
                        engWord)
                        );
                    termList.Add(new KeyValuePair<string, string>(
                        "ml",
                        Regex.Replace(line.Trim(), @"^\d+\W+", "")) // replace whitespaces and numbered bullets
                        );

                    parsedData.Add(count.ToString(), termList);

                    termList = new List<KeyValuePair<string, string>>();
                    count++;
                }
            }

            Console.WriteLine($"Total terms: {count}");

            return this;
        }

        public DataFormatter ParseAsCsv()
        {
            throw new NotImplementedException();            
        }

        public DataFormatter ParseAsJson()
        {
            throw new NotImplementedException();
        }

        public DataFormatter SaveAsCsv(string fileUrl, params string[] filterKeys)
        {

            using (TextWriter textWriter = File.CreateText(fileUrl))
            {
                var csvWriter = new CsvWriter(textWriter);

                foreach(var key in parsedData.Keys)
                {
                    List<KeyValuePair<string, string>> allKeyValuePairs = parsedData[key];

                    dynamic csvRecord = new ExpandoObject();
                    foreach (var filterKey in filterKeys)
                    {
                        var item = allKeyValuePairs
                            .Where(x => x.Key == filterKey)
                            .FirstOrDefault();
                        AddProperty(csvRecord, filterKey, item.Value);
                    }
                    csvWriter.WriteRecord(csvRecord);
                    csvWriter.NextRecord();

                }

                csvWriter.Flush();
            }
            return this;
        }

        public DataFormatter SaveAsJson(string fileUrl, params string[] filterKeys)
        {
            throw new NotImplementedException();
        }

        public DataFormatter SaveAsDictfmt(string fileUrl, params string[] filterKeys)
        {
            throw new NotImplementedException();
        }

        private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
