using CsvHelper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace Aksharapura.DataFormatter
{
    public partial class DataFormatter
    {
        public DataFormatter ParseAsCsv(string sourceLang, string targetLang, bool hasHeader = true, string name = "", string desc = "")
        {
            if (parsedDataset == null)
            {
                parsedDataset = new Dataset()
                {
                    Name = name,
                    Description = desc,
                    SourceItems = new List<DataItem>(),
                    TranslationItems = new List<DataItem>()
                };
            }

            bool isHeader = true;
            foreach (var line in sourceData)
            {
                if(isHeader)
                {
                    isHeader = false;
                    continue;
                }

                using (TextReader textReader = new StringReader(line))
                {
                    using (var csvReader = new CsvReader(textReader))
                    {
                        csvReader.Read();

                        var sourceWord = csvReader[0];
                        var translatedWord = csvReader[1];

                        var sourceDataItem = parsedDataset.SourceItems
                            .Where(item => item.Data.ToLowerInvariant().Equals(sourceWord.ToLowerInvariant()))
                            .FirstOrDefault();
                        if (sourceDataItem == null)
                        {
                            sourceDataItem = new DataItem()
                            {
                                Id = (parsedDataset.SourceItems.Count + 1).ToString(),
                                Lang = sourceLang,
                                Data = sourceWord.Trim()
                            };
                            parsedDataset.SourceItems.Add(sourceDataItem);
                        }

                        var translatedItem = new DataItem()
                        {
                            Id = sourceDataItem.Id,
                            Lang = targetLang,
                            Data = translatedWord.Trim()
                        };
                        parsedDataset.TranslationItems.Add(translatedItem);
                    }
                }
                
            }

            return this;
        }

        public DataFormatter TransformAsCsv(string sourceLang, string targetLang)
        {
            targetData = new List<string>();
            using (TextWriter textWriter = new StringWriter())
            {
                using (var csvWriter = new CsvWriter(textWriter))
                {
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

                    targetData.AddRange(textWriter.ToString().Replace("\r\n","\n").Split('\n'));
                }
            }
            return this;
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
