using CsvHelper;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace Aksharapura.DataFormatter
{
    public partial class DataFormatter
    {
        public DataFormatter ParseAsCsv(string name, string desc)
        {
            throw new NotImplementedException();
        }

        public DataFormatter SaveAsCsv(string fileUrl, string sourceLang, string targetLang)
        {
            using (TextWriter textWriter = File.CreateText(fileUrl))
            {
                var csvWriter = new CsvWriter(textWriter);

                foreach (DataItem sourceItem in parsedDataset.SourceItems)
                {
                    if (sourceItem.Lang.Trim().Equals(sourceLang.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        var translatedItems = parsedDataset.TranslationItems
                            .Where(item => item.Id.Equals(sourceItem.Id, StringComparison.InvariantCultureIgnoreCase) 
                                && item.Lang.Trim().Equals(targetLang.Trim(), StringComparison.InvariantCultureIgnoreCase))
                            .ToList();

                        foreach (var translatedItem in translatedItems)
                        {
                            dynamic csvRecord = new ExpandoObject();
                            AddProperty(csvRecord, sourceLang, sourceItem.Data);
                            AddProperty(csvRecord, targetLang, translatedItem.Data);

                            csvWriter.WriteRecord(csvRecord);
                            csvWriter.NextRecord();
                        }
                    }
                }
                csvWriter.Flush();
            }
            return this;
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
