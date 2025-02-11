namespace TX.RMC.DataService.MongoDB;

using global::MongoDB.Driver;
using global::MongoDB.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

internal class MongoDbContext : DbContext
{
    public MongoDbContext()
    { }

    public MongoDbContext(DbContextOptions<MongoDbContext> options)
        : base(options)
    { }

    public DbSet<Models.User> Users { get; init; } = null!;
    public DbSet<Models.Robot> Robots { get; init; } = null!;

    public static MongoDbContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<MongoDbContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.User>().Property(u => u.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Models.Robot>().Property(r => r.Id).ValueGeneratedOnAdd();
    }
}
