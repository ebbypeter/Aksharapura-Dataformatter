using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aksharapura.DataFormatter
{
    public partial class DataFormatter
    {
        public DataFormatter ParseAsDictfmt(string sourceLang, string targetLang, string name = "", string desc = "")
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

            DataItem sourceDataItem = null;
            foreach (var line in sourceData)
            {
                if (char.IsLetterOrDigit(line, 0))
                {
                    string sourceWord = line.Trim();

                    sourceDataItem = parsedDataset.SourceItems
                        .Where(item => item.Data.ToLowerInvariant().Equals(sourceWord.ToLowerInvariant()))
                        .FirstOrDefault();

                    if (sourceDataItem == null)
                    {
                        sourceDataItem = new DataItem()
                        {
                            Id = (parsedDataset.SourceItems.Count + 1).ToString(),
                            Lang = sourceLang,
                            Data = sourceWord
                        };
                        parsedDataset.SourceItems.Add(sourceDataItem);
                    }
                }
                else if (char.IsWhiteSpace(line, 0))
                {
                    string translatedWord = Regex.Replace(line.Trim(), @"^\d+\W+", ""); // replace whitespaces and numbered bullets
                    var translatedItem = new DataItem()
                    {
                        Id = sourceDataItem.Id,
                        Lang = targetLang,
                        Data = translatedWord.Trim()
                    };
                    parsedDataset.TranslationItems.Add(translatedItem);
                }
            }

            return this;
        }

        public DataFormatter TransformAsDictfmt()
        {
            targetData = new List<string>();
            targetData.Add($"Name : {parsedDataset.Name}");
            targetData.Add(parsedDataset.Description);

            foreach (var sourceItem in parsedDataset.SourceItems)
            {
                targetData.Add(sourceItem.Data);

                var translatedItems = parsedDataset.TranslationItems
                    .Where(item => item.Id.Equals(sourceItem.Id, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                int count = 1;
                foreach (var translatedItem in translatedItems)
                {
                    targetData.Add($"\t{count}. {translatedItem.Data}");
                    count++;
                }
            }
            return this;
        }

        public DataFormatter SaveAsDictfmt(string fileUrl)
        {
            using (TextWriter textWriter = File.CreateText(fileUrl))
            {
                textWriter.WriteLine($"Name : {parsedDataset.Name}");
                textWriter.WriteLine(parsedDataset.Description);

                foreach (var sourceItem in parsedDataset.SourceItems)
                {
                    textWriter.WriteLine(sourceItem.Data);

                    var translatedItems = parsedDataset.TranslationItems
                        .Where(item => item.Id.Equals(sourceItem.Id, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();

                    int count = 1;
                    foreach(var translatedItem in translatedItems)
                    {
                        textWriter.WriteLine($"\t{count}. {translatedItem.Data}");
                        count++;
                    }
                }
            }
            return this;
        }
    }
}
