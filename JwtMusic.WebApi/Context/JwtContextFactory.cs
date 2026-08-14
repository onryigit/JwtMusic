using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JwtMusic.WebApi.Context;

public class JwtContextFactory : IDesignTimeDbContextFactory<JwtContext>
{
    public JwtContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var options = new DbContextOptionsBuilder<JwtContext>()
            .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
            .Options;
        return new JwtContext(options);
    }
}
