namespace CsvParser;

public class Row
{
    public int id { get; set; }
    public DateTime tpep_pickup_datetime { get; set; }
    public DateTime tpep_dropoff_datetime { get; set; }
    public int? passenger_count { get; set; }
    public double trip_distance { get; set; }
    public string store_and_fwd_flag { get; set; }
    public int PULocationID { get; set; }
    public int DOLocationID { get; set; }
    public double fare_amount { get; set; }
    public double tip_amount { get; set; }
}
