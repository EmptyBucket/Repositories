using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Samples;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FooDbContext>
{
    public FooDbContext CreateDbContext(string[] args)
    {
        var postgreSettings = new NpgsqlConnectionStringBuilder
                { Host = "localhost", Database = "postgres", Username = "postgres", Password = "postgres" }
            .ConnectionString;
        var dbContextOptions = new DbContextOptionsBuilder<FooDbContext>().UseNpgsql(postgreSettings).Options;
        return new FooDbContext(dbContextOptions);
    }
}