namespace TX.RMC.DataService.MongoDB;

using global::MongoDB.Driver;
using global::MongoDB.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

public class MongoDbContext : DbContext
{
    public MongoDbContext()
    { }

    public MongoDbContext(DbContextOptions<MongoDbContext> options)
        : base(options)
    { }

    public DbSet<Models.Command> Commands { get; init; } = null!;
    public DbSet<Models.User> Users { get; init; } = null!;
    public DbSet<Models.Robot> Robots { get; init; } = null!;

    public static MongoDbContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<MongoDbContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Command>().ToCollection("commands");
        modelBuilder.Entity<Models.Command>().HasOne(c => c.User).WithMany(u => u.Commands).HasForeignKey(c => c.UserId).IsRequired();
        modelBuilder.Entity<Models.Command>().HasOne(c => c.Robot).WithMany(r => r.Commands).HasForeignKey(c => c.RobotId).IsRequired();
        modelBuilder.Entity<Models.Command>()
            .HasOne<Models.Command>()
            .WithOne()
            .HasForeignKey<Models.Command>(c => c.ReplacedByCommandId)
            .IsRequired(false);

        modelBuilder.Entity<Models.User>().ToCollection("users");
        modelBuilder.Entity<Models.User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<Models.Robot>().ToCollection("robots");
        modelBuilder.Entity<Models.Robot>().HasIndex(r => r.NameIdentity).IsUnique();
    }
}
