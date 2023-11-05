using CsvParser;

Console.WriteLine("Hello, World!");
var path = @"C:\Users\itach\Downloads\sample-cab-data.csv";
await FileReaderService.FileReader(path);