using System.Globalization;
using EFCore.NamingConventions.Internal;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Modules.Authentication.Infrastructure.DB;

public class AppIdentityDbContext : IdentityDbContext
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("identity");

        var snakeCaseRewriter = new SnakeCaseNameRewriter(CultureInfo.InvariantCulture);
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName is not null && tableName.StartsWith("AspNet"))
            {
                var cleanName = tableName.Replace("AspNet", "");
                var snakeCaseName = snakeCaseRewriter.RewriteName(cleanName);
                entity.SetTableName(snakeCaseName);
            }
        }
    }
}