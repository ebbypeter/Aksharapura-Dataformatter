using System;

namespace Aksharapura.DataFormatter.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var sourcePath = @"D:\OneDrive\Misc\Malayalam\Raw Data\Malayalam Dict #02.txt";
            var testPath = @"D:\Dev\Aashan\TestDataT1.txt";

            var formatter = new DataFormatter();
            formatter.LoadData(sourcePath)
                .ParseAsDictfmt()                
                .SaveAsCsv(testPath, "en", "ml");

            Console.ReadKey();
        }
    }
}
