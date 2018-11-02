using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace Aksharapura.DataFormatter
{
    public partial class DataFormatter
    {
        private List<string> sourceData;
        private Dataset parsedDataset;

        public DataFormatter LoadData(string fileUrl)
        {
            sourceData = new List<string>(File.ReadAllLines(fileUrl));

            Console.WriteLine($"Total Lines in source : {sourceData.Count}");

            return this;
        }
    }
}
