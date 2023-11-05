using CsvParser.Repository;
using EFCore.BulkExtensions;
using System.Text;

namespace CsvParser;

public class FileReaderService
{
    public static async Task FileReader(string path)
    {
        var table = new List<Row>();

        using var reader = new StreamReader(path);
        await Console.Out.WriteLineAsync("Parsing...");

        await reader.ReadLineAsync();
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            var values = line?.Split(',');

            if (values.Any(v => string.IsNullOrEmpty(v.Trim())))
            {
                continue;
            }

            if (values?.Length > 3)
            {
                var row = new Row();

                TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime pickupUtc = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(values[1].Trim()), est);
                row.tpep_pickup_datetime = pickupUtc;

                DateTime dropoffUtc = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(values[2].Trim()), est);
                row.tpep_dropoff_datetime = dropoffUtc;

                int.TryParse(values[3].Trim(), out int passengerCount);
                row.passenger_count = passengerCount;

                row.trip_distance = double.Parse(values[4].Trim());
                row.store_and_fwd_flag = string.IsNullOrEmpty(values[6])
                    ? string.Empty
                    : values[6].Trim().Contains('N')
                        ? "No"
                        : "Yes";
                row.PULocationID = int.Parse(values[7].Trim());
                row.DOLocationID = int.Parse(values[8].Trim());
                row.fare_amount = double.Parse(values[10].Trim());
                row.tip_amount = double.Parse(values[13].Trim());

                table.Add(row);
            }
        }

        await Console.Out.WriteLineAsync("Done");

        await Console.Out.WriteLineAsync("Searchig for duplicates...");
        var duplicates = GetDuplicates(table);

        await CreateDuplicatesFile(path, duplicates);

        await RecordToDb(table);
    }

    private static IEnumerable<Row> GetDuplicates(List<Row> table)
    {
        var query = table.GroupBy(r => new { r.tpep_pickup_datetime, r.tpep_dropoff_datetime, r.passenger_count })
              .Where(g => g.Skip(1).Any())
              .SelectMany(r => r);
        return query.DistinctBy(r => new { r.tpep_pickup_datetime, r.tpep_dropoff_datetime, r.passenger_count });
    }

    private static async Task CreateDuplicatesFile(string path, IEnumerable<Row> duplicates)
    {
        var duplicatesCsv = new StringBuilder();

        duplicatesCsv.AppendLine("tpep_pickup_datetime,tpep_dropoff_datetime,passenger_count,trip_distance,store_and_fwd_flag,PULocationID,DOLocationID,fare_amount,tip_amount");
        foreach (var d in duplicates)
        {
            duplicatesCsv.AppendLine($"{d.tpep_pickup_datetime},{d.tpep_dropoff_datetime},{d.passenger_count},{d.trip_distance},{d.store_and_fwd_flag},{d.PULocationID},{d.DOLocationID},{d.fare_amount},{d.tip_amount}");
        }

        await File.WriteAllTextAsync(path.Remove(path.LastIndexOf('\\') + 1) + "duplicates.csv", duplicatesCsv.ToString());
    }

    private static async Task RecordToDb(List<Row> models)
    {
        using Context db = new();
        Console.WriteLine("Writing to db...");

        try
        {
            await db.BulkInsertAsync(models, options =>
            {
                options.UpdateByProperties = new List<string>() {
                    nameof(Row.tpep_pickup_datetime),
                    nameof(Row.tpep_dropoff_datetime),
                    nameof(Row.passenger_count) };
            });

            Console.WriteLine("Done");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}