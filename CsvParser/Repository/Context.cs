using Microsoft.EntityFrameworkCore;

namespace CsvParser.Repository;

public class Context : DbContext
{
    public DbSet<Row> Row { get; set; }

    public Context()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=(local)\\SQLEXPRESS;Initial Catalog=TableStorage;Integrated Security=True;TrustServerCertificate=true");
    }
}
