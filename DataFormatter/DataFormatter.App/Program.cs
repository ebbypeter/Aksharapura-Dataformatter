using System;

namespace Aksharapura.DataFormatter.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var sourceFile = @"D:\OneDrive\Misc\Malayalam\Raw Data\TestData.txt";
            var targetFile_dictfmt = @"D:\OneDrive\Misc\Malayalam\Raw Data\TestData_temp_dictfmt.txt";
            var targetFile_csv = @"D:\OneDrive\Misc\Malayalam\Raw Data\TestData_temp_csv.txt";

            var formatter = new DataFormatter();
            formatter.LoadData(sourceFile)
                .ParseAsDictfmt("en", "ml", "test", "test desc")
                //.TransformAsDictfmt();
                .TransformAsCsv("en", "ml");
                //.SaveAsDictfmt(targetFile_dictfmt)
                //.SaveAsCsv(targetFile_csv, "en", "ml");

            Console.ReadKey();
        }
    }
}
